﻿// The MIT License (MIT)
// 
// Copyright (c) 2014-2016, Institute for Software & Systems Engineering
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafetySharp.Runtime
{
	using System.Collections;
	using System.Diagnostics;
	using System.Globalization;
	using System.Runtime.CompilerServices;
	using System.Threading;
	using Analysis;
	using Analysis.ModelChecking.Transitions;
	using Modeling;
	using Serialization;
	using Utilities;


	internal class MarkovDecisionProcess : IFormalismWithStateLabeling
	{
		// Distributions here are Probability Distributions

		public string[] StateFormulaLabels { get; set; }

		public string[] StateRewardRetrieverLabels;

		// Every state might have several non-deterministic choices (and at least 1).
		// Each such choice has a probability distribution which is saved in RowsWithProbabilityDistributions.
		public int StateToRowsEntries { private set; get; } = 0;
		private int _currentMarkovChainState = -1;
		private int _rowCountOfCurrentState = 0;
		public int[] StateToRowsL;
		public int[] StateToRowsRowCount;
		

		public SparseDoubleMatrix RowsWithDistributions { get; }

		public LabelVector StateLabeling { get; }

		public MarkovDecisionProcess(int maxNumberOfStates = 1 << 21, int maxNumberOfTransitions = 0)
		{
			if (maxNumberOfTransitions <= 0)
			{
				maxNumberOfTransitions = maxNumberOfStates << 12;
				var limit = 5 * 1024 / 16 * 1024 * 1024; // 5 gb / 16 bytes (for entries)

				if (maxNumberOfTransitions < maxNumberOfStates || maxNumberOfTransitions > limit)
					maxNumberOfTransitions = limit;
			}

			StateLabeling = new LabelVector();
			RowsWithDistributions = new SparseDoubleMatrix(maxNumberOfStates + 1, maxNumberOfTransitions); // one additional row for initial distributions (more might be necessary)
			StateToRowsL = new int[maxNumberOfStates+1]; // one additional row for initial distributions
			StateToRowsRowCount = new int[maxNumberOfStates+1]; // one additional row for initial distributions
			SetRowOfStateEntriesToInvalid();
		}

		private void SetRowOfStateEntriesToInvalid()
		{
			for (var i = 0; i < StateToRowsL.Length; i++)
			{
				StateToRowsL[i] = -1;
			}
			for (var i = 0; i < StateToRowsRowCount.Length; i++)
			{
				StateToRowsRowCount[i] = -1;
			}
		}

		// Retrieving matrix phase

		public int States => StateToRowsEntries -1; //Note: returns 0 if initial distribution added and -1 if nothing was added. So check for 0 is not enough

		public int Transitions { get; private set; } = 0; //without entries of initial distribution

		public int StateToRowsEntryOfInitialDistributions = 0;
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int StateToColumn(int state) => state; //Do nothing! Just here to make the algorithms more clear.

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int ColumnToState(int state) => state; //Do nothing! Just here to make the algorithms more clear.


		// Creating matrix phase

		// For the initial distributions the process is
		//    StartWithInitialDistributions()
		//    while(distributions to add exist) {
		//	      StartWithNewInitialDistribution();
		//	      while(transitions to add exist) {
		//	          AddTransitionToInitialDistribution();
		//	      }
		//        FinishInitialDistribution()
		//    }
		//    FinishInitialDistributions()
		internal void StartWithInitialDistributions()
		{
			_rowCountOfCurrentState = 0;
			StateToRowsL[StateToRowsEntryOfInitialDistributions] = RowsWithDistributions.Rows; //set beginning row of state to the next free row
		}
		
		internal void StartWithNewInitialDistribution()
		{
			RowsWithDistributions.SetRow(RowsWithDistributions.Rows); //just append one row in the matrix
			_rowCountOfCurrentState++;
		}

		internal void AddTransitionToInitialDistribution(int markovChainState, double probability)
		{
			RowsWithDistributions.AddColumnValueToCurrentRow(new SparseDoubleMatrix.ColumnValue(StateToColumn(markovChainState), probability));
		}

		internal void FinishInitialDistribution()
		{
			RowsWithDistributions.FinishRow();
		}

		internal void FinishInitialDistributions()
		{
			StateToRowsRowCount[StateToRowsEntryOfInitialDistributions] = _rowCountOfCurrentState;
			StateToRowsEntries++;
		}
		
		// For distributions of a state the process is
		//    StartWithNewDistributions(markovChainSourceState)
		//    while(distributions to add exist) {
		//	      StartWithNewDistribution();
		//	      while(transitions to add exist) {
		//	          AddTransitionToDistribution();
		//	      }
		//        FinishDistribution()
		//    }
		//    FinishDistributions()
		internal void StartWithNewDistributions(int markovChainState)
		{
			_rowCountOfCurrentState = 0;
			_currentMarkovChainState = markovChainState;
			StateToRowsL[_currentMarkovChainState + 1] = RowsWithDistributions.Rows; //set beginning row of state to the next free row
		}

		internal void StartWithNewDistribution()
		{
			RowsWithDistributions.SetRow(RowsWithDistributions.Rows); //just append one row
			_rowCountOfCurrentState++;
		}

		internal void AddTransition(int markovChainState, double probability)
		{
			RowsWithDistributions.AddColumnValueToCurrentRow(new SparseDoubleMatrix.ColumnValue(StateToColumn(markovChainState), probability));
			Transitions++;
		}
		
		internal void FinishDistribution()
		{
			RowsWithDistributions.FinishRow();
		}

		internal void FinishDistributions()
		{
			StateToRowsRowCount[_currentMarkovChainState+1] = _rowCountOfCurrentState;
			StateToRowsEntries++;
		}



		internal void SetStateLabeling(int markovChainState, StateFormulaSet formula)
		{
			StateLabeling[markovChainState] = formula;
		}

		public void SealProbabilityMatrix()
		{
			RowsWithDistributions.OptimizeAndSeal();
		}

		// Validation

		[Conditional("DEBUG")]
		public void ValidateStates()
		{
			var enumerator = RowsWithDistributions.GetEnumerator();

			//every row contains one probability distribution (also the initial distribution)
			while (enumerator.MoveNextRow())
			{
				// for each state there is a row. The sum of all columns in a row should be 1.0
				var probability = 0.0;
				while (enumerator.MoveNextColumn())
				{
					if (enumerator.CurrentColumnValue != null)
						probability += enumerator.CurrentColumnValue.Value.Value;
					else
						throw new Exception("Entry must not be null");
				}
				if (!Probability.IsOne(probability, 0.000000001))
					throw new Exception("Probabilities should sum up to 1");
			}
		}
		
		[Conditional("DEBUG")]
		internal void PrintPathWithStepwiseHighestProbability(int steps)
		{
			var enumerator = GetEnumerator();
			Func<SparseDoubleMatrix.ColumnValue> selectRowEntryWithHighestProbability =
				() =>
				{
					var candidate = new SparseDoubleMatrix.ColumnValue(-1,Double.NegativeInfinity);
					while (enumerator.MoveNextDistribution())
					{
						while (enumerator.MoveNextTransition())
						{
							if (candidate.Value < enumerator.CurrentTransition.Value)
								candidate = enumerator.CurrentTransition;
						}
					}
					
					return candidate;
				};
			Action<int, double> printStateAndProbability =
				(state, probability) =>
				{
					Console.Write($"step: {probability.ToString(CultureInfo.InvariantCulture)} {state}");
					for (var i = 0; i < StateFormulaLabels.Length; i++)
					{
						var label = StateFormulaLabels[i];
						Console.Write(" " + label + "=");
						if (StateLabeling[state][i])
							Console.Write("true");
						else
							Console.Write("false");
					}
					for (var i = 0; i < StateRewardRetrieverLabels.Length; i++)
					{
						var label = StateRewardRetrieverLabels[i];
						Console.Write(" " + label + "=");
						Console.Write("TODO");
					}
					Console.WriteLine();
				};


			enumerator.SelectInitialDistributions();
			var currentTuple = selectRowEntryWithHighestProbability();
			printStateAndProbability(ColumnToState(currentTuple.Column), currentTuple.Value);
			var lastState = ColumnToState(currentTuple.Column);
			
			for (var i = 0; i < steps; i++)
			{
				enumerator.SelectSourceState(lastState);
				currentTuple = selectRowEntryWithHighestProbability();
				printStateAndProbability(ColumnToState(currentTuple.Column), currentTuple.Value);
				lastState = ColumnToState(currentTuple.Column);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int InitialDistributionsRowL()
		{
			return StateToRowsL[StateToRowsEntryOfInitialDistributions];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int InitialDistributionsRowH()
		{
			return StateToRowsL[StateToRowsEntryOfInitialDistributions] + StateToRowsRowCount[StateToRowsEntryOfInitialDistributions];
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int StateToRowL(int state)
		{
			return StateToRowsL[state+1];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int StateToRowH(int state)
		{
			return StateToRowsL[state + 1] + StateToRowsRowCount[state + 1];
		}
		

		public Func<int, bool> CreateFormulaEvaluator(Analysis.Formula formula)
		{
			return Analysis.FormulaVisitors.FormulaEvaluatorCompilationVisitor.Compile(this, formula);
		}
		
		public UnderlyingDigraph CreateUnderlyingDigraphAllDistributions()
		{
			return UnderlyingDigraph.EdgeWhenAllDistributionsContainTransition(this);
		}

		public UnderlyingDigraph CreateUnderlyingDigraphAnyDistribution()
		{
			return UnderlyingDigraph.EdgeWhenAnyDistributionContainsTransition(this);
		}

		internal class UnderlyingDigraph
		{
			public BidirectionalGraph Graph { get; }

			public UnderlyingDigraph(BidirectionalGraph graph)
			{
				Graph = graph;
			}

			public static UnderlyingDigraph EdgeWhenAllDistributionsContainTransition(MarkovDecisionProcess markovChain)
			{
				//Assumption "every node is reachable" is fulfilled due to the construction
				var graph = new BidirectionalGraph();
				
				var enumerator = markovChain.GetEnumerator();
				while (enumerator.MoveNextState())
				{
					// select targets of first distribution as candidates
					enumerator.MoveNextDistribution();
					var possibleSuccessors = new HashSet<int>();
					while (enumerator.MoveNextTransition())
					{
						if (enumerator.CurrentTransition.Value > 0.0)
							possibleSuccessors.Add(enumerator.CurrentTransition.Column);
					}

					while (enumerator.MoveNextDistribution())
					{
						//find targets of this distribution and create the intersection. Some possibleSuccessors may be removed
						var successorsOfTransition = new HashSet<int>();
						while (enumerator.MoveNextTransition())
						{
							if (enumerator.CurrentTransition.Value > 0.0)
								successorsOfTransition.Add(enumerator.CurrentTransition.Column);
						}
						possibleSuccessors.IntersectWith(successorsOfTransition);
					}

					// add all possibleSuccessors
					foreach (var successor in possibleSuccessors)
					{
						graph.AddVerticesAndEdge(new BidirectionalGraph.Edge(enumerator.CurrentState, successor));
					}
				}
				return new UnderlyingDigraph(graph);
			}

			public static UnderlyingDigraph EdgeWhenAnyDistributionContainsTransition(MarkovDecisionProcess markovChain)
			{
				//Assumption "every node is reachable" is fulfilled due to the construction
				var graph = new BidirectionalGraph();

				var enumerator = markovChain.GetEnumerator();
				while (enumerator.MoveNextState())
				{
					// select targets of first distribution as candidates
					enumerator.MoveNextDistribution();
					var foundSuccessors = new HashSet<int>();
					while (enumerator.MoveNextTransition())
					{
						if (enumerator.CurrentTransition.Value > 0.0)
							foundSuccessors.Add(enumerator.CurrentTransition.Column);
					}

					while (enumerator.MoveNextDistribution())
					{
						//find targets of this distribution and create the union. Some possibleSuccessors may be added
						var successorsOfTransition = new HashSet<int>();
						while (enumerator.MoveNextTransition())
						{
							if (enumerator.CurrentTransition.Value > 0.0)
								successorsOfTransition.Add(enumerator.CurrentTransition.Column);
						}
						foundSuccessors.UnionWith(successorsOfTransition);
					}

					// add all possibleSuccessors
					foreach (var successor in foundSuccessors)
					{
						graph.AddVerticesAndEdge(new BidirectionalGraph.Edge(enumerator.CurrentState, successor));
					}
				}
				return new UnderlyingDigraph(graph);
			}

		}

		internal MarkovDecisionProcessEnumerator GetEnumerator()
		{
			return new MarkovDecisionProcessEnumerator(this);
		}
		
		// a nested class can access private members
		internal class MarkovDecisionProcessEnumerator
		{
			private MarkovDecisionProcess _mdp;
			private SparseDoubleMatrix.SparseDoubleMatrixEnumerator _matrixEnumerator;

			public int CurrentState { get; private set; }

			// The CurrentState has several probability distributions, but at least one.
			// These are saved in _mdp.RowsWithDistributions.
			// The row numbers [_rowLOfCurrentState,...,_rowHOfCurrentState) belong to CurrentState.
			private int _rowLOfCurrentState; //inclusive
			private int _rowHOfCurrentState; //exclusive
			//the current row of the enumerator
			public int _currentRow;


			public SparseDoubleMatrix.ColumnValue CurrentTransition => _matrixEnumerator.CurrentColumnValue.Value;

			public MarkovDecisionProcessEnumerator(MarkovDecisionProcess mdp)
			{
				_mdp = mdp;
				_matrixEnumerator = mdp.RowsWithDistributions.GetEnumerator();
				Reset();
			}

			/// <summary>
			/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
			/// </summary>
			public void Dispose()
			{
			}

			public void SelectInitialDistributions()
			{
				CurrentState = -1;
				_rowLOfCurrentState = _mdp.InitialDistributionsRowL();
				_rowHOfCurrentState = _mdp.InitialDistributionsRowH();
				_currentRow = _rowLOfCurrentState-1; //select 1 entry before the actual first entry. So MoveNextDistribution can move to the right entry.
			}

			public bool SelectSourceState(int state)
			{
				if (state > _mdp.States)
					return false;
				CurrentState = state;
				_rowLOfCurrentState = _mdp.StateToRowL(state);
				_rowHOfCurrentState = _mdp.StateToRowH(state);
				_currentRow = _rowLOfCurrentState-1; //select 1 entry before the actual first entry. So MoveNextDistribution can move to the right entry.
				return true;
			}
			
			/// <summary>
			/// Advances the enumerator to the next element of the collection.
			/// </summary>
			/// <returns>
			/// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
			/// </returns>
			/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
			public bool MoveNextState()
			{
				// MoveNextState() returns on a reseted enumerator the first state
				return SelectSourceState(CurrentState + 1);
			}

			/// <summary>
			/// Advances the enumerator to the next element of the collection.
			/// </summary>
			/// <returns>
			/// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
			/// </returns>
			/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
			public bool MoveNextDistribution()
			{
				_currentRow++;
				if (_currentRow >= _rowHOfCurrentState)
					return false;
				return _matrixEnumerator.MoveRow(_currentRow);
			}


			/// <summary>
			/// Advances the enumerator to the next element of the collection.
			/// </summary>
			/// <returns>
			/// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
			/// </returns>
			/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
			public bool MoveNextTransition()
			{
				return _matrixEnumerator.MoveNextColumn();
			}

			/// <summary>
			/// Sets the enumerator to its initial position, which is before the first element in the collection.
			/// </summary>
			/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
			public void Reset()
			{
				SelectInitialDistributions();
			}
		}
	}
}