
namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using SafetySharp.Modeling;

    public class Model : ModelBase
    {
        /// <summary>
		///   The target's position
		/// </summary>
		public const double XTarget = 5;
        public const double YTarget = 0;

        public Model()
        {
            Controller  c = new Controller
            {
                 Robot = new Robot()
            };

            var DynObstacle = new DynamicObstacle();
            var StatObstacle = new StaticObstacle();


            Bind(nameof(Robot.DynObstaclePosition), nameof(DynamicObstacle.GetPosition));
			Bind(nameof(Robot.StatObstaclePosition), nameof(StaticObstacle.GetPosition));
        }

    }
}
