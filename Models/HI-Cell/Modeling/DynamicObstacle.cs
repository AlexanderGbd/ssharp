using UnityEngine;
namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using SafetySharp.Modeling;
    using System;

    public class DynamicObstacle : Component
    {
        public Vector3 Position = new Vector3(3, 2, 0);
        public bool IsMoving { get; private set; }
        public bool HasStopped => !IsMoving;
        public extern bool IsDetected { get; }
        public extern Vector3 RobotPosition { get; }

        public float GetXCoord()
        {
            return Position.x;
        }

        public float GetYCoord()
        {
            return Position.y;
        }

        public Vector3 GetPosition() {
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

            while (true)
            {
                int xDelta = rnd.Next(-1, 2);
                int yDelta = rnd.Next(-1, 2);

                if (TryMoveTo(xDelta, yDelta))
                    break;
            }
        }

        /// <summary>
        /// Try moving to the next position without hitting the robot, which stands still, after having detected an obstacle
        /// </summary>
        public bool TryMoveTo(int xDelta, int yDelta)
        {
            int xTarget = (int)Position.x + xDelta;
            int yTarget = (int)Position.y + yDelta;

            if (xTarget > 4 || xTarget < 0 || yTarget > 4 || yTarget < 0)
                return false;
            if (xTarget == (int)RobotPosition.x && yTarget == (int)RobotPosition.y)
                return false;
            
            Position.x = xTarget;
            Position.y = yTarget;
            return true;
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
