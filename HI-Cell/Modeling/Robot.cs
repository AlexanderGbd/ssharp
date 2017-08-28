using UnityEngine;

namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using SafetySharp.Modeling;
    using System;
    

    public class Robot : Component
    {

        private Vector2 Position = new Vector2(0, 0);
        public bool IsMoving { get; private set; }
        public bool HasStopped => !IsMoving;
        public bool IsCollided => SamePositionAsObst;

        public bool SamePositionAsObst => IsSamePositionAsObst;
        public bool SamePositionAsTarg => IsSamePositionAsTarg;
        public bool ObstDetected => ObstacleDetected;

        public extern bool IsSamePositionAsObst { get; }
        public extern bool IsSamePositionAsTarg { get; }
        public extern bool ObstacleDetected { get; }

        /// <summary>
		///   The fault that prevents the robot from moving.
		/// </summary>
		public readonly Fault SuppressMoving = new PermanentFault();
        /// <summary>
		///   The fault that doesn't recognise an obstacle, thus causes the robot to collide with an obstacle.
		/// </summary>
        public readonly Fault SuppressStop = new PermanentFault();

        /// <summary>
        ///   Moves the robot. Increases the direction by a maximum of one.
        /// </summary>
        public virtual void Move(double x, double y)
        {
            double PosX = GetXCoord();
            double PosY = GetYCoord();

            if (x > 0 && GetXCoord() < 5 && !SamePositionAsObst && ObstDetected)
            {
                Position.x++;
            }
            if (y > 0 && GetYCoord() < 5 && !SamePositionAsObst && ObstDetected) {
                Position.y++;
            }
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
            if (ObstDetected)
                IsMoving = false;
            else if (Position[0] < 5)
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

                if (x > 0 && PosX < 5 && !ObstDetected && !SamePositionAsObst)
                {
                    Position[0]++;
                }
                if (y > 0 && Position[1] < 5 && !ObstDetected && !SamePositionAsObst)
                {
                    Position[1]++;
                }
                IsMoving = true;
            }
        }

        public double GetXCoord() {
            return Position.x;
        }

        public double GetYCoord()
        {
            return Position.y;
        }

        public Vector2 GetPosition() {
            return Position;
        }
    }
}