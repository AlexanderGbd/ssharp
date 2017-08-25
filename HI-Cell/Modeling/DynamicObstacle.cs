using UnityEngine;
namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using SafetySharp.Modeling;
    using System;

    public class DynamicObstacle : Component
    {
        private Vector2 Position = new Vector2(3, 1);
        public bool IsMoving { get; private set; }
        public bool HasStopped => !IsMoving;

        public double GetXCoord()
        {
            return Position.x;
        }

        public double GetYCoord()
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
            if (rnd.Next(0, 2) != 0)
                Position.x = (Position.x + 1) % 6;
            if (rnd.Next(0, 2) != 0)
                Position.y = (Position.y + 1) % 6;
        }

        public void Stop()
        {
            IsMoving = false;
        }

        public override void Update()
        {
            Move();
        }
    }
}
