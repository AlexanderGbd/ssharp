
namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using SafetySharp.Modeling;

    public class Model : ModelBase
    {
        public Model()
        {
            Controller = new Controller
            {
                Robot = new Robot(),
                DynamicObstacle = new DynamicObstacle(),
                StaticObstacle = new StaticObstacle()
            };
        }

    }
}
