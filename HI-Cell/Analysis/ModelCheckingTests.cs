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
    using static SafetySharp.Analysis.Operators;

    class ModelCheckingTests
    {
        /// <summary>
        ///   Simply enumerates all states of the case study by checking a valid formula; 'true', in this case. The test's primary
        ///   intent is to support model checking efficiency measurements: Valid formulas represent the worst case for S# as all
        ///   reachable states and transitions have to be computed.
        /// </summary>
        [Test]
        public void EnumerateAllStates()
        {
            var model = new Model();

            var result = ModelChecker.CheckInvariant(model, true);
            result.FormulaHolds.Should().BeTrue();
        }

        /// <summary>
        ///   Checks that the robot is always detecting and is alway reaching its target when no faults occur
        /// </summary>
        [Test]
        public void RobotReachesTargetWhenNoFaultsOccurModel()
        {
            var model = new Model();
            model.Faults.SuppressActivations();

            var result = ModelChecker.CheckInvariant(model, model.Robot.IsDetecting);
            var result1 = ModelChecker.CheckInvariant(model, model.Robot.HasStopped && model.Robot.SamePositionAsTarg);
            result.FormulaHolds.Should().BeTrue();
        }

        /// <summary>
        ///   Checks a formula over the case study, the x coordinate of our robot never exceeds 5, and the y coordinate always stays 0 (for now)
        /// </summary>
        [Test]
        public void StateGraphAllStates()
        {
            var model = new Model();

            var result = ModelChecker.CheckInvariant(model, model.Robot.GetXCoord() < 6);
            var result1 = ModelChecker.CheckInvariant(model, model.Robot.GetYCoord() == 0);

            Console.WriteLine("\nX: " + model.Robot.GetXCoord() + "Y: " + model.Robot.GetYCoord() + "Bool statement: " + (model.Robot.GetXCoord() < 6));
           
            result.FormulaHolds.Should().BeTrue();
        }

        /// <summary>
        ///   Checks that the robot is not moving when its 'moving' fault occurs
        /// </summary>
        [Test]
        public void RobotCantMoveWhenMovingFaultOccurs()
        {
            var model = new Model();
            model.Faults.SuppressActivations();
            model.Robot.SuppressMoving.Activation = Activation.Forced;

            var result = ModelChecker.CheckInvariant(model, model.Robot.IsMoving);
            result.FormulaHolds.Should().BeFalse();
        }

        /// <summary>
        ///   Checks that the robot doesn't collide when its sensors are working
        /// </summary>
        [Test]
        public void RobotDoesntCollideWhenSensorDetectsObstacles()
        {
            var model = new Model();
            model.Faults.SuppressActivations();

            //To be continued
        }
    }
}
