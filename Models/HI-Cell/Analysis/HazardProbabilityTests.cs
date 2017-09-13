using System;
using ISSE.SafetyChecking.Modeling;
using NUnit.Framework;
using SafetySharp.Analysis;
using SafetySharp.Modeling;
using SafetySharp.CaseStudies.HI_Cell.Modeling;

namespace SafetySharp.CaseStudies.HI_Cell.Analysis
{
    
    class HazardTests
    {
        [Test]
        public void CalculateHazardIsCollided()
        {
            var model = new Model();
            model.Sensor.SuppressDetecting.ProbabilityOfOccurrence = new Probability(0.0);

            var result = SafetySharpModelChecker.CalculateProbabilityRangeToReachStateBounded(model, model.Robot.IsCollided, 200);
            Console.Write($"Probability of hazard: {result}");
        }

    }
}
