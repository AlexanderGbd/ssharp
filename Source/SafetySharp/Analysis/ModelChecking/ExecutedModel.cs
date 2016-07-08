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

namespace SafetySharp.Analysis.ModelChecking
{
	using System;
	using Modeling;
	using Runtime;
	using Runtime.Serialization;
	using Transitions;
	using Utilities;

	/// <summary>
	///   Represents an <see cref="AnalysisModel" /> that computes its state by executing a <see cref="Runtime.RuntimeModel" />.
	/// </summary>
	internal abstract unsafe class ExecutedModel : AnalysisModel
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="createModel">A factory function that creates the model instance that should be executed.</param>
		internal ExecutedModel(Func<RuntimeModel> createModel)
		{
			Requires.NotNull(createModel, nameof(createModel));

			RuntimeModelCreator = createModel;
			RuntimeModel = createModel();
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="runtimeModel">The model instance that should be executed.</param>
		internal ExecutedModel(RuntimeModel runtimeModel)
		{
			Requires.NotNull(runtimeModel, nameof(runtimeModel));
			RuntimeModel = runtimeModel;
		}

		/// <summary>
		///   Gets the runtime model that is directly or indirectly analyzed by this <see cref="AnalysisModel" />.
		/// </summary>
		public sealed override RuntimeModel RuntimeModel { get; }

		/// <summary>
		///   Gets the factory function that was used to create the runtime model that is directly or indirectly analyzed by this
		///   <see cref="AnalysisModel" />.
		/// </summary>
		public sealed override Func<RuntimeModel> RuntimeModelCreator { get; }

		/// <summary>
		///   Gets the model's state vector layout.
		/// </summary>
		public StateVectorLayout StateVectorLayout => RuntimeModel.StateVectorLayout;

		/// <summary>
		///   Gets the size of the model's state vector in bytes.
		/// </summary>
		public sealed override int StateVectorSize => RuntimeModel.StateVectorSize;

		/// <summary>
		///   Updates the activation states of the worker's faults.
		/// </summary>
		/// <param name="getActivation">The callback that should be used to determine a fault's activation state.</param>
		internal void ChangeFaultActivations(Func<Fault, Activation> getActivation)
		{
			RuntimeModel.ChangeFaultActivations(getActivation);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		/// <param name="disposing">If true, indicates that the object is disposed; otherwise, the object is finalized.</param>
		protected override void OnDisposing(bool disposing)
		{
			if (disposing)
				RuntimeModel.SafeDispose();
		}

		/// <summary>
		///   Executes an initial transition of the model.
		/// </summary>
		protected abstract void ExecuteInitialTransition();

		/// <summary>
		///   Executes a transition of the model.
		/// </summary>
		protected abstract void ExecuteTransition();

		/// <summary>
		///   Generates a transition from the model's current state.
		/// </summary>
		protected abstract void GenerateTransition();

		/// <summary>
		///   Invoked before the execution of the next step is started on the model, i.e., before a set of initial
		///   states or successor states is computed.
		/// </summary>
		protected abstract void BeginExecution();

		/// <summary>
		///   Invoked after the execution of a step is completed on the model, i.e., after a set of initial
		///   states or successor states have been computed.
		/// </summary>
		protected abstract TransitionCollection EndExecution();

		/// <summary>
		///   Gets all initial transitions of the model.
		/// </summary>
		public override TransitionCollection GetInitialTransitions()
		{
			BeginExecution();
			RuntimeModel.ChoiceResolver.PrepareNextState();

			fixed (byte* state = RuntimeModel.ConstructionState)
			{
				while (RuntimeModel.ChoiceResolver.PrepareNextPath())
				{
					RuntimeModel.Deserialize(state);
					ExecuteInitialTransition();

					GenerateTransition();
				}
			}

			return EndExecution();
		}

		/// <summary>
		///   Gets all transitions towards successor states of <paramref name="state" />.
		/// </summary>
		/// <param name="state">The state the successors should be returned for.</param>
		public override TransitionCollection GetSuccessorTransitions(byte* state)
		{
			BeginExecution();
			RuntimeModel.ChoiceResolver.PrepareNextState();

			while (RuntimeModel.ChoiceResolver.PrepareNextPath())
			{
				RuntimeModel.Deserialize(state);
				ExecuteTransition();

				GenerateTransition();
			}

			return EndExecution();
		}

		/// <summary>
		///   Resets the model to its initial state.
		/// </summary>
		public sealed override void Reset()
		{
			RuntimeModel.Reset();
		}
	}
}