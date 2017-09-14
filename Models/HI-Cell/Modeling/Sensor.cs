using System;
using UnityEngine;

namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using ISSE.SafetyChecking.Modeling;
    using SafetySharp.Modeling;

    public class Sensor : Component
    {
        /// <summary>
        ///   The fault that doesn't recognise an obstacle, thus causes the robot to collide with an obstacle.
        /// </summary>
        //public readonly ISSE.SafetyChecking.Modeling.Fault SuppressDetecting = new ISSE.SafetyChecking.Modeling.PermanentFault();
        public readonly Fault SuppressDetecting = new PermanentFault();

        /// <summary>
        ///   The positions of the robot, the obstacles and the target
        ///   Note: ew Vector2(DynObstaclePosition.x, DynObstaclePosition.y)
        ///   
        /// </summary>
        private Vector2 RobPosition => RobotPosition;
        private Vector2 StatObstPosition => StatObstaclePosition;
        private Vector2 DynObstPosition => DynObstaclePosition;
        private Vector2 TargetPosition => new Vector2(Model.XTarget, Model.YTarget);

        public bool ObstDetected { get; private set; }
        public bool IsDetecting { get; private set; } = true;
        public bool ObstInEnvironment { get; private set; }

        /// <summary>
        ///   Gets the robot's position
        /// </summary>
        public extern Vector2 RobotPosition { get; }

        /// <summary>
        ///   Gets the position of the dynamic obstacle
        /// </summary>
        public extern Vector2 DynObstaclePosition { get; }

        /// <summary>
        ///   Gets the position of the static obstacle
        /// </summary>
        public extern Vector2 StatObstaclePosition { get; }

        /// <summary>
        ///   Looks if the robot is at the same position as a obstacle
        /// </summary>
        public bool ComparePositions()
        {
            return (int) DynObstPosition.x == (int) RobPosition.x && (int) DynObstPosition.y == (int) RobPosition.y ||
                    (int) StatObstPosition.x == (int) RobPosition.x && (int) StatObstPosition.y == (int) RobPosition.y;
        }

        public bool ScanForObstaclesInNextStep(double x, double y)
        {
            bool obstacleInXDirection = (int)DynObstPosition.x == (int)(RobPosition.x + x) && (int)DynObstPosition.y == (int)(RobPosition.y + y) ||
                                        (int)StatObstPosition.x == (int)(RobPosition.x + x) && (int)StatObstPosition.y == (int)(RobPosition.y + y);
            bool obstacleInXyDirection = (int)DynObstPosition.x == (int)(RobPosition.x + x) && (int)DynObstPosition.y == (int)(RobPosition.y + y) ||
                                         (int)StatObstPosition.y == (int)(RobPosition.y - y) && (int)StatObstPosition.y == (int)(RobPosition.y - y);
            return obstacleInXDirection && obstacleInXyDirection;
        }

        /// <summary>
        /// Scans only the x-direction currently
        /// </summary>
        public bool OneStepScanIsOk()
        {
            return (int) DynObstPosition.x == (int) RobPosition.x + 1 || (int) StatObstPosition.x == (int) RobPosition.x + 1;
        }

        /// <summary>
		///   Gets the value indicating, that the robot has the same position as its target
		/// </summary>
        public bool SamePositionAsTarg => ((int) TargetPosition.x == (int) RobPosition.x && (int) TargetPosition.y == (int) RobPosition.y);

        /// <summary>
        ///   Gets the distance between the robot and the dynamic obstacle
        /// </summary>
        public Vector2 DistanceToDynObstacle => new Vector2(Math.Abs(RobPosition.x-DynObstPosition.x), Math.Abs(RobPosition.y-DynObstPosition.y));
        /// <summary>
        ///   Gets the distance between the robot and the static obstacle
        /// </summary>
        public Vector2 DistanceToStatObstacle => new Vector2(Math.Abs(RobPosition.x - StatObstPosition.x), Math.Abs(RobPosition.y - StatObstPosition.y));

        /// <summary>
        ///   Calculates if the robot is at the same position as an obstacle
        /// </summary>
        public bool IsSamePositionAsObst() {
            return ComparePositions();
        }

        public bool IsSamePositionAsTarg() {
            return (int) TargetPosition[0] == (int) RobPosition.x && (int) TargetPosition[1] == (int) RobPosition.y;
        }

        [FaultEffect(Fault = nameof(SuppressDetecting))]
        public class SuppressDetectingEffect : Sensor
        {
            public override void Update()
            {
                ObstDetected = false;
                ObstInEnvironment = false;
                IsDetecting = false;
            }
        }

        /// <summary>
        /// Updates the sensor's state
        /// Note: ScanForObstaclesInNextStep(0, 1) || ScanForObstaclesInNextStep(1, 1) ||
        /// </summary>
        public override void Update()
        {
            ObstDetected = (ScanForObstaclesInNextStep(1, 0) || ComparePositions());
            ObstInEnvironment = ScanForObstaclesInNextStep(1, 0) || ScanForObstaclesInNextStep(0, 1) || ScanForObstaclesInNextStep(1, 1)
                                || ScanForObstaclesInNextStep(0, -1) || ScanForObstaclesInNextStep(-1, 0) || ScanForObstaclesInNextStep(-1, -1)
                                || ScanForObstaclesInNextStep(-1, 1) || ScanForObstaclesInNextStep(1, -1) || ComparePositions();
        }
    }
}
