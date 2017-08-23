
namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using SafetySharp.Modeling;
    public class DynamicObstacle : Component
    {
        private double[] Position = new double[] {3, 1};

        public double GetXCoord()
        {
            return Position[0];
        }

        public double GetYCoord()
        {
            return Position[1];
        }

        public double[] GetPosition() {
            return Position; 
        }
    }
}
