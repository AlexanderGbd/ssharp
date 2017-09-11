namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using SafetySharp.Modeling;

    public class Model : ModelBase
    {
        /// <summary>
		///   The target's position
		/// </summary>
		public const float XTarget = 5;
        public const float YTarget = 4;

        public Model()
        {
            Controller = new Controller
            {
                Robot = new Robot(),
                Sensor = new Sensor(),
                Camera = new Camera(),
                DynamicObstacle = new DynamicObstacle(),
                StaticObstacle = new StaticObstacle()
            };

            Bind(nameof(Sensor.DynObstaclePosition), nameof(DynamicObstacle.GetPosition));
            Bind(nameof(Sensor.StatObstaclePosition), nameof(StaticObstacle.GetPosition));
            Bind(nameof(Sensor.RobotPosition), nameof(Robot.GetPosition));
            Bind(nameof(Robot.IsSamePositionAsObst), nameof(Sensor.IsSamePositionAsObst));
            Bind(nameof(Robot.IsSamePositionAsTarg), nameof(Sensor.IsSamePositionAsTarg));
            Bind(nameof(Robot.ObstacleDetected), nameof(Sensor.ObstDetected));
            /*Bind(nameof(Robot.CameraPosition), nameof(Camera.Position));*/
            //Monitor bindings         MIT AGENTEN MACHEN WIE IN ROBOT CELL!
            /*Bind(nameof(Monitor.CameraIsRecording), nameof(Camera.IsRecording));
            Bind(nameof(Monitor.SensorIsDetecting), nameof(Sensor.IsDetecting));
            Bind(nameof(Monitor.ObstacleDetected), nameof(Sensor.ObstDetected));
            Bind(nameof(Monitor.RobotIsAtSamePositionAsObst), nameof(Sensor.IsSamePositionAsObst));
            Bind(nameof(Monitor.RobotIsAtSamePositionAsTarg), nameof(Sensor.IsSamePositionAsTarg));
            Bind(nameof(Monitor.RobotPosition), nameof(Camera.Position));
            Bind(nameof(Monitor.RobotIsMoving), nameof(Robot.IsMoving));
            Bind(nameof(Monitor.RobotIsNotMoving), nameof(Robot.HasStopped));
            Bind(nameof(Monitor.RobotCollided), nameof(Robot.IsCollided));
            Bind(nameof(Monitor.DynObstaclePosition), nameof(DynamicObstacle.GetPosition));
            Bind(nameof(Monitor.StatObstaclePosition), nameof(StaticObstacle.GetPosition));*/
        }

        [Root(RootKind.Controller)]
        public Controller Controller { get; }

        public Robot Robot => Controller.Robot;
        public DynamicObstacle DynamicObstacle => Controller.DynamicObstacle;
        public StaticObstacle StaticObstacle => Controller.StaticObstacle;
        public Sensor Sensor => Controller.Sensor;
        public Camera Camera => Controller.Camera;
        

    }
}
