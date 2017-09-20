namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using SafetySharp.Modeling;

    public class Model : ModelBase
    {
        /// <summary>
		///   The target's position
		/// </summary>
		public const float XTarget = 4;
        public const float YTarget = 4;
        public const float ZTarget = 0;

        public Model()
        {
            Controller = new Controller
            {
                Robot = new Robot(),
                Sensor = Sensor.Insstance,
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
            Bind(nameof(Robot.ObstacleInEnvironment), nameof(Sensor.ObstInEnvironment));
            Bind(nameof(DynamicObstacle.IsDetected), nameof(Sensor.ObstInEnvironment));
            Bind(nameof(DynamicObstacle.RobotPosition), nameof(Robot.GetPosition));
            Bind(nameof(Robot.DynamicObstInEnvironment), nameof(Sensor.DynamicObstInEnvironment));
            Bind(nameof(Robot.StaticObstInEnvironment), nameof(Sensor.StaticObstInEnvironment));

            Bind(nameof(Robot.XCalculated), nameof(Controller.XCalculated));
            Bind(nameof(Robot.YCalculated), nameof(Controller.YCalculated));
            /*Bind(nameof(Robot.CameraPosition), nameof(Camera.Position));*/
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
