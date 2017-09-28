using System;
using UnityEngine;

namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using ISSE.SafetyChecking.Modeling;
    using SafetySharp.Modeling;

    public class Sensor : Component
    {
        private static Sensor instance;

        private Sensor() { }

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

        public Vector3 RobPosition => APIPosition /*RobotPosition*/;
        private Vector3 StatObstPosition => StatObstaclePosition;
        private Vector3 DynObstPosition => DynObstaclePosition;
        private Vector3 TargetPosition => new Vector3(Model.XTarget, Model.YTarget, 0);

        public bool IsDetecting { get; private set; } = true;
        public bool ObstacleDetectedDuringMovement { get; private set; }
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

        ///<summary>
        /// Scan - methods for 3d space
        /// </summary
        public bool ScanForObstaclesInNextStep(int x, int y, int z)
        {
            return ScanForDynamicObstacleInNextStep(x, y, z) || ScanForStaticObstacleInNextStep(x, y, z);
        }

        public bool ScanForDynamicObstacleInNextStep(int x, int y, int z)
        {
            return (int)DynObstPosition.x == (int)(RobPosition.x + x) && (int)DynObstPosition.y == (int)(RobPosition.y + y) && (int)DynObstPosition.z == (int)(RobPosition.z + z);
        }
        public bool ScanForStaticObstacleInNextStep(int x, int y, int z)
        {
            return (int)StatObstPosition.x == (int)(RobPosition.x + x) && (int)StatObstPosition.y == (int)(RobPosition.y + y) && (int)StatObstPosition.z == (int)(RobPosition.z + z);
        }


        /// <summary>
        /// Scans only the x-direction currently
        /// </summary>
        public bool OneStepScanIsOk()
        {
            return (int) DynObstPosition.x == (int) RobPosition.x + 1 || (int) StatObstPosition.x == (int) RobPosition.x + 1;
        }

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
            //return (int) TargetPosition.x == (int) RobPosition.x && (int) TargetPosition[1] == (int) RobPosition.y;
            return Client.getInstance.SamePositionAsTarget;
        }

        [FaultEffect(Fault = nameof(SuppressDetecting))]
        public class SuppressDetectingEffect : Sensor
        {
            public override void Update()
            {
                ObstInEnvironment = false;
                ObstacleDetectedDuringMovement = false;
                IsDetecting = false;
            }
        }

        /// <summary>
        /// Updates the sensor's state
        /// </summary>
        public override void Update()
        {
            ObstacleDetectedDuringMovement = Client.getInstance.ObstacleDetectedDuringMovement;
            DynamicObstInEnvironment = DynamicObstacleInEnvironment();
            StaticObstInEnvironment = StaticObstacleInEnvironment();
            ObstInEnvironment = DynamicObstInEnvironment || StaticObstInEnvironment;
        }

        public bool DynamicObstacleInEnvironment()
        {
            bool detected = false;
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (ScanForDynamicObstacleInNextStep(x, y))
                        detected = true;
                }
            }
            return detected;
        }

        public bool StaticObstacleInEnvironment()
        {
            bool detected = false;
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (ScanForStaticObstacleInNextStep(x, y))
                        detected = true;

                    //3-dimensional:
                    //for (int z = 0; z < 2; z++)
                    //{
                    //    if (ScanForStaticObstacleInNextStep(x, y, z))
                    //        detected = true;
                    //}
                }
            }
            return detected;
        }
    }
}
