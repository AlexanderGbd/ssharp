namespace SafetySharp.CaseStudies.Visualizations
{
    using System;
    using System.Windows.Media.Animation;
    using HI_Cell.Modeling;
    using Infrastructure;
    using System.Windows;
    using System.Windows.Input;
    using Modeling;
    using SafetySharp.Analysis;
    using System.Windows.Controls;

    public partial class HRICell
    {
        private readonly Storyboard _movingStoryboard;
        private readonly Storyboard _sensorAlertStoryboard;
        private readonly Storyboard _cameraAlertStoryboard;
        private readonly Storyboard _dynObstacleStoryboard;
        private Model _model;
        //private RealTimeSafetySharpSimulator realTimeSimulator;
        //private SafetySharpSimulator simulator;
        //int stepDelay;

        public HRICell()
        {
            InitializeComponent();

            // Initialize visualization resources
            _movingStoryboard = (Storyboard)Resources["MoveRobot"];
            //_movingStoryboard.Begin();
           
            //_dynObstacleStoryboard = (Storyboard)Resources["MoveObstacle"];
            //_dynObstacleStoryboard.Begin();
            //_dynObstacleStoryboard.Pause();

            _cameraAlertStoryboard = (Storyboard)Resources["CameraWarningOn"];
            _sensorAlertStoryboard = (Storyboard)Resources["SensorWarningOn"];

            // Initialize the simulation environment
            SimulationControls.ModelStateChanged += (o, e) => UpdateModelState();
            SimulationControls.Reset += (o, e) => OnModelStateReset();
            SimulationControls.SetModel(new Model());

            // Initialize the visualization state
            UpdateModelState();

            Warning.Opacity = 0;
            SensorWarning.Opacity = 0;
            CameraWarning.Opacity = 0;
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
                //Canvas.SetTop(Robot, 327.5);
                //Canvas.SetLeft(Robot, 104.5);
                //Canvas.SetTop(DynamicObstacle, 166.5);
                //Canvas.SetLeft(DynamicObstacle, 386);
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
            float xPos = _model.Robot.GetXCoord();
            if (!_model.Robot.SamePositionAsTarg)
                Canvas.SetLeft(Robot, xPos * 100);
            if (_model.Robot.SamePositionAsTarg)
                Target.Opacity = 0.5;

            //Canvas.GetLeft(Robot);
            //MoveRobotAnimation((int)Canvas.GetLeft(Robot) + 100, new TimeSpan(0, 0, 1));

            SuppressMoving.IsChecked = _model.Robot.SuppressMoving.IsActivated;
            SuppressStop.IsChecked = _model.Robot.SuppressStop.IsActivated;

            if (_model.Robot.IsCollided) {
                _movingStoryboard.Stop();
            }
            if ((_model.Robot.HasStopped || _model.Robot.ObstDetected || _model.Robot.SamePositionAsTarg) && !SuppressStop.IsChecked) {
                _movingStoryboard.Stop();
            }
            if (_model.Robot.HasStopped && !_model.Sensor.ObstDetected && !_model.Robot.IsCollided && !_model.Robot.SamePositionAsTarg && !SuppressMoving.IsChecked) {
                _movingStoryboard.Begin();
            }
            

            ////Dynamic Obstacle
            //DynamicObstacle.Visibility = true.ToVisibility();
            //_dynObstacleStoryboard.Begin();

            //Sensor
            SuppressDetecting.IsChecked = _model.Sensor.SuppressDetecting.IsActivated;
            Warning.Visibility = SuppressDetecting.IsChecked.ToVisibility();
            SensorWarning.Visibility = SuppressDetecting.IsChecked.ToVisibility();

            if (!_model.Sensor.IsDetecting)
                _sensorAlertStoryboard.Begin();
            else
                _sensorAlertStoryboard.Stop();

            //Camera
            SuppressRecording.IsChecked = _model.Camera.SuppressRecording.IsActivated;
            Warning.Visibility = SuppressRecording.IsChecked.ToVisibility();
            CameraWarning.Visibility = SuppressRecording.IsChecked.ToVisibility();

            if (!_model.Camera.IsRecording)
                _cameraAlertStoryboard.Begin();
            else
                _cameraAlertStoryboard.Stop();

            //Controller
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

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Random random = new Random();
            double leftRob = Canvas.GetLeft(Robot);
            double leftObst = Canvas.GetLeft(DynamicObstacle);
            double topObst = Canvas.GetTop(DynamicObstacle);

            if (leftRob + 100 <= 750 && leftRob >= 0) {
                MoveRobotAnimation((int)Canvas.GetLeft(Robot) + 100, new TimeSpan(0, 0, 1));
                //Canvas.SetLeft(Robot, leftRob + 100);
                _model.Robot.Move(true, false);
            }
            int rnd = random.Next(-100, 101);
            if (topObst + rnd <= 400 && topObst >= 0) {
                int value = ChooseValue(-100, 100);
                MoveLeftDynObstacleAnimation((int)Canvas.GetTop(DynamicObstacle) + value, new TimeSpan(0, 0, 1));
                //Canvas.SetTop(DynamicObstacle, topObst + value);

                //if (value == 100)
                //    _model.DynamicObstacle.Move(0, 1);
                //else
                //    _model.DynamicObstacle.Move(0, -1);
            }
            rnd = random.Next(-100, 101);
            if (leftObst + rnd <= 750 && leftObst >= 0)
            {
                int value = ChooseValue(-100, 100);
                MoveTopDynObstacleAnimation((int)Canvas.GetLeft(DynamicObstacle) + value, new TimeSpan(0, 0, 1));
                //Canvas.SetLeft(DynamicObstacle, leftObst + value);

                //if (value == 100)
                //    _model.DynamicObstacle.Move(0, 1);
                //else
                //    _model.DynamicObstacle.Move(0, -1);
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
        }

        private void MoveRobotAnimation(int newCoordinate, TimeSpan duration)
        {
            //PointAnimation animation = new PointAnimation(new Point(300, 300), duration);
            //Robot.BeginAnimation(EllipseGeometry.CenterProperty, animation);

            DoubleAnimation animation = new DoubleAnimation(newCoordinate, duration);
            Robot.BeginAnimation(Canvas.LeftProperty, animation);
        }

        private void MoveLeftDynObstacleAnimation(int newCoordinate, TimeSpan duration)
        {
            DoubleAnimation animation = new DoubleAnimation(newCoordinate, duration);
            DynamicObstacle.BeginAnimation(Canvas.LeftProperty, animation);
        }

        private void MoveTopDynObstacleAnimation(int newCoordinate, TimeSpan duration)
        {
            DoubleAnimation animation = new DoubleAnimation(newCoordinate, duration);
            DynamicObstacle.BeginAnimation(Canvas.TopProperty, animation);
        }

        /// <summary>
        /// Randomly chooses one of two values
        /// </summary>
        private int ChooseValue(int value1, int value2)
        {
            Random random = new Random();
            int rand = random.Next(0, 2);
            if (rand == 0)
                return value1;
            else
                return value2;
        }
    }
}
