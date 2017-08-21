
namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using SafetySharp.Modeling;
    public class DynamicObstacle : Component
    {
        //private int xCoord;
        //private int yCoord;
        private int[] Position = new int[2];

        public int GetXCoord()
        {
            return Position[0];
        }

        public int GetYCoord()
        {
            return Position[1];
        }

    }
}
