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
            /// <summary>
            ///     Indicates that the robot hit an obstacle, instead of reaching its target
            /// </summary>
            Collision,

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

        /// <summary>
		///   Gets the state machine that manages the state of the controller.
		/// </summary>
		public readonly StateMachine<State> StateMachine = State.NotMoving;

        /// <summary>
		///   The robot that is controlled by the controller.
		/// </summary>
		[Hidden, Subcomponent]
        public Robot Robot;

        /// <summary>
        ///   A dynamic obstacle in the robot's environment
        /// </summary>
        [Hidden, Subcomponent]
        public DynamicObstacle DynObstacle;

        /// <summary>
        ///   A static obstacle in the robot's environment
        /// </summary>
        [Hidden, Subcomponent]
        public StaticObstacle StatObstacle;

        /// <summary>
		///   Updates the state of the component.
		/// </summary>
		public override void Update()
        {
            Update(Robot, DynObstacle, StatObstacle);

            StateMachine
                .Transition(
                    from: State.IsMoving,
                    to: State.NotMoving,
                    guard: !Robot.IsMoving,
                    action: Robot.Stop)
                .Transition(
                    from: State.IsMoving,
                    to: State.Collision,
                    guard: Robot.SamePositionAsObst,
                    action: Robot.Stop)
                .Transition(
                    from: new[] { State.NotMoving },
                    to: State.IsMoving,
                    guard: Robot.SamePositionAsTarg,
                    action: () =>
                    {
                        Robot.Move(1, 0);
                    });
        }

    }
}
