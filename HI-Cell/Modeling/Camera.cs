using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using SafetySharp.Modeling;
    public class Camera : Component
    {
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
    }
}
