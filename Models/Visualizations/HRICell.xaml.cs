namespace SafetySharp.CaseStudies.Visualizations
{
    using System;
    using System.Windows.Media.Animation;
    using HI_Cell.Modeling;
    using Infrastructure;
    using System.Windows;
    using System.Windows.Input;
    using Modeling;
    using System.Windows.Shapes;
    using System.Windows.Media;
    using System.Windows.Controls;

    public partial class HRICell
    {
        private readonly Storyboard _movingStoryboard;
        private readonly Storyboard _sensorAlertStoryboard;
        private readonly Storyboard _cameraAlertStoryboard;
        private readonly Storyboard _dynObstacleStoryboard;
        private Storyboard sb;
        private Model _model;

        public HRICell()
        {
            InitializeComponent();

            //Tests
            sb = new Storyboard();
            //Timeline tl = ;
            //sb.Children.Add();


            // Initialize visualization resources
            _movingStoryboard = (Storyboard)Resources["MoveRobot"];
            Canvas.GetLeft(Robot);
            Canvas.GetTop(Robot);
            _movingStoryboard.SetValue(TranslateTransform.XProperty, Canvas.GetLeft(Robot) + 100);
            _movingStoryboard.Begin();
            _movingStoryboard.Pause();
           
            _dynObstacleStoryboard = (Storyboard)Resources["MoveObstacle"];
            _dynObstacleStoryboard.Begin();
            _dynObstacleStoryboard.Pause();

            _cameraAlertStoryboard = (Storyboard)Resources["CameraWarningOn"];
            _sensorAlertStoryboard = (Storyboard)Resources["SensorWarningOn"];

            // Initialize the simulation environment
            SimulationControls.ModelStateChanged += (o, e) => UpdateModelState();
            SimulationControls.Reset += (o, e) => OnModelStateReset();
            SimulationControls.SetModel(new Model());

            // Initialize the visualization state
            UpdateModelState();

            SimulationControls.MaxSpeed = 64;
            SimulationControls.ChangeSpeed(8);
        }

        private void OnSuppressMoving(object sender, RoutedEventArgs e)
        {
            _model.Robot.SuppressMoving.ToggleActivationMode();
        }

        private void OnSuppressStop(object sender, RoutedEventArgs e) {
            _model.Robot.SuppressStop.ToggleActivationMode();               
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

            if (SimulationControls.Simulator.IsReplay) {
                Canvas.SetTop(Robot, 327.5);
                Canvas.SetLeft(Robot, 104.5);
                Canvas.SetTop(DynamicObstacle, 166.5);
                Canvas.SetLeft(DynamicObstacle, 386);
                return;
            }
            
            _model.Robot.SuppressMoving.Activation = SuppressMoving.IsChecked.ToOccurrenceKind();
            _model.Robot.SuppressStop.Activation = SuppressStop.IsChecked.ToOccurrenceKind();
            _model.Sensor.SuppressDetecting.Activation = SuppressDetecting.IsChecked.ToOccurrenceKind();
            _model.Camera.SuppressRecording.Activation = SuppressRecording.IsChecked.ToOccurrenceKind();
        }

        private void UpdateModelState()
        {
            //Robot
            SuppressMoving.IsChecked = _model.Robot.SuppressMoving.IsActivated;
            SuppressStop.IsChecked = _model.Robot.SuppressStop.IsActivated; 

            if (!_model.Robot.IsMoving && !_model.Sensor.ObstDetected && !_model.Robot.IsCollided && !_model.Robot.SamePositionAsTarg && !SuppressMoving.IsChecked)
            {
                _movingStoryboard.Begin();
            }
            if ((_model.Robot.SamePositionAsObst || _model.Robot.SamePositionAsTarg) && !SuppressStop.IsChecked)
                _movingStoryboard.Stop();

            if (_model.Robot.ObstDetected && !SuppressStop.IsChecked)
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
            Console.WriteLine("\nI WAS HERE!!!!!!!!!!!!!!!!!!!!\n");
            Point position = Robot.TranslatePoint(Robot.RenderTransformOrigin, Robot);
            Robot.TranslatePoint(new Point(position.X + 100, position.Y), Robot);

            Random rnd = new Random();
            position = DynamicObstacle.RenderTransformOrigin;
            DynamicObstacle.TranslatePoint(new Point(position.X + rnd.Next(101), position.Y + rnd.Next(101)), DynamicObstacle);
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Random random = new Random();
            double leftRob = Canvas.GetLeft(Robot);
            double leftObst = Canvas.GetLeft(DynamicObstacle);
            double topObst = Canvas.GetTop(DynamicObstacle);

            if (leftRob + 100 <= 750 && leftRob >= 0) {
                Canvas.SetLeft(Robot, leftRob + 100);
            }
            int rnd = random.Next(-100, 101);
            if (topObst + rnd <= 320 && topObst >= 0) {
                Canvas.SetTop(DynamicObstacle, topObst + random.Next(-100, 101));
            }
            rnd = random.Next(-100, 101);
            if (leftObst + rnd <= 750 && leftObst >= 0)
            {
                Canvas.SetLeft(DynamicObstacle, leftObst + random.Next(-101, 101));
            }



            //Test
            //Rectangle test = new Rectangle();
            //test.Width = 100;
            //test.Height = 100;
            //test.Fill = Brushes.Red;
            //test.Stroke = Brushes.Red;
            //test.StrokeThickness = 4;
            //test.RadiusX = 5;
            //test.RadiusY = 5;
            //canvas.Children.Add(test);
            //Point pos = e.GetPosition(canvas);
            //Canvas.SetLeft(test, (int) pos.X);
            //Canvas.SetTop(test, (int) pos.Y);

            _sensorAlertStoryboard.Begin();
        }
    }
}
