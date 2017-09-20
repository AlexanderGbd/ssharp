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
        public static int Counter = 0;

        private Model _model;

        public HRICell()
        {
            InitializeComponent();

            // Initialize visualization resources

            // Initialize the simulation environment
            SimulationControls.ModelStateChanged += (o, e) => UpdateModelState();
            SimulationControls.Reset += (o, e) => OnModelStateReset();
            SimulationControls.SetModel(new Model());

            // Initialize the visualization state
            UpdateModelState();

            Warning.Opacity = 0;
            SimulationControls.MaxSpeed = 3.0;
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
                return;
            }
            
            _model.Robot.SuppressMoving.Activation = SuppressMoving.IsChecked.ToOccurrenceKind();
            _model.Robot.SuppressStop.Activation = SuppressStop.IsChecked.ToOccurrenceKind();
            _model.Sensor.SuppressDetecting.Activation = SuppressDetecting.IsChecked.ToOccurrenceKind();
            _model.Camera.SuppressRecording.Activation = SuppressRecording.IsChecked.ToOccurrenceKind();
        }

        private void UpdateModelState()
        {
            Console.WriteLine("\nDas ist der "+Counter+". Durchgang: ");

            if (_model.Robot.MonitorText)
            {
                Exception.Opacity = 1.0;
                Exception.Text = "EXCEPTION WAS THROWN!";
            }
            else
                Exception.Text = "EXCEPTION WAS NOT THROWN!";

            //Dynamic Obstacle
            float obstacleX = _model.DynamicObstacle.GetXCoord();
            float obstacleY = _model.DynamicObstacle.GetYCoord();
            Canvas.SetLeft(DynamicObstacle, obstacleX * 100);
            Canvas.SetTop(DynamicObstacle, obstacleY * 100);
            
            Console.WriteLine("Dynamic Obstacle x-Coordinate: "+ Canvas.GetLeft(DynamicObstacle));
            Console.WriteLine("Dynamic Obstacle y-Coordinate: "+ Canvas.GetTop(DynamicObstacle)  + "\n");
            
            //Robot
            float xPos = _model.Robot.GetXCoord();
            float yPos = _model.Robot.GetYCoord();

            Canvas.SetLeft(Robot, xPos * 100);
            Canvas.SetTop(Robot, yPos * 100);

            Console.WriteLine("Robot x-coordinate: "+ Canvas.GetLeft(Robot));
            Console.WriteLine("Robot y-coordinate: "+ Canvas.GetTop(Robot) + "\n");

            SuppressMoving.IsChecked = _model.Robot.SuppressMoving.IsActivated;
            SuppressStop.IsChecked = _model.Robot.SuppressStop.IsActivated;

            //Sensor
            SuppressDetecting.IsChecked = _model.Sensor.SuppressDetecting.IsActivated;


            if (!_model.Sensor.IsDetecting)
            {
                SensorWarning.Opacity = 1.0;
            }
            else
            {
                SensorWarning.Opacity = 0;
            }
            
            //Camera
            SuppressRecording.IsChecked = _model.Camera.SuppressRecording.IsActivated;

            if (!_model.Camera.IsRecording)
            {
                CameraWarning.Opacity = 1.0;
            }
            else
            {
                CameraWarning.Opacity = 0;
            }
            
            //Controller
            switch (_model.Controller.StateMachine.State)
            {
                case Controller.State.IsMoving:
                    ControllerScreen.Text = "Is Moving";
                    break; 
                case Controller.State.NotMoving:
                    ControllerScreen.Text = "Not Moving";
                    break;
                case Controller.State.ObstacleDetected:
                    ControllerScreen.Text = "Obstacle detected";
                    break;
                case Controller.State.Collided:
                    ControllerScreen.Text = "Collided";
                    break;
                case Controller.State.StoppedAtTarget:
                    ControllerScreen.Text = "Stopped at target";
                    break;
            }
            Counter++;
        }


        //private void MoveRobotAnimation(int newCoordinate, TimeSpan duration)
        //{ 
        //    DoubleAnimation animation = new DoubleAnimation(newCoordinate, duration);
        //    Robot.BeginAnimation(Canvas.LeftProperty, animation);
        //}

        }
}
