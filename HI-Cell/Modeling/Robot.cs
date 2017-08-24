namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using SafetySharp.Modeling;
    using System;
    public class Robot : Component
    {

        private double[] Position = new double[] {0, 0};
        public bool IsMoving { get; private set; }

        /// <summary>
        ///   The positions of the obstacles and the target
        /// </summary>
        private double[] StatObstPosition => new double[] { StatObstaclePosition[0], StatObstaclePosition[1] };
        private double[] DynObstPosition => new double[] { DynObstaclePosition[0], DynObstaclePosition[1] };
        private double[] TargetPosition => new double[] {Model.XTarget, Model.YTarget};

        /// <summary>
        ///   Gets the position of the dynamic obstacle
        /// </summary>
        public extern double[] DynObstaclePosition { get; }

        /// <summary>
        ///   Gets the position of the static obstacle
        /// </summary>
        public extern double[] StatObstaclePosition { get; }

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
        public readonly Fault SuppressObstacle = new PermanentFault();

        /// <summary>
        ///   Moves the robot. Increases the direction by a maximum of one.
        /// </summary>
        public virtual void Move(double x, double y)
        {
            if (x > 0 && Position[0] < 5)
                Position[0]++;
            if (y >0 && Position[1] < 5)
                Position[1]++;
            IsMoving = true;
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

        [FaultEffect(Fault = nameof(SuppressObstacle))]
        public class SuppressObstacleEffect : Robot
        {
            public override void Move(double x, double y)
            {
                Position[0] += 1;
            }
        }

        public double GetXCoord() {
            return Position[0];
        }

        public double GetYCoord()
        {
            return Position[1];
        }
    }
}