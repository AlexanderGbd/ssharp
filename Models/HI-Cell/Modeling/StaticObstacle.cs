using UnityEngine;
namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using SafetySharp.Modeling;

    public class StaticObstacle : Component
    {
        private Vector2 Position = new Vector2(4, 3);

        public double GetXCoord()
        {
            return Position.x;
        }

        public double GetYCoord()
        {
            return Position.y;
        }

        public Vector2 GetPosition()
        {
            return Position;
        }

    }
}
