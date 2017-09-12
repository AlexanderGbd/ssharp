using UnityEngine;
namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using SafetySharp.Modeling;
    using System;

    public class DynamicObstacle : Component
    {
        private Vector2 Position = new Vector2(3, 3);
        public bool IsMoving { get; private set; }
        public bool HasStopped => !IsMoving;

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

            bool plusOperation = rnd.Next(2) == 1;

            switch (rnd.Next(0, 4))
            {
                case 0:
                    if (plusOperation)
                        Position.x = (Position.x + 1) % 5;
                    else
                        Position.x = (Position.x - 1) % 5;
                    break;

                case 1:
                    if (plusOperation)
                        Position.y = (Position.y + 1) % 5;
                    else
                        Position.y = (Position.y - 1) % 5;
                    break;

                case 2:
                    if (plusOperation)
                    {
                        Position.x = (Position.x + 1) % 5;
                        Position.y = (Position.y + 1) % 5;
                    }
                    else
                    {
                        Position.x = (Position.x - 1) % 5;
                        Position.y = (Position.y - 1) % 5;
                    }
                    break;

                case 3:
                    if (plusOperation)
                    {
                        Position.x = (Position.x + 1) % 5;
                        Position.y = (Position.y - 1) % 5;
                    }
                    else
                    {
                        Position.x = (Position.x - 1) % 5;
                        Position.y = (Position.y + 1) % 5;
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
            if (!HasStopped)
                Move();
        }
    }
}
