namespace SafetySharp.CaseStudies.HI_Cell.Analysis
{
    using System;
    using System.Threading;
    using FluentAssertions;
    using Modeling;
    using NUnit.Framework;
    using SafetySharp.Analysis;
    using SafetySharp.Modeling;
    class SimulationTests
    {
        [Test]
        public void FirstAPITest()
        {
            Client client = Client.getInstance;
            client.MoveDirectlyTo(0.4, 0.4, 0.4, Math.PI, 0, -Math.PI);
            while (Math.Abs(Sensor.getInstance.APIPosition.x - 4) > 0.00001 && Math.Abs(Sensor.getInstance.APIPosition.y - 4) > 0.00001 /*!Sensor.getInstance.SamePositionAsTarg*/)
            {
                //Console.WriteLine("Client Current Position: " + client.CurrentPosition/* + "    "+ client.CurrentPosition.x*/);
                // Console.WriteLine("Sensor API-Position: "+sensor.APIPosition);
            }
        }

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
        public void RobotDoesntStopWhenItShould()
        {
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

            model.Sensor.ObstInEnvironment.Should().BeFalse();
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

            //if (model.Sensor.ObstDetected)
            //    Assert.IsFalse(model.Robot.IsMoving);
            if (model.Sensor.ObstInEnvironment)
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
