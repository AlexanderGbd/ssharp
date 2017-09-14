using UnityEngine;
namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using SafetySharp.Modeling;
    using System;

    public class DynamicObstacle : Component
    {
        public Vector2 Position = new Vector2(3, 2);
        public bool IsMoving { get; private set; }
        public bool HasStopped => !IsMoving;
        public extern bool IsDetected { get; }
        //When the robot has already stopped, because of detecting the obstacle, the obstacle shouldn't hit the still standing robot
        public extern Vector2 RobotPosition { get; }

        public float GetXCoord()
        {
            return Position.x;
        }

        public float GetYCoord()
        {
            return Position.y;
        }

        public Vector2 GetPosition() {
            return Position; 
        }

        /// <summary>
        /// Moves the obstacle dynamically, with a random function as a basis.
        /// The position increases by a maximum of one step in each direction.
        /// </summary>
        public void Move() {
            Random rnd = new Random();
            IsMoving = true;
            bool plusOperation = rnd.Next(2) == 1;

            switch (rnd.Next(0, 4))
            {
                case 0:
                    if ((plusOperation || (int)Position.x == 0) && (int)Position.x < 5 && !(IsDetected && (int)RobotPosition.x == (int)Position.x +1))
                        Position.x = (Position.x + 1) % 5;
                    else if (!(IsDetected && (int)RobotPosition.x == (int)Position.x - 1))
                        Position.x = (Position.x - 1) % 5;
                    break;

                case 1:
                    if ((plusOperation || (int)Position.y == 0) && (int)Position.y < 5 && !(IsDetected && (int)RobotPosition.y == (int)Position.y + 1))
                        Position.y = (Position.y + 1) % 5;
                    else if (!(IsDetected && (int)RobotPosition.y == (int)Position.y - 1))
                        Position.y = (Position.y - 1) % 5;
                    break;

                case 2:
                    if (plusOperation && (int)Position.x == 0 && (int)Position.x < 5 &&
                        !(IsDetected && (int)RobotPosition.x == (int)Position.x + 1 && (int)RobotPosition.y == (int)Position.y + 1))
                    {
                        Position.x = (Position.x + 1) % 5;
                        Position.y = (Position.y + 1) % 5;
                    }
                    else if (Position.x > 0 && Position.y > 0 &&
                        !(IsDetected && (int)RobotPosition.x == (int)Position.x - 1 && (int)RobotPosition.y == (int)Position.y - 1))
                    {
                        Position.x = (Position.x - 1) % 5;
                        Position.y = (Position.y - 1) % 5;
                    }
                    else
                    {
                        goto case  0;
                    }
                    break;

                case 3:
                    if (plusOperation && (int)Position.x < 5 && Position.y > 0 && (int)Position.y < 5 &&
                        !(IsDetected && (int)RobotPosition.x == (int)Position.x + 1 && (int)RobotPosition.y == (int)Position.y - 1))
                    {
                        Position.x = (Position.x + 1) % 5;
                        Position.y = (Position.y - 1) % 5;
                    }
                    else if (Position.x > 0 && !(IsDetected && (int)RobotPosition.x == (int)Position.x - 1 && (int)RobotPosition.y == (int)Position.y + 1))
                    {
                        Position.x = (Position.x - 1) % 5;
                        Position.y = (Position.y + 1) % 5;
                    }
                    else
                    {
                        goto case 0;
                    }
                    break;
            }
        }

        public void Stop()
        {
            IsMoving = false;
        }

        public override void Update()
        {
            if (IsMoving)
                Move();
        }
    }
}
