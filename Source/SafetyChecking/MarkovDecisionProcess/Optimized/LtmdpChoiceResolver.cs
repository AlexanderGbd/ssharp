// The MIT License (MIT)
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

namespace ISSE.SafetyChecking.MarkovDecisionProcess.Optimized
{
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using AnalysisModelTraverser;
	using ExecutableModel;
	using Modeling;
	using Utilities;
	using System;
	using System.Diagnostics;

	// https://github.com/isse-augsburg/ssharp/commit/06455a592d2e16e873c6c7232f94015444a739a3

	/// <summary>
	///   Represents a stack that is used to resolve nondeterministic choices during state space enumeration.
	/// </summary>
	internal sealed class LtmdpChoiceResolver : ChoiceResolver
	{
		/// <summary>
		///   The number of nondeterministic choices that can be stored initially.
		/// </summary>
		private const int InitialCapacity = 64;

		/// <summary>
		///   The stack that indicates the chosen values for the current path.
		/// </summary>
		private readonly LtmdpChosenValueStack _chosenValues = new LtmdpChosenValueStack(InitialCapacity);

		/// <summary>
		///   The stack that stores the number of possible values of all encountered choices along the current path.
		/// </summary>
		private readonly ChoiceStack _valueCount = new ChoiceStack(InitialCapacity);

		/// <summary>
		///   The number of choices that have been encountered for the current path.
		/// </summary>
		private int _choiceIndex = -1;

		/// <summary>
		///   The current continuation id.
		/// </summary>
		private int _continuationId = 0;

		/// <summary>
		///   The next free continuation id.
		/// </summary>
		private int _nextFreeContinuationId = 1;

		/// <summary>
		///   Indicates whether the next path is the first one of the current state.
		/// </summary>
		private bool _firstPath;

		/// <summary>
		///   Contains the graph of the taken decisions.
		/// </summary>
		private LtmdpStepGraph LtmdpStepGraph { get; }

		/// <summary>
		///   Is ForwardOptimization enabled.
		/// </summary>
		private readonly bool _useForwardOptimization;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="useForwardOptimization">Use Forward Optimization.</param>
		public LtmdpChoiceResolver(LtmdpStepGraph ltmdpStepGraph, bool useForwardOptimization)
				: base(useForwardOptimization)
		{
			LtmdpStepGraph = ltmdpStepGraph;
			_useForwardOptimization = useForwardOptimization;
		}

		/// <summary>
		///   Gets the index of the last choice that has been made.
		/// </summary>
		// ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
		internal override int LastChoiceIndex => _choiceIndex;

		/// <summary>
		///   Prepares the resolver for resolving the choices of the next state.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void PrepareNextState()
		{
			_firstPath = true;
			_choiceIndex = -1;
			_continuationId = 0;
			_nextFreeContinuationId = 1;
		}

		private Probability GetProbabilityOfPreviousPath()
		{
			if (_choiceIndex == -1 || _choiceIndex == 0)
				return Probability.One;
			return _chosenValues[_choiceIndex - 1].Probability;
		}

		private Probability GetProbabilityUntilIndex(int index)
		{
			if (index == -1)
				return Probability.One;
			return _chosenValues[index].Probability;
		}


		/// <summary>
		///   Prepares the resolver for the next path. Returns <c>true</c> to indicate that all paths have been enumerated.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool PrepareNextPath()
		{
			if (_choiceIndex != _valueCount.Count - 1)
				throw new NondeterminismException();

			// Reset the choice counter as each path starts from the beginning
			_choiceIndex = -1;

			// If this is the first path of the state, we definitely have to enumerate it
			if (_firstPath)
			{
				_firstPath = false;
				return true;
			}

			// Let's go through the entire stack to determine what we have to do next
			while (_chosenValues.Count > 0)
			{
				// Remove the value we've chosen last -- we've already chosen it, so we're done with it
				var chosenValue = _chosenValues.Remove();

				// If we have at least one other value to choose, let's do that next
				if (_valueCount.Peek() > chosenValue.OptionIndex + 1)
				{
					var previousProbability = GetProbabilityUntilIndex(_valueCount.Count - 2);

					_continuationId = chosenValue.ContinuationId + 1;
					
					var newChosenValue =
						new LtmdpChosenValue
						{
							OptionIndex = chosenValue.OptionIndex + 1,
							ContinuationId = _continuationId,
							Probability = previousProbability
						};
					_chosenValues.Push(newChosenValue);
					return true;
				}

				// Otherwise, we've chosen all values of the last choice, so we're done with it
				_valueCount.Remove();
			}

			AssertThatDatastructureIsIntact();

			// If we reach this point, we know that we've chosen all values of all choices, so there are no further paths
			return false;
		}

		/// <summary>
		///   Handles a nondeterministic choice that chooses between <paramref name="valueCount" /> values.
		/// </summary>
		/// <param name="valueCount">The number of values that can be chosen.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int HandleChoice(int valueCount)
		{
			++_choiceIndex;

			// If we have a preselected value that we should choose for the current path, return it
			var chosenValuesMaxIndex = _chosenValues.Count - 1;
			if (_choiceIndex <= chosenValuesMaxIndex)
				return _chosenValues[_choiceIndex].OptionIndex;

			// We haven't encountered this choice before; store the value count and return the first value
			_valueCount.Push(valueCount);
			var oldContinuationId = _continuationId;
			_continuationId = _nextFreeContinuationId;

			var newChosenValue =
				new LtmdpChosenValue
				{
					OptionIndex = 0,
					ContinuationId = _continuationId,
					Probability = GetProbabilityOfPreviousPath() // no probability is changed for this choice
				};
			_chosenValues.Push(newChosenValue);

			_nextFreeContinuationId = _nextFreeContinuationId + valueCount;
			LtmdpStepGraph.NonDeterministicSplit(oldContinuationId, _continuationId, _continuationId + valueCount - 1);

			return 0;
		}


		/// <summary>
		///   Handles a probabilistic choice that chooses between <paramref name="valueCount" /> options.
		/// </summary>
		/// <param name="valueCount">The number of values that can be chosen.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int HandleProbabilisticChoice(int valueCount)
		{
			++_choiceIndex;

			if (_nextFreeContinuationId >= 185 - valueCount)
			{

			}

			// If we have a preselected value that we should choose for the current path, return it
			var chosenValuesMaxIndex = _chosenValues.Count - 1;
			if (_choiceIndex <= chosenValuesMaxIndex)
				return _chosenValues[_choiceIndex].OptionIndex;

			// We haven't encountered this choice before; store the value count and return the first value
			_valueCount.Push(valueCount);
			var oldContinuationId = _continuationId;
			_continuationId = _nextFreeContinuationId;

			var newChosenValue =
				new LtmdpChosenValue
				{
					OptionIndex = 0,
					ContinuationId = _continuationId,
					Probability = GetProbabilityOfPreviousPath() //placeholder value
				};
			_chosenValues.Push(newChosenValue);

			_nextFreeContinuationId = _nextFreeContinuationId + valueCount;
			LtmdpStepGraph.ProbabilisticSplit(oldContinuationId, _continuationId, _continuationId + valueCount - 1);

			return 0;
		}

		/// <summary>
		///   Gets the continuation id of the current path.
		/// </summary>
		internal int GetContinuationId()
		{
			return _continuationId;
		}


		/// <summary>
		/// </summary>
		/// <param name="valueCount">The number of values that can be chosen.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void SetProbabilityOfLastChoice(Probability probability)
		{
			var probabilitiesOfChosenValuesMaxIndex = _chosenValues.Count - 1;
			// If this part of the path has previously been visited we do not change the value
			// because this value has already been set by a previous call of SetProbabilityOfLastChoice.
			// Only if we explore a new part of the path the probability needs to be written.
			// A new part of the path is explored, iff the previous HandleChoice pushed a new placeholder
			// value onto the three stacks).
			if (_choiceIndex == probabilitiesOfChosenValuesMaxIndex)
			{
				_chosenValues[_choiceIndex] =
					new LtmdpChosenValue
					{
						OptionIndex = _chosenValues[_choiceIndex].OptionIndex,
						ContinuationId = _chosenValues[_choiceIndex].ContinuationId,
						Probability = GetProbabilityOfPreviousPath() * probability
					};
			}
		}

		/// <summary>
		///   Makes taken choice identified by the <paramref name="choiceIndexToForward" /> deterministic.
		/// </summary>
		/// <param name="choiceIndexToForward">The index of the choice that should be undone.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override void ForwardUntakenChoicesAtIndex(int choiceIndexToForward)
		{
			// This method is called when it is assumed, that choosing anything different at choiceIndex
			// leads to the same state as the path until the last choice.
			// Thus, we can simply revert this unnecessary choice and add the probability of the unmade alternatives
			// of the reverted choice to the last choice.
			// Note, very small numbers get multiplied and summarized. Maybe type double is too imprecise.

			if (_valueCount[choiceIndexToForward] == 0)
				return; //Nothing to do

			Assert.That(_chosenValues[choiceIndexToForward].OptionIndex == 0, "Only first choice can be made deterministic.");

			// We disable a choice by setting the number of values that we have yet to choose to 0, effectively
			// turning the choice into a deterministic selection of the value at index 0

			var oldProbabilityUntilDeterministicChoice = GetProbabilityUntilIndex(choiceIndexToForward - 1).Value;
			var oldProbabilityOfDeterministicChoice = GetProbabilityUntilIndex(choiceIndexToForward).Value;
			var differenceProbabilityToAdd = oldProbabilityUntilDeterministicChoice - oldProbabilityOfDeterministicChoice;

			var probabilityOfLastChoicePath = GetProbabilityUntilIndex(LastChoiceIndex).Value;

			var newValueOfLastChoice = (probabilityOfLastChoicePath + differenceProbabilityToAdd);

			// set the calculated value
			_chosenValues[LastChoiceIndex] =
				new LtmdpChosenValue
				{
					OptionIndex = _chosenValues[LastChoiceIndex].OptionIndex,
					ContinuationId = _chosenValues[LastChoiceIndex].ContinuationId,
					Probability = new Probability(newValueOfLastChoice)
				};

			var parentOfLastChoice = choiceIndexToForward - 1;
			int parentContinuationId;
			if (parentOfLastChoice == -1)
			{
				// first choice was undone. The continuationId of its parent is 0
				parentContinuationId = 0;
			}
			else
			{
				parentContinuationId = _chosenValues[parentOfLastChoice].ContinuationId;
			}

			LtmdpStepGraph.MakeChoiceOfCidDeterministic(parentContinuationId);

			// Set the alternatives to zero.
			_valueCount[choiceIndexToForward] = 0;
		}

		/// <summary>
		///   Sets the choices that should be made during the next step.
		/// </summary>
		/// <param name="choices">The choices that should be made.</param>
		internal override void SetChoices(int[] choices)
		{
			Requires.NotNull(choices, nameof(choices));

			for (var i = 0; i< choices.Length; i++)
			{
				var newChosenValue =
					new LtmdpChosenValue
					{
						Probability = Probability.One,
						ContinuationId = i,
						OptionIndex = choices[i]
					};
				_chosenValues.Push(newChosenValue);
				_valueCount.Push(0);
			}
		}

		/// <summary>
		///	  The probability of the current path
		/// </summary>
		internal Probability CalculateProbabilityOfPath()
		{
			if (_choiceIndex == -1)
				return Probability.One;
			return _chosenValues[_choiceIndex].Probability;
		}

		/// <summary>
		///   Clears all choice information.
		/// </summary>
		internal override void Clear()
		{
			_chosenValues.Clear();
			_valueCount.Clear();
			_choiceIndex = -1;
			_continuationId = 0;
			_nextFreeContinuationId = 1;
		}

		/// <summary>
		///   Gets the choices that were made to generate the last transitions.
		/// </summary>
		internal override IEnumerable<int> GetChoices()
		{
			for (var i = 0; i < _chosenValues.Count; ++i)
				yield return _chosenValues[i].OptionIndex;
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		/// <param name="disposing">If true, indicates that the object is disposed; otherwise, the object is finalized.</param>
		protected override void OnDisposing(bool disposing)
		{
			if (!disposing)
				return;

			_chosenValues.SafeDispose();
			_valueCount.SafeDispose();
		}

		[Conditional("DEBUG")]
		public void AssertThatDatastructureIsIntact()
		{
			Assert.That(_valueCount.Count == _chosenValues.Count, "All stacks must have the same size");
		}
	}
}