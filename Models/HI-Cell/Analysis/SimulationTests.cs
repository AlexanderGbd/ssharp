namespace SafetySharp.CaseStudies.HI_Cell.Analysis
{
    using FluentAssertions;
    using Modeling;
    using NUnit.Framework;
    using SafetySharp.Analysis;
    using SafetySharp.Modeling;
    class SimulationTests
    {
        /// <summary>
        ///   Simulates a path where no faults occur with the expectation that the robot reaches its target
        /// </summary>
        [Test]
        public void RobotReachesTargetWhenNoFaultsOccur() {
            var model = new Model();
            model.Faults.SuppressActivations();

            var simulator = new SafetySharpSimulator(model);
            model = (Model)simulator.Model;
            simulator.FastForward(steps: 120);

            model.Robot.SamePositionAsTarg.Should().BeTrue();
        }

        /// <summary>
        ///   Simulates a path where only the robot's 'is moving' fault occurs with the expectation that the robot doesn't move.
        /// </summary>
        [Test]
        public void RobotIsNotMovingWhenItShouldBe() {
            var model = new Model();
            model.Faults.SuppressActivations();
            model.Robot.SuppressMoving.ForceActivation();

            var simulator = new SafetySharpSimulator(model);
            model = (Model)simulator.Model;
            simulator.FastForward(steps: 120);

            model.Robot.IsMoving.Should().BeFalse();
        }

        /// <summary>
        ///   Simulates a path where only the robot's 'stop' fault occurs with the expectation that the robot does in fact not stop.
        /// </summary>
        [Test]
        public void RobotDoesntStopWhenItShould() {
            var model = new Model();
            model.Faults.SuppressActivations();
            model.Robot.SuppressStop.ForceActivation();

            var simulator = new SafetySharpSimulator(model);
            model = (Model)simulator.Model;
            simulator.FastForward(steps: 120);

            model.Robot.IsMoving.Should().BeTrue();
        }

        /// <summary>
        ///   Simulates a path where only the robot's 'detecting' fault occurs with the expectation that the robot does in fact not detect an obstacle.
        /// </summary>
        [Test]
        public void RobotDoesntDetectObstacleWhenItShouldDo() {
            var model = new Model();
            model.Faults.SuppressActivations();
            model.Sensor.SuppressDetecting.ForceActivation();

            var simulator = new SafetySharpSimulator(model);
            model = (Model)simulator.Model;
            simulator.FastForward(steps: 120);

            model.Sensor.ObstDetected.Should().BeFalse();
        }

        /// <summary>
        ///   Checks that the robot stops when it would hit an obstacle in the next step
        /// </summary>
        [Test]
        public void RobotStopsWhenItWouldHitAnObstacleInTheNextStep()
        {
            var model = new Model();
            model.Faults.SuppressActivations();

            var simulator = new SafetySharpSimulator(model);
            model = (Model)simulator.Model;
            simulator.FastForward(steps: 120);

            if (model.Sensor.ObstDetected)
                Assert.IsFalse(model.Robot.IsMoving);
        }

        /// <summary>
        /// The robot's camera does in fact not record when its 'recording' fault occurs
        /// </summary>
        [Test]
        public void CameraDoesntRecordWhenItShouldDo() {
            var model = new Model();
            model.Faults.SuppressActivations();
            model.Camera.SuppressRecording.ForceActivation();

            var simulator = new SafetySharpSimulator(model);
            model = (Model)simulator.Model;
            simulator.FastForward(steps: 120);

            model.Camera.IsRecording.Should().BeFalse();
        }
    }
}
