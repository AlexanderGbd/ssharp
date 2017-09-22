using UnityEngine;

namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ISSE.SafetyChecking.Modeling;
    using SafetySharp.Modeling;

    public partial class Robot : Component
    {

        //private Vector2 Position => CameraPosition;
        private Vector3 Position = new Vector3(0, 0, 0);
        public bool IsMoving { get; private set; }
        public bool HasStopped => !IsMoving;
        public bool IsCollided => SamePositionAsObst;
        public bool PassedTarget = false;

        public bool SamePositionAsObst => IsSamePositionAsObst;
        public bool SamePositionAsTarg => IsSamePositionAsTarg;
        //public bool ObstDetected => ObstacleDetected;

        public extern bool IsSamePositionAsObst { get; }
        public extern bool IsSamePositionAsTarg { get; }
        //public extern bool ObstacleDetected { get; }
        public extern bool ObstacleInEnvironment { get; }
        public extern bool DynamicObstInEnvironment { get; }
        public extern bool StaticObstInEnvironment { get; }

        public extern float XCalculated { get; }
        public extern float YCalculated { get; }

        /*public extern Vector2 CameraPosition { get; }*/
        public bool MonitorText;

        public List<Func<bool>> Constraints;

        /// <summary>
		///   The fault that prevents the robot from moving.
		/// </summary>
        public readonly Fault SuppressMoving = new PermanentFault();
        /// <summary>
		///   The fault that doesn't recognise an obstacle, thus causes the robot to collide with an obstacle.
		/// </summary>
        public readonly Fault SuppressStop = new PermanentFault();


        public Robot()
        {
            SetConstraints();

            //Problem at the beginning: the binding between the ports is done, AFTER this constructor was avoked...
            //Console.WriteLine(ObstacleInEnvironment + "\n");
        }



        /// <summary>
        ///   Moves the robot when the stop fault occurs. Increases the direction by a maximum of one.
        /// </summary>
        public virtual void MoveOnStopFault(bool moveX, bool moveY)
        {
            IsMoving = true;

            if (moveX) 
            {
                Position.x++;
            }
            if (moveY)
            {
                Position.y++;
            }

            if (IsSamePositionAsTarg)
                Stop();
        }

        /// <summary>
        /// Moves the robot dynamically. Increases the directions by a maximum of one
        /// </summary>
        public virtual void Move(int x, int y)
        {
            IsMoving = true;

            if (x == 1)
                Position.x++;
            else if (x == -1)
                Position.x--;
            if (y == 1)
                Position.y++;
            else if (y == -1)
                Position.y--;

            if (IsSamePositionAsTarg)
                Stop();
        }

        /// <summary>
        ///   Stops the robot.
        /// </summary>
        public virtual void Stop()
        {
            IsMoving = false;
        }

        public override void Update()
        {
            if (ObstacleInEnvironment)
                IsMoving = false;
            else if (!SamePositionAsObst && !ObstacleInEnvironment && IsMoving)
            {
                if (!(IsMoving && SamePositionAsTarg) && !PassedTarget)
                    Move((int)XCalculated, (int)YCalculated);
                else
                {
                    MoveOnStopFault(true, false);
                    PassedTarget = true;
                }
            }
            CheckConstraints();                             
        }

        [FaultEffect(Fault = nameof(SuppressMoving)), Priority(2)]
        public class SuppressMovingEffect : Robot
        {
            public override void Move(int x, int y) {

            }

        }

        [FaultEffect(Fault = nameof(SuppressStop)), Priority(1)]
        public class SuppressStopEffect : Robot
        {
        public override void Stop()
            {
                IsMoving = true;
            }
        }

        public float GetXCoord() {
            return Position.x;
        }

        public float GetYCoord()
        {
            return Position.y;
        }

        public Vector3 GetPosition() {
            return Position;
        }
    }

    public partial class Robot
    {
        private void SetConstraints()
        {
            Constraints = new List<Func<bool>>()
            {
                () => !ObstacleInEnvironment || !IsCollided,
                () => !IsMoving || !IsCollided,
                () => !IsMoving || !SamePositionAsTarg || !ObstacleInEnvironment,         
                () => !IsCollided || SamePositionAsObst,
                () => !IsSamePositionAsTarg || HasStopped           
            };
        }

        public bool ValidateConstraints()
        {
            return Constraints.All(constraint => constraint());
        }

        public void CheckConstraints()
        {
            if (!ValidateConstraints())
            {
                //throw new Exception();
                MonitorText = true;
            }
            else
            {
                MonitorText = false;
            }
        }
    }
}