using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using SafetySharp.Modeling;
    class Controller : Component
    {
        public enum State {
   //         /// <summary>
   //         ///   Indicates that the robots' sensors are active
   //         /// </summary>
   //         ActiveSensors,

   //         /// <summary>
   //         ///     Indicates that thsensors aren't active
   //         /// </summary>
   //         InactiveSensors,

            /// <summary>
            ///     Indicates that the robot hit an obstacle, instead of reaching its target
            /// </summary>
            StoppedAtObstacle,

            /// <summary>
            ///     Indicates that the robot stopped at its target
            /// </summary>
            StoppedAtTarget,

            /// <summary>
            ///     Indicates that the robot is moving
            /// </summary>
            IsMoving,

            /// <summary>
            ///     Indicates that the robot stands still
            /// </summary>
            NotMoving
        }

    }
}
