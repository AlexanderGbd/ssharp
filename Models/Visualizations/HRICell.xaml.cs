namespace SafetySharp.CaseStudies.Visualizations
{
    using System;
    using System.Windows.Media.Animation;
    using CaseStudies.HI_Cell.Modeling;
    using Infrastructure;
    using System.Windows;

    public partial class HRICell
    {
        private readonly Storyboard _movingStoryboard;
        private readonly Storyboard _sensorAlertStoryboard;
        private readonly Storyboard _cameraAlertStoryboard;
        private Model _model;

        public HRICell()
        {
            InitializeComponent();

            // Initialize visualization resources
            _movingStoryboard = (Storyboard)Resources["MovingRobot"];
            _movingStoryboard.Begin();
            _movingStoryboard.Pause();

            _cameraAlertStoryboard = (Storyboard)Resources["CameraEvent"];
            _sensorAlertStoryboard = (Storyboard)Resources["SensorEvent"];

            // Initialize the simulation environment
            SimulationControls.ModelStateChanged += (o, e) => UpdateModelState();
            SimulationControls.Reset += (o, e) => OnModelStateReset();
            SimulationControls.SetModel(new Model());

            // Initialize the visualization state
            UpdateModelState();

            //DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            //myDoubleAnimation.From = 100;
            //myDoubleAnimation.To = 200;
            //myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            //Storyboard.SetTargetName(myDoubleAnimation, Robot.Name);
            //Storyboard.SetTargetProperty(myDoubleAnimation,
            //    new PropertyPath(WidthProperty));

            Warning.Opacity = 0;
            SensorWarning.Opacity = 0;
            SimulationControls.MaxSpeed = 64;
            SimulationControls.ChangeSpeed(8);
        }

        private void OnSuppressMoving(object sender, RoutedEventArgs e) {

        }

        private void OnSuppressDetecting(object sender, RoutedEventArgs e)
        {
            
        }

        private void OnSuppressRecording(object sender, RoutedEventArgs e) {

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


            //Sensor


            //Camera


            //Controller still to implement
            switch (_model.Controller.StateMachine.State)
            {
                case Controller.State.IsMoving: break; 
                case Controller.State.NotMoving: break;
                case Controller.State.Collision: break;
                case Controller.State.StoppedAtTarget: break;
            }

        }

        //private void UpdateModelState()
        //{
        //    // Timer
        //    CountDown.Text = _model.Timer.RemainingTime.ToString();
        //    CountDown.Visibility = _model.Timer.IsActive.ToVisibility();
        //    SuppressTimeout.IsChecked = _model.Timer.SuppressTimeout.IsActivated;
        //    TimerFailure.Visibility = SuppressTimeout.IsChecked.ToVisibility();

        //    if (_model.Timer.HasElapsed)
        //        _timerAlertStoryboard.Begin();

        //    hallo.Visibility = (_model.Timer.RemainingTime > 30).ToVisibility();

        //    // Camera
        //    var pressureLevel = Math.Round(_model.Tank.PressureLevel / (double)Model.PressureLimit * 100);
        //    _pressureLevelStoryboard.Seek(TimeSpan.FromMilliseconds(Math.Max(0, 10 * pressureLevel)));
        //    PressureLevel.Text = $"{pressureLevel}%";
        //    PressureLevel.Visibility = (!_model.Tank.IsRuptured).ToVisibility();
        //    TankRupture.Visibility = _model.Tank.IsRuptured.ToVisibility();

        //    // Sensor
        //    SuppressFull.IsChecked = _model.Sensor.SuppressIsFull.IsActivated;
        //    SuppressEmpty.IsChecked = _model.Sensor.SuppressIsEmpty.IsActivated;
        //    SensorFailure.Visibility = (SuppressFull.IsChecked || SuppressEmpty.IsChecked).ToVisibility();

        //    if ((_model.Sensor.IsFull || _model.Sensor.IsEmpty))
        //        _sensorAlertStoryboard.Begin();

        //    // Controller
        //    switch (_model.Controller.StateMachine.State)
        //    {
        //        case Controller.State.Inactive:
        //            ControllerScreen.Text = "Inactive";
        //            break;
        //        case Controller.State.Filling:
        //            ControllerScreen.Text = "Filling";
        //            break;
        //        case Controller.State.StoppedBySensor:
        //            ControllerScreen.Text = "Stopped: Sensor";
        //            break;
        //        case Controller.State.StoppedByTimer:
        //            ControllerScreen.Text = "Stopped: Timer";
        //            break;
        //    }

        //    // Pump
        //    if (!_model.Pump.IsEnabled)
        //        _pumpingStoryboard.Pause();
        //    else
        //        _pumpingStoryboard.Resume();

        //    SuppressPumping.IsChecked = _model.Pump.SuppressPumping.IsActivated;
        //    PumpFailure.Visibility = SuppressPumping.IsChecked.ToVisibility();

        //}
    }
}
