using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafetySharp.CaseStudies.HI_Cell.Analysis
{
    using Modeling;
    using NUnit.Framework;
    using SafetySharp.Analysis;
    using SafetySharp.Modeling;
    class HazardTests
    {

        [Test]
        public void CalculateHazardCollision() {
            var model = new Model();
            
        }

        //Example

        //[Test]
        //public void CalculateHazardIsRuptured()
        //{
        //    var model = new Model();
        //    model.Pump.SuppressPumping.ProbabilityOfOccurrence = new Probability(0.0);
        //    model.Sensor.SuppressIsFull.ProbabilityOfOccurrence = new Probability(0.0001);
        //    model.Sensor.SuppressIsEmpty.ProbabilityOfOccurrence = new Probability(0.0);
        //    model.Timer.SuppressTimeout.ProbabilityOfOccurrence = new Probability(0.0001);

        //    var result = SafetySharpModelChecker.CalculateProbabilityRangeToReachStateBounded(model, model.Tank.IsRuptured, 200);
        //    Console.Write($"Probability of hazard: {result}");
        //}

    }
}
