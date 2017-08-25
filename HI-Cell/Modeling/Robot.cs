using UnityEngine;

namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using SafetySharp.Modeling;
    using System;
    

    public class Robot : Component
    {

        private Vector2 Position = new Vector2(0, 0);
        public bool IsMoving { get; private set; }
        public bool ObstDetected { get; private set; }
        public bool IsDetecting { get; private set; } = true;
        public bool HasStopped => !IsMoving;
        public bool IsCollided => SamePositionAsObst;

        /// <summary>
        ///   The positions of the obstacles and the target
        /// </summary>
        private Vector2 StatObstPosition => new Vector2(StatObstaclePosition.x, StatObstaclePosition.y);
        private Vector2 DynObstPosition => new Vector2(DynObstaclePosition.x, DynObstaclePosition.y);
        private Vector2 TargetPosition => new Vector2(Model.XTarget, Model.YTarget);

        /// <summary>
        ///   Gets the position of the dynamic obstacle
        /// </summary>
        public extern Vector2 DynObstaclePosition { get; }

        /// <summary>
        ///   Gets the position of the static obstacle
        /// </summary>
        public extern Vector2 StatObstaclePosition { get; }

        /// <summary>
		///   Gets the value indicating, that the robot has the same position as an obstacle
		/// </summary>
        public bool SamePositionAsObst => ComparePositions();

        /// <summary>
		///   Looks if the robot is at the same position as a obstacle
		/// </summary>
        public bool ComparePositions() {
            return (((DynObstPosition[0] == Position[0]) && (DynObstPosition[1] == Position[1])) ||
                    ((StatObstPosition[0] == Position[0]) && (StatObstPosition[1] == Position[1])));
        }

        public bool ScanForObstaclesInNextStep(double x, double y)
        {
            return (((DynObstPosition[0] == Position[0]+x) && (DynObstPosition[1] == Position[1]+y)) ||
                    ((StatObstPosition[0] == Position[0]+x) && (StatObstPosition[1] == Position[1]+y)));
        }

        /// <summary>
		///   Gets the value indicating, that the robot has the same position as its target
		/// </summary>
        public bool SamePositionAsTarg => ((TargetPosition[0] == Position[0]) && (TargetPosition[1] == Position[1])) ? true : false;

        /// <summary>
		///   The fault that prevents the robot from moving.
		/// </summary>
		public readonly Fault SuppressMoving = new PermanentFault();
        /// <summary>
		///   The fault that doesn't recognise an obstacle, thus causes the robot to collide with an obstacle.
		/// </summary>
        public readonly Fault SuppressStop = new PermanentFault();
        /// <summary>
        ///   The fault that doesn't recognise an obstacle, thus causes the robot to collide with an obstacle.
        /// </summary>
        public readonly Fault SuppressDetecting = new PermanentFault();


        /// <summary>
        ///   Moves the robot. Increases the direction by a maximum of one.
        /// </summary>
        public virtual void Move(double x, double y)
        {
            double PosX = GetXCoord();
            double PosY = GetYCoord();
            bool IncreaseX = false;
            bool WasInIfClause = false;

            if (x > 0 && PosX < 5 && !SamePositionAsObst)
            {
                Position[0]++;
                IncreaseX = true;
                ObstDetected = ScanForObstaclesInNextStep(1, 0);
                WasInIfClause = true;
            }
            if (y > 0 && Position[1] < 5 && !SamePositionAsObst) {
                Position[1]++;
                if (IncreaseX)
                    ObstDetected = ScanForObstaclesInNextStep(1, 1);
                else
                    ObstDetected = ScanForObstaclesInNextStep(0, 1);
                WasInIfClause = true;
            }
            if (!WasInIfClause || !IsDetecting)
                ObstDetected = false;
        }

        /// <summary>
        ///   Stops the robot.
        /// </summary>
        public void Stop()
        {
            IsMoving = false;
        }

        public override void Update()
        {
            if (Position[0] < 5)
                Position[0]++;
        }

        [FaultEffect(Fault = nameof(SuppressMoving))]
        public class SuppressMovingEffect : Robot
        {
            public override void Move(double x, double y) {

            }
        }

        [FaultEffect(Fault = nameof(SuppressStop))]
        public class SuppressStopEffect : Robot
        {
            public override void Move(double x, double y)
            {
                double PosX = GetXCoord();
                double PosY = GetYCoord();
                bool IncreaseX = false;

                if (x > 0 && PosX < 5)
                {
                    Position[0]++;
                    IncreaseX = true;
                    ObstDetected = ScanForObstaclesInNextStep(1, 0);
                }
                if (y > 0 && Position[1] < 5)
                {
                    Position[1]++;
                    if (IncreaseX)
                        ObstDetected = ScanForObstaclesInNextStep(1, 1);
                    else
                        ObstDetected = ScanForObstaclesInNextStep(0, 1);
                }
                ObstDetected = false;
                IsMoving = true;
            }
        }

        [FaultEffect(Fault = nameof(SuppressDetecting))]
        public class SuppressDetectingEffect : Robot
        {
            public override void Move(double x, double y)
            {
                ObstDetected = false;
            }
        }

        public double GetXCoord() {
            return Position.x;
        }

        public double GetYCoord()
        {
            return Position.y;
        }
    }
}