
namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using SafetySharp.Modeling;
    public class Robot : Component
    {
        //private int XCoord;
        //private int YCoord;
        private int[] Position = new int[2];
        public bool IsMoving { get; private set; }

        /// <summary>
		///   The fault that prevents the robot from moving.
		/// </summary>
		public readonly Fault SuppressMoving = new PermanentFault();
        /// <summary>
		///   The fault that causes the robot to collide with an obstacle.
		/// </summary>
        public readonly Fault SuppressIsObstacle = new TransientFault();

        /// <summary>
        ///   Moves the robot.
        /// </summary>
        public virtual void Move()
        {
            IsMoving = true;
        }

        /// <summary>
        ///   Stops the robot.
        /// </summary>
        public void Stop()
        {
            IsMoving = false;
        }

        [FaultEffect(Fault = nameof(SuppressMoving))]
        public class SuppressMovingEffect : Robot
        {
            public override void Move() {

            }
        }

        public int GetXCoord() {
            return Position[0];
        }

        public int GetYCoord()
        {
            return Position[1];
        }
    }
}
