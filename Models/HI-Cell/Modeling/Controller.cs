namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using SafetySharp.Modeling;

    public class Controller : Component
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
        public DynamicObstacle DynamicObstacle;

        /// <summary>
        ///   A static obstacle in the robot's environment
        /// </summary>
        [Hidden, Subcomponent]
        public StaticObstacle StaticObstacle;

        /// <summary>
        ///   The robot's sensor
        /// </summary>
        [Hidden, Subcomponent]
        public Sensor Sensor;

        /// <summary>
        ///   The robot's camera
        /// </summary>
        [Hidden, Subcomponent]
        public Camera Camera;

        /// <summary>
		///   Updates the state of the component.
		/// </summary>
		public override void Update()
        {
            Update(Robot, DynamicObstacle, StaticObstacle, Sensor, Camera);

            StateMachine
                .Transition(
                    from: State.IsMoving,
                    to: State.NotMoving,
                    guard: Robot.IsSamePositionAsTarg || Robot.ObstDetected,
                    action: () => {
                        Robot.Stop();
                        //DynamicObstacle.Stop();
                    })
                .Transition(
                    from: State.IsMoving,
                    to: State.Collision,
                    guard: Robot.SamePositionAsObst,
                    action: () => {
                        Robot.Stop();
                        /*DynamicObstacle.Stop();*/ })
                .Transition(
                    from: new[] { State.NotMoving },
                    to: State.IsMoving,
                    guard: !Sensor.ObstDetected && !Robot.IsSamePositionAsTarg,
                    action: () =>
                    {
                        //DynamicObstacle.Move();
                        Robot.Move(true, false);
                    });
        }

    }
}
