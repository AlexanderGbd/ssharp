namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using SafetySharp.Modeling;

    public class Model : ModelBase
    {
        /// <summary>
		///   The target's position
		/// </summary>
		public const float XTarget = 5;
        public const float YTarget = 0;

        public Model()
        {
            Controller = new Controller
            {
                Robot = new Robot(),
                DynamicObstacle = new DynamicObstacle(),
                StaticObstacle = new StaticObstacle()
            };

            Bind(nameof(Robot.DynObstaclePosition), nameof(DynamicObstacle.GetPosition));
			Bind(nameof(Robot.StatObstaclePosition), nameof(StaticObstacle.GetPosition));
        }

        [Root(RootKind.Controller)]
        public Controller Controller { get; }

        public Robot Robot => Controller.Robot;
        public DynamicObstacle DynamicObstacle => Controller.DynamicObstacle;
        public StaticObstacle StaticObstacle => Controller.StaticObstacle;

    }
}
