using UnityEngine;
namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using SafetySharp.Modeling;

    public class StaticObstacle : Component
    {
        private Vector2 Position = new Vector2(1, 2);

        public float GetXCoord()
        {
            return Position.x;
        }

        public float GetYCoord()
        {
            return Position.y;
        }

        public Vector2 GetPosition()
        {
            return Position;
        }

    }
}
