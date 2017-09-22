using System;
using UnityEngine;

namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using System.Threading;
    using ISSE.SafetyChecking.Modeling;
    using SafetySharp.Modeling;

    public class Sensor : Component
    {
        private static Sensor instance;

        private Sensor()
        {
            //Thread printing = new Thread(Printing);
            //printing.Start();
        }

        public void Printing()
        {
            while (true)
            {
                Console.WriteLine(APIPosition);
            }
        }

        public static Sensor getInstance
        {
            get
            {
                if (instance == null)
                    instance = new Sensor();
                return instance;
            }
        }

        /// <summary>
        ///   The fault that doesn't recognise an obstacle, thus causes the robot to collide with an obstacle.
        /// </summary>
        public readonly Fault SuppressDetecting = new PermanentFault();

        public Vector3 APIPosition => Client.getInstance.CurrentPosition;

        Vector3 RobPosition => RobotPosition;
        private Vector3 StatObstPosition => StatObstaclePosition;
        private Vector3 DynObstPosition => DynObstaclePosition;
        private Vector3 TargetPosition => new Vector3(Model.XTarget, Model.YTarget, 0);

        //public bool ObstDetected { get; private set; }
        public bool IsDetecting { get; private set; } = true;
        public bool ObstInEnvironment { get; private set; }
        public bool DynamicObstInEnvironment { get; private set; }
        public bool StaticObstInEnvironment { get; private set; }

        /// <summary>
        ///   Gets the robot's position
        /// </summary>
        public extern Vector3 RobotPosition { get; }

        /// <summary>
        ///   Gets the position of the dynamic obstacle
        /// </summary>
        public extern Vector3 DynObstaclePosition { get; }

        /// <summary>
        ///   Gets the position of the static obstacle
        /// </summary>
        public extern Vector3 StatObstaclePosition { get; }

        /// <summary>
        ///   Looks if the robot is at the same position as a obstacle
        /// </summary>
        public bool ComparePositions()
        {
            return (int) DynObstPosition.x == (int) RobPosition.x && (int) DynObstPosition.y == (int) RobPosition.y ||
                    (int) StatObstPosition.x == (int) RobPosition.x && (int) StatObstPosition.y == (int) RobPosition.y;
        }

        public bool ScanForObstaclesInNextStep(int x, int y)
        {
            return ScanForDynamicObstacleInNextStep(x, y) || ScanForStaticObstacleInNextStep(x, y);
        }

        public bool ScanForDynamicObstacleInNextStep(int x, int y)
        {
            return (int)DynObstPosition.x == (int)(RobPosition.x + x) && (int)DynObstPosition.y == (int)(RobPosition.y + y);
        }
        public bool ScanForStaticObstacleInNextStep(int x, int y)
        {
            return (int)StatObstPosition.x == (int)(RobPosition.x + x) && (int)StatObstPosition.y == (int)(RobPosition.y + y);
        }

        /// <summary>
        /// Scans only the x-direction currently
        /// </summary>
        public bool OneStepScanIsOk()
        {
            return (int) DynObstPosition.x == (int) RobPosition.x + 1 || (int) StatObstPosition.x == (int) RobPosition.x + 1;
        }

        /// <summary>
		///   Gets the value indicating, that the robot has the same position as its target
		/// </summary>
        public bool SamePositionAsTarg => (int) TargetPosition.x == (int) RobPosition.x && (int) TargetPosition.y == (int) RobPosition.y;

        /// <summary>
        ///   Gets the distance between the robot and the dynamic obstacle
        /// </summary>
        public Vector3 DistanceToDynObstacle => new Vector3(Math.Abs(RobPosition.x-DynObstPosition.x), Math.Abs(RobPosition.y-DynObstPosition.y));
        /// <summary>
        ///   Gets the distance between the robot and the static obstacle
        /// </summary>
        public Vector3 DistanceToStatObstacle => new Vector3(Math.Abs(RobPosition.x - StatObstPosition.x), Math.Abs(RobPosition.y - StatObstPosition.y));

        /// <summary>
        ///   Calculates if the robot is at the same position as an obstacle
        /// </summary>
        public bool IsSamePositionAsObst() {
            return ComparePositions();
        }

        public bool IsSamePositionAsTarg() {
            return (int) TargetPosition.x == (int) RobPosition.x && (int) TargetPosition[1] == (int) RobPosition.y;
        }

        [FaultEffect(Fault = nameof(SuppressDetecting))]
        public class SuppressDetectingEffect : Sensor
        {
            public override void Update()
            {
                ObstInEnvironment = false;
                IsDetecting = false;
            }
        }

        /// <summary>
        /// Updates the sensor's state
        /// </summary>
        public override void Update()
        {
            DynamicObstInEnvironment = DynamicObstacleInEnvironment();
            StaticObstInEnvironment = StaticObstacleInEnvironment();
            ObstInEnvironment = DynamicObstInEnvironment || StaticObstInEnvironment;
        }

        public bool DynamicObstacleInEnvironment()
        {
            bool detected = false;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (ScanForDynamicObstacleInNextStep(i, j))
                        detected = true;
                }
            }
            return detected;
        }

        public bool StaticObstacleInEnvironment()
        {
            bool detected = false;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (ScanForStaticObstacleInNextStep(i, j))
                        detected = true;
                }
            }
            return detected;
        }
    }
}
