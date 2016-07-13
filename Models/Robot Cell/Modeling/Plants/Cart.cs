// The MIT License (MIT)
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

namespace SafetySharp.CaseStudies.RobotCell.Modeling.Plants
{
	using System.Collections.Generic;
	using System.Linq;
	using Controllers;
	using SafetySharp.Modeling;

	internal class Cart : Component
	{
		[Hidden]
		private string _name;

		private Robot _position;

		public Fault Broken = new PermanentFault();

		public Workpiece LoadedWorkpiece;

		public Cart(Robot startPosition, params Route[] routes)
		{
			Routes = routes;
			_position = startPosition;
		}

		public Cart()
		{
		}

		[Hidden(HideElements = true)]
		public Route[] Routes { get; }

		public bool HasWorkpiece => LoadedWorkpiece != null;

	    public virtual bool MoveTo(Robot robot)
	    {
		    if (!CanMove(robot))
			    return false;

		    _position = robot;
		    return true;
	    }

		public void SetNames(int cartId)
		{
			_name = $"C{cartId}";
			Broken.Name = $"C{cartId}.Broken";

			//foreach (var route in Routes)
			//	route.Blocked.Name = $"C{cartId}.{route.Robot1.Name}->{route.Robot2.Name}.Blocked";
		}

		public override string ToString()
		{
			return $"{_name}@{_position.Name}: Workpiece: {LoadedWorkpiece?.Name}";
		}

		public virtual bool CanMove(Robot robot)
		{
			if (_position == robot)
				return true;

			//return Routes.Any(route => route.CanNavigate(_position, robot));

			// todo: for now, ignore specific routes
			var allRobots = Routes.SelectMany(route => new[] { route.Robot1, route.Robot2 }).ToArray();
			return allRobots.Contains(_position) && allRobots.Contains(robot);
		}

		[FaultEffect(Fault = nameof(Broken))]
		internal class BrokenEffect : Cart
		{
			public override bool MoveTo(Robot robot) => false;
			public override bool CanMove(Robot robot) => false;
		}
	}
}