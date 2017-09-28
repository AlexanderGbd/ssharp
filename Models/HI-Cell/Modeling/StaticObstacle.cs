using UnityEngine;
namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using SafetySharp.Modeling;

    public class StaticObstacle : Component
    {
        public static Vector3 Position = new Vector3(6, 3, 3);

        public float GetXCoord()
        {
            return Position.x;
        }

        public float GetYCoord()
        {
            return Position.y;
        }

        public Vector3 GetPosition()
        {
            return Position;
        }

    }
}
