﻿using UnityEngine;

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
        private Vector2 Position = new Vector2(0, 4);
        public bool IsMoving { get; private set; }
        public bool HasStopped => !IsMoving;
        public bool IsCollided => SamePositionAsObst;

        public bool SamePositionAsObst => IsSamePositionAsObst;
        public bool SamePositionAsTarg => IsSamePositionAsTarg;
        public bool ObstDetected => ObstacleDetected;

        public extern bool IsSamePositionAsObst { get; }
        public extern bool IsSamePositionAsTarg { get; }
        public extern bool ObstacleDetected { get; }
        public extern bool ObstacleInEnvironment { get; }
        /*public extern Vector2 CameraPosition { get; }*/
        public bool MonitorText = false;

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
            Constraints = new List<Func<bool>>()
            {
                () => ObstacleInEnvironment == !IsCollided,
                () => IsMoving == !IsCollided,
                () => IsMoving == !SamePositionAsTarg,
                () => IsCollided == SamePositionAsObst,
                () => IsSamePositionAsTarg ? HasStopped : !HasStopped
            };
            //Problem: the binding between the ports is done, AFTER this constructor was avoked...
            //Console.WriteLine(ObstacleInEnvironment + "\n");
        }

        /// <summary>
        ///   Moves the robot. Increases the direction by a maximum of one.
        /// </summary>
        public virtual void Move(bool moveX, bool moveY)
        {
            IsMoving = true;
            if (moveX && GetXCoord() < 5) /*&& !SamePositionAsObst && !ObstDetected && !HasStopped*/
            {
                Position.x++;
            }
            if (moveY && GetYCoord() < 5) /*&& !SamePositionAsObst && !ObstDetected && !HasStopped*/
            {
                Position.y++;
            }
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
            if (ObstDetected || ObstacleInEnvironment)
                IsMoving = false;
            else if (Position[0] < 5 && !SamePositionAsObst && !ObstDetected && !ObstacleInEnvironment && !HasStopped)
                Move(true, false);
            CheckConstraints();
        }

        [FaultEffect(Fault = nameof(SuppressMoving)), Priority(2)]
        public class SuppressMovingEffect : Robot
        {
            public override void Move(bool moveX, bool moveY) {

            }
        }

        [FaultEffect(Fault = nameof(SuppressStop)), Priority(1)]
        public class SuppressStopEffect : Robot
        {
            public override void Move(bool moveX, bool moveY)
            {

                if (moveX)     /* && posX < 5 && !ObstDetected && !SamePositionAsObst*/
                {
                    Position[0]++;
                }
                if (moveY)     /* && posY < 5 && !ObstDetected && !SamePositionAsObst*/
                {
                    Position[1]++;
                }
                IsMoving = true;
            }

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

        public Vector2 GetPosition() {
            return Position;
        }
    }

    public partial class Robot
    {
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