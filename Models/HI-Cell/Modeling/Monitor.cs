using UnityEngine;

namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using ISSE.SafetyChecking.Modeling;
    using SafetySharp.Modeling;
    class Monitor : Component
    {
        public extern Vector2 RobotPosition { get; }
        public extern Vector2 DynObstaclePosition { get; }
        public extern Vector2 StatObstaclePosition { get; }
        public extern bool SensorIsDetecting { get; }
        public extern bool CameraIsRecording { get; }
        public extern bool RobotIsMoving { get; }
        public extern bool RobotIsNotMoving { get; }
        public extern bool RobotCollided { get; }
        public extern bool RobotIsAtSamePositionAsObst { get; }
        public extern bool RobotIsAtSamePositionAsTarg { get; }
        public extern bool ObstacleDetected { get; }

        public override void Update()
        {
            //To be implemented
        }
    }
}
