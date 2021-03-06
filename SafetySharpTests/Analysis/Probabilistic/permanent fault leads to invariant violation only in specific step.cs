﻿// The MIT License (MIT)
// 
// Copyright (c) 2014-2017, Institute for Software & Systems Engineering
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace Tests.Analysis.Probabilistic
{
	using System;
	using SafetySharp.Analysis;
	using SafetySharp.ModelChecking;
	using ISSE.SafetyChecking.DiscreteTimeMarkovChain;
	using ISSE.SafetyChecking.ExecutedModel;
	using ISSE.SafetyChecking.Formula;
	using ISSE.SafetyChecking.Modeling;
	using SafetySharp.Modeling;
	using Shouldly;
	using Utilities;
	
	internal class PermanentFaultLeadsToInvariantViolationOnlyInSpecificStep : ProbabilisticAnalysisTestObject
	{
		protected override void Check()
		{
			var c = new C();
			Probability probabilityOfInvariantViolation;
			
			Formula invariantViolated = c.ViolateInvariant;
			var finallyInvariantViolated = new UnaryFormula(invariantViolated, UnaryOperator.Finally);

			var markovChainGenerator = new SafetySharpMarkovChainFromExecutableModelGenerator(TestModel.InitializeModel(c));
			markovChainGenerator.Configuration.ModelCapacity = ModelCapacityByMemorySize.Small;
			markovChainGenerator.AddFormulaToCheck(finallyInvariantViolated);
			var dtmc = markovChainGenerator.GenerateMarkovChain();
			var typeOfModelChecker = (Type)Arguments[0];
			var modelChecker = (DtmcModelChecker)Activator.CreateInstance(typeOfModelChecker, dtmc, Output.TextWriterAdapter());
			using (modelChecker)
			{
				probabilityOfInvariantViolation = modelChecker.CalculateProbability(finallyInvariantViolated);
			}

			// 1.0-(1.0-0.1)^11 = 0.68618940391
			probabilityOfInvariantViolation.Is(0.68618940391, 0.00001).ShouldBe(true);
		}

		private class C : Component
		{
			public readonly Fault F1 = new PermanentFault
			{
				ProbabilityOfOccurrence = new Probability(0.1)
			};

			[Range(0, 11, OverflowBehavior.Clamp)]
			private int _timestep;

			public bool ViolateInvariant;

			protected virtual void CriticalStep()
			{
				ViolateInvariant = false;
			}

			public override void Update()
			{
				_timestep++;
				if (_timestep == 10)
					CriticalStep();
			}


			[FaultEffect(Fault = nameof(F1)), Priority(1)]
			public class E1 : C
			{
				protected override void CriticalStep()
				{
					base.ViolateInvariant = true;
				}
			}
		}
	}

}