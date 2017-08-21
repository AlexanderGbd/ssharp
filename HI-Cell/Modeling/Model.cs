
namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using SafetySharp.Modeling;

    public class Model : ModelBase
    {
        public Model()
        {
            Controller  c = new Controller
            {
                Robot = new Robot()
            };

            var DynObstacle = new DynamicObstacle();
            var StatObstacle = new StaticObstacle();
        }
    }
}
