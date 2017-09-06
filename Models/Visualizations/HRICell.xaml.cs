namespace SafetySharp.CaseStudies.Visualizations
{
    using System;
    using System.Windows.Media.Animation;
    using CaseStudies.HRICell.Modeling;
    using Infrastructure;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Shapes;
    using Modeling;


    public partial class HRICell
    {
        private readonly Storyboard _movingStoryboard;
        private readonly Storyboard _sensorAlertStoryboard;
        private readonly Storyboard _cameraAlertStoryboard;
        private readonly Storyboard _dynObstacleStoryboard;
        private Model _model;

        public HRICell()
        {
            InitializeComponent();

            // Initialize visualization resources
            _movingStoryboard = (Storyboard)Resources["MovingRobot"];
            _dynObstacleStoryboard = (Storyboard)Resources["MovingObstacle"];
            _cameraAlertStoryboard = (Storyboard)Resources["CameraEvent"];
            _sensorAlertStoryboard = (Storyboard)Resources["SensorEvent"];

            // Initialize the simulation environment
            SimulationControls.ModelStateChanged += (o, e) => UpdateModelState();
            SimulationControls.Reset += (o, e) => OnModelStateReset();
            SimulationControls.SetModel(new Model());

            // Initialize the visualization state
            UpdateModelState();

            Warning.Opacity = 0;
            SensorWarning.Opacity = 0;
            SimulationControls.MaxSpeed = 64;
            SimulationControls.ChangeSpeed(8);
        }

        private void OnSuppressMoving(object sender, RoutedEventArgs e)
        {
            _model.Robot.SuppressMoving.ToggleActivationMode();
        }

        private void OnSuppressDetecting(object sender, RoutedEventArgs e)
        {
            _model.Sensor.SuppressDetecting.ToggleActivationMode();
        }

        private void OnSuppressRecording(object sender, RoutedEventArgs e) {
            _model.Camera.SuppressRecording.ToggleActivationMode();
        }

        private void OnModelStateReset()
        {
            _model = (Model)SimulationControls.Model;

            if (SimulationControls.Simulator.IsReplay)
                return;

            _model.Robot.SuppressMoving.Activation = SuppressMoving.IsChecked.ToOccurrenceKind();
            _model.Sensor.SuppressDetecting.Activation = SuppressDetecting.IsChecked.ToOccurrenceKind();
            _model.Camera.SuppressRecording.Activation = SuppressRecording.IsChecked.ToOccurrenceKind();
        }

        private void UpdateModelState()
        {
            //Robot
            Robot.Visibility = SuppressMoving.IsChecked.ToVisibility();

            if (!_model.Robot.IsMoving && !_model.Sensor.ObstDetected && !_model.Robot.IsCollided && !_model.Robot.SamePositionAsTarg)
            {
                _movingStoryboard.Begin();
            }
            if (_model.Robot.SamePositionAsObst || _model.Robot.SamePositionAsTarg)
                _movingStoryboard.Stop();

            if (_model.Robot.ObstDetected)
                _movingStoryboard.Pause();

            //Dynamic Obstacle
            DynamicObstacle.Visibility = true.ToVisibility();
            _dynObstacleStoryboard.Begin();

            //Sensor
            SuppressDetecting.IsChecked = _model.Sensor.SuppressDetecting.IsActivated;
            Warning.Visibility = SuppressDetecting.IsChecked.ToVisibility();
            SensorWarning.Visibility = SuppressDetecting.IsChecked.ToVisibility();

            if (!_model.Sensor.IsDetecting)
                _sensorAlertStoryboard.Begin();

            //Camera
            SuppressRecording.IsChecked = _model.Camera.SuppressRecording.IsActivated;
            Warning.Visibility = SuppressRecording.IsChecked.ToVisibility();
            CameraWarning.Visibility = SuppressRecording.IsChecked.ToVisibility();

            if (!_model.Camera.IsRecording)
                _cameraAlertStoryboard.Begin();

            //Controller still to implement
            switch (_model.Controller.StateMachine.State)
            {
                case Controller.State.IsMoving:
                    ControllerScreen.Text = "Is Moving";
                    break; 
                case Controller.State.NotMoving:
                    ControllerScreen.Text = "Not Moving";
                    break;
                case Controller.State.Collision:
                    ControllerScreen.Text = "Collision";
                    break;
                case Controller.State.StoppedAtTarget:
                    ControllerScreen.Text = "Stopped at target";
                    break;
            }

        }

        /// <summary>
        /// This method is invoked, when a key was pressed
        /// </summary>
        private void Canvas_OnKeyDown_(object sender, KeyEventArgs e)
        {
            Point position = Robot.TranslatePoint(Robot.RenderTransformOrigin, Robot);
            Robot.TranslatePoint(new Point(position.X + 100, position.Y), Robot);

            Random rnd = new Random();
            position = DynamicObstacle.RenderTransformOrigin;
            DynamicObstacle.TranslatePoint(new Point(position.X + rnd.Next(101), position.Y + rnd.Next(101)), DynamicObstacle);
        }
    }
}
