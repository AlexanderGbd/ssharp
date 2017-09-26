namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using System;
    using SafetySharp.Modeling;
    using UnityEngine;
    using Component = SafetySharp.Modeling.Component;

    public class Controller : Component
    {
        public float XCalculated => CalculatePathForNextStep().x;
        public float YCalculated => CalculatePathForNextStep().y;

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
                    from: new[] { State.NotMoving },
                    to: State.IsMoving,
                    guard: !Sensor.ObstInEnvironment && !Robot.IsSamePositionAsObst,
                    action: () =>
                    {
                        //DynamicObstacle.Move();
                        Client.getInstance.MoveDirectlyTo(Model.XTarget / 10, Model.YTarget / 10, Model.ZTarget / 10, Math.PI, 0, -Math.PI); 
                        //Robot.Move((int)CalculatePathForNextStep().x, (int)CalculatePathForNextStep().y);
                    })
                //.Transition(
                //    from: State.IsMoving,
                //    to: State.ObstacleDetected,
                //    guard: Sensor.ObstInEnvironment && !Sensor.SamePositionAsTarg,    
                //    action: Robot.Stop
                //    )
                //.Transition(
                //    from: State.IsMoving,
                //    to: State.Collided,
                //    guard: Robot.IsSamePositionAsObst,
                //    action: () => {
                //        Robot.Stop(); 
                //        DynamicObstacle.Stop();

                //    })
                .Transition(
                    from: State.IsMoving,
                    to: State.StoppedAtTarget,
                    guard: Robot.IsSamePositionAsTarg,
                    action: () =>
                    {
                        Robot.Stop();
                        //DynamicObstacle.Stop();
                    })
                //.Transition(
                //    from: State.ObstacleDetected,
                //    to: State.IsMoving,
                //    guard: !Sensor.ObstInEnvironment && !Sensor.IsSamePositionAsTarg(),
                //    action: () =>
                //    {
                //        //Robot.Move((int) CalculatePathForNextStep().x, (int) CalculatePathForNextStep().y);
                //        Client.getInstance.MoveDirectlyTo(Model.XTarget / 10, Model.YTarget / 10, Model.ZTarget / 10, Math.PI, 0, -Math.PI); 
                //    })
                //.Transition(
                //    from: State.ObstacleDetected,
                //    to: State.StoppedAtTarget,
                //    guard: Sensor.SamePositionAsTarg,
                //    action: () =>
                //    {
                //        Robot.Stop();
                //    })
                ;
        }

        /// <summary>
        /// Calculates the coordinates for the robot's next movement
        /// </summary>
        public Vector3 CalculatePathForNextStep()
        {
            Vector3 coordinates = new Vector3(0, 0, 0);
            float xTarget = Model.XTarget;
            float yTarget = Model.YTarget;
            //float xDynObst = DynamicObstacle.GetXCoord();
            //float yDynObst = DynamicObstacle.GetYCoord();
            //float xStatObst = StaticObstacle.GetXCoord();
            //float yStatObst = StaticObstacle.GetYCoord();
            float xRobot = Robot.GetXCoord();
            float yRobot = Robot.GetYCoord();
            //float zRobot = Robot.GetZCoord();

            if (!Robot.SamePositionAsTarg)
            {

                if (xTarget - xRobot > 0 && !Sensor.ScanForObstaclesInNextStep(1, 0))
                    coordinates.x = 1;
                if (xTarget - xRobot < 0 && !Sensor.ScanForObstaclesInNextStep(-1, 0))
                    coordinates.x = -1;
                if (yTarget - yRobot < 0 && !Sensor.ScanForObstaclesInNextStep(0, -1))
                    coordinates.y = -1;
                if (yTarget - yRobot > 0 && !Sensor.ScanForObstaclesInNextStep(0, 1))
                    coordinates.y = 1;

                //if (xTarget - xRobot > 0 && (int)xDynObst != (int)xRobot + 1 && !((int)xStatObst == (int)xRobot + 1 && (int)yStatObst == (int)yRobot))
                //    coordinates.x = 1;
                //if (yTarget - yRobot > 0 && (int)yDynObst != (int)yRobot + 1 && !((int)yStatObst == (int)yRobot + 1 && (int)xStatObst == (int)xRobot))
                //    coordinates.y = 1;
                //if (xTarget - xRobot < 0 && (int)xDynObst != (int)xRobot - 1 && !((int)xStatObst == (int)xRobot - 1 && (int)yStatObst != (int)yRobot))
                //    coordinates.x = -1;
                //if (yTarget - yRobot < 0 && (int)yDynObst != (int)yRobot - 1 && !((int)yStatObst == (int)yRobot - 1 && (int)xStatObst != (int)xRobot))
                //    coordinates.y = -1;
                //if ((int)xTarget - (int)xRobot == 0 && (int)xDynObst != (int)xRobot)
                //    coordinates.x = 0;
                //if ((int)yTarget - (int)yRobot == 0 && (int)yDynObst != (int)yRobot)
                //    coordinates.y = 0;
            }

            Console.WriteLine("X-Coordinate: "+ coordinates.x + "   Y-Coordinate: " + coordinates.y);
            return coordinates;
        }
    }
}
