using UnityEngine;

namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using SafetySharp.Modeling;
    using ISSE.SafetyChecking.Modeling;
    public class Camera : Component
    {
        public Vector3 Position = new Vector3(0, 0, 0);
        public bool IsRecording { get; private set; } = true;
        /// <summary>
        /// The fault that prevents the camera from recording
        /// </summary>
        public readonly Fault SuppressRecording = new PermanentFault();

        [FaultEffect(Fault = nameof(SuppressRecording))]
        public class SuppressRecordingEffect : Camera
        {
            public override void Update()
            {
                IsRecording = false;
            }
        }

        public override void Update()
        {

        }

        public float GetXCoord()
        {
            return Position.x;
        }

        public float GetYCoord()
        {
            return Position.y;
        }

        public float GetZCoord()
        {
            return Position.z;
        }

        public Vector3 GetPosition()
        {
            return Position;
        }
    }
}
