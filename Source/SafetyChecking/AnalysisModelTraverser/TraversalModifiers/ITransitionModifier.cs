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

namespace ISSE.SafetyChecking.AnalysisModelTraverser
{
	using AnalysisModel;
	using ExecutableModel;

	/// <summary>
	///   Represents a modifier that is executed when new transitions are found during model traversal.
	/// </summary>
	internal unsafe interface ITransitionModifier
	{
		/// <summary>
		///   Optionally modifies the <paramref name="transitions" />, changing any of their values. However, no new transitions can be
		///   added; transitions can be removed by setting their <see cref="CandidateTransition.IsValid" /> flag to <c>false</c>.
		///   During subsequent traversal steps, only valid transitions and target states reached by at least one valid transition
		///   are considered.
		/// </summary>
		/// <param name="context">The context of the model traversal.</param>
		/// <param name="worker">The worker that found the transition.</param>
		/// <param name="transitions">The transitions that should be checked.</param>
		/// <param name="sourceState">The source state of the transitions.</param>
		/// <param name="sourceStateIndex">The unique index of the transition's source state.</param>
		/// <param name="isInitial">Indicates whether the transitions are initial transitions not starting in any valid source state.</param>
		void ModifyTransitions(TraversalContext context, Worker worker, TransitionCollection transitions, byte* sourceState, int sourceStateIndex, bool isInitial);
	}
}