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
        [Test]
        public void RobotReachesTargetWhenNoFaultsOccur() {
            var model = new Model();
            model.Faults.SuppressActivations();

            var simulator = new Simulator(model);
            model = (Model)simulator.Model;
            simulator.FastForward(steps: 5);

            model.Robot.SamePositionAsTarg.Should().BeTrue();
        }

        //Example:
        //[Test]
        //public void TankDoesNotRuptureWhenNoFaultsOccur()
        //{
        //    var model = new Model();
        //    model.Faults.SuppressActivations();

        //    var simulator = new SafetySharpSimulator(model);
        //    model = (Model)simulator.Model;
        //    simulator.FastForward(steps: 120);

        //    model.Tank.IsRuptured.Should().BeFalse();
        //}

    }
}
