using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using SafetySharp.Modeling;

    class Sensor : Component
    {
        /// <summary>
        ///   The fault that doesn't recognise an obstacle, thus causes the robot to collide with an obstacle.
        /// </summary>
        public readonly Fault SuppressDetecting = new PermanentFault();

        /// <summary>
        ///   The positions of the robot, the obstacles and the target
        /// </summary>
        private Vector2 RobPosition => new Vector2(RobotPosition.x, RobotPosition.y);
        private Vector2 StatObstPosition => new Vector2(StatObstaclePosition.x, StatObstaclePosition.y);
        private Vector2 DynObstPosition => new Vector2(DynObstaclePosition.x, DynObstaclePosition.y);
        private Vector2 TargetPosition => new Vector2(Model.XTarget, Model.YTarget);

        public bool ObstDetected { get; private set; }
        public bool IsDetecting { get; private set; } = true;

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
            return (((DynObstPosition[0] == RobotPosition.x) && (DynObstPosition[1] == RobotPosition.y)) ||
                    ((StatObstPosition[0] == RobotPosition.x) && (StatObstPosition[1] == RobotPosition.y)));
        }

        public bool ScanForObstaclesInNextStep(double x, double y)
        {
            return (((DynObstPosition[0] == RobotPosition.x + x) && (DynObstPosition[1] == RobotPosition.y + y)) ||
                    ((StatObstPosition[0] == RobotPosition.x + x) && (StatObstPosition[1] == RobotPosition.y + y)));
        }
        /// <summary>
        ///   Gets the value indicating, that the robot has the same position as an obstacle
        /// </summary>
        public bool SamePositionAsObst => ComparePositions();
        /// <summary>
		///   Gets the value indicating, that the robot has the same position as its target
		/// </summary>
        public bool SamePositionAsTarg => ((TargetPosition[0] == RobotPosition.x) && (TargetPosition[1] == RobotPosition.y)) ? true : false;

        /// <summary>
        ///   Gets the distance between the robot and the dynamic obstacle
        /// </summary>
        public Vector2 DistanceToDynObstacle => new Vector2(Math.Abs(RobPosition.x-DynObstPosition.x), Math.Abs(RobPosition.y-DynObstPosition.y));
        /// <summary>
        ///   Gets the distance between the robot and the static obstacle
        /// </summary>
        public Vector2 DistanceToStatObstacle => new Vector2(Math.Abs(RobPosition.x - StatObstPosition.x), Math.Abs(RobPosition.y - StatObstPosition.y));

        [FaultEffect(Fault = nameof(SuppressDetecting))]
        public class SuppressDetectingEffect : Sensor
        {
            public override void Update()
            {
                ObstDetected = false;
                IsDetecting = false;
            }
        }
    }
}
