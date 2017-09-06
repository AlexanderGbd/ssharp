using UnityEngine;

namespace SafetySharp.CaseStudies.HRICell.Modeling
{
    using SafetySharp.Modeling;
    public class Camera : Component
    {
        public Vector2 Position = new Vector2(0, 0);
        public bool IsRecording { get; private set; } = true;
        /// <summary>
        /// The fault that prevents the camera from recording
        /// </summary>
        //public readonly ISSE.SafetyChecking.Modeling.Fault SuppressRecording = new ISSE.SafetyChecking.Modeling.PermanentFault();
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
            //Move method here or still in Robot?
        }

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
