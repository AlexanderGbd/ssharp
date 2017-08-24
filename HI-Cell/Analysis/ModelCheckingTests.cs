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
            //result.FormulaHolds.Should().BeTrue();
        }
    }
}
