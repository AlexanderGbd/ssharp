namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using SafetySharp.Modeling;
    using UnityEngine;
    using Component = SafetySharp.Modeling.Component;

    public class Controller : Component
    {
        public enum State {
            /// <summary>
            ///     Indicates that the sensor detected an obstacle
            /// </summary>
            ObstacleDetected,
            /// <summary>
            ///     Indicates that the robot hit an obstacle, instead of reaching its target
            /// </summary>
            Collided,

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
            Update(DynamicObstacle, StaticObstacle, Sensor, Camera, Robot);

            StateMachine
                .Transition(
                    from: State.NotMoving,
                    to: State.Collided,
                    guard: Robot.SamePositionAsObst,
                    action: () => {
                        Robot.Stop();
                        DynamicObstacle.Stop();
                    })
                .Transition(
                    from: new[] { State.NotMoving },
                    to: State.IsMoving,
                    guard: !Sensor.ObstInEnvironment && !Robot.IsSamePositionAsObst,
                    action: () =>
                    {
                        DynamicObstacle.Move();
                        Robot.Move(true, false);
                    })
                .Transition(
                    from: State.IsMoving,
                    to: State.ObstacleDetected,
                    guard: Sensor.ObstInEnvironment,    
                    action: () => {
                        Robot.Stop();
                        //DynamicObstacle.Stop();
                    })
                .Transition(
                    from: State.IsMoving,
                    to: State.Collided,
                    guard: Robot.IsSamePositionAsObst,
                    action: Robot.Stop
                )
                .Transition(
                    from: State.IsMoving,
                    to: State.StoppedAtTarget,
                    guard: Robot.IsSamePositionAsTarg,
                    action: () => {
                        Robot.Stop();
                        DynamicObstacle.Stop();
                    })
                .Transition(
                    from: State.ObstacleDetected,
                    to: State.NotMoving,
                    guard: Sensor.ObstInEnvironment,
                    action: () =>
                    {
                        //Robot.Stop();
                    })
                //.Transition(
                //    from: State.StoppedAtTarget,
                //    to: State.IsMoving,
                //    guard: !Robot.IsSamePositionAsTarg,
                //    action: () =>
                //    {
                //        Robot.Move(true, false);
                //    })
                ;
        }

        /// <summary>
        /// Calculates the coordinates for the robot's next movement
        /// </summary>
        public Vector2 CalculatePathForNextStep()
        {
            //To be implemented
            return new Vector2(0, 0);
        }
    }
}
