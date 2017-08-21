
namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using SafetySharp.Modeling;

    public class Model : ModelBase
    {
        public Model()
        {
            Controller = new Controller
            {
                Robot = new Robot()
            };

            DynObstacle = new DynamicObstacle();
            StatObstacle = new StaticObstacle();
        }
    }
}
