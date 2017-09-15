namespace SafetySharp.CaseStudies.HI_Cell.Analysis
{
    using System;
    using FluentAssertions;
    using Modeling;
    using NUnit.Framework;
    using ISSE.SafetyChecking.Formula;
    using ISSE.SafetyChecking.Modeling;
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

            var result = SafetySharpModelChecker.CheckInvariant(model, true);
            result.FormulaHolds.Should().BeTrue();
        }

        /// <summary>
        ///   Checks that the robot is always detecting and is always reaching its target when no faults occur
        /// </summary>
        [Test]
        public void RobotReachesTargetWhenNoFaultsOccurModel()
        {
            var model = new Model();
            model.Faults.SuppressActivations();

            SafetySharpInvariantAnalysisResult result;
            if (!model.Sensor.SuppressDetecting.IsActivated &&
                !model.Camera.SuppressRecording.IsActivated &&
                !model.Robot.SuppressMoving.IsActivated &&
                !model.Robot.SuppressStop.IsActivated)
            {
                result = SafetySharpModelChecker.CheckInvariant(model, !model.Robot.IsCollided);
            }
            else
            {
                Console.WriteLine("Shouldn't be here!!!");
                result = new SafetySharpInvariantAnalysisResult();
            }
            //var result = ModelChecker.CheckInvariant(model, G(noFaults).Implies(!F(model.Robot.IsCollided)));
            result.FormulaHolds.Should().BeTrue();
        }

        ///// <summary>
        /////   Checks a formula over the case study, the x coordinate of our robot (for now) never exceeds 5, and the y coordinate always stays 0 
        ///// </summary>
        //[Test]
        //public void StateGraphAllStates()
        //{
        //    var model = new Model();

        //    var result = SafetySharpModelChecker.CheckInvariant(model, model.Robot.GetXCoord() < 6);
        //    //var result1 = SafetySharpModelChecker.CheckInvariant(model, model.Robot.GetYCoord() == 0);
                       
        //    result.FormulaHolds.Should().BeTrue();
        //}

        /// <summary>
        ///   Checks that the robot is not moving when its 'moving' fault occurs
        /// </summary>
        [Test]
        public void RobotCantMoveWhenMovingFaultOccurs()
        {
            var model = new Model();
            model.Faults.SuppressActivations();
            model.Robot.SuppressMoving.Activation = Activation.Forced;

            var result = SafetySharpModelChecker.CheckInvariant(model, model.Robot.IsMoving);
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

            var result = SafetySharpModelChecker.CheckInvariant(model, model.Robot.IsCollided);
            result.FormulaHolds.Should().BeFalse();
        }

        /// <summary>
        ///   Checks that the robot can collide when its 'detecting' fault occurs
        /// </summary>
        [Test]
        public void RobotCollidesWhenSensorDoesntDetectObstacles()
        {
            var model = new Model();
            model.Faults.SuppressActivations();
            model.Sensor.SuppressDetecting.Activation = Activation.Forced;

            var result = SafetySharpModelChecker.CheckInvariant(model, model.Robot.IsCollided && !model.Robot.ObstDetected);
            result.FormulaHolds.Should().BeFalse();
        }
    }
}
