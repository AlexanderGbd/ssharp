using NUnit.Framework;
using SafetySharp.Analysis;
using SafetySharp.CaseStudies.HI_Cell.Modeling;
using SafetySharp.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            var simulator = new Simulator(model);
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

            var simulator = new Simulator(model);
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

            var simulator = new Simulator(model);
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
            model.Robot.SuppressDetecting.ForceActivation();

            var simulator = new Simulator(model);
            model = (Model)simulator.Model;
            simulator.FastForward(steps: 120);

            model.Robot.ObstDetected.Should().BeFalse();
        }

    }
}
