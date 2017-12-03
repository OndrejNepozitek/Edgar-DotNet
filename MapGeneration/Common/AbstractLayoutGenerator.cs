namespace MapGeneration.Common
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using GeneralAlgorithms.DataStructures.Graphs;
	using Interfaces;
	using Grid;
	using Grid.Fast;

	public abstract class AbstractLayoutGenerator<TNode, TPolygon, TPosition> : ILayoutGenerator<TNode, TPolygon, TPosition>
	{
		protected Random Random = new Random(0);
		protected int MinimumDifference = 200;
		protected MapDescription<TNode, TPolygon> MapDescription;
		protected Graph<int> Graph;
		public event Action<ILayout<TNode, TPolygon, TPosition>> OnPerturbed;
		public event Action<ILayout<TNode, TPolygon, TPosition>> OnValid;

		private int iterationsCount;

		public IList<ILayout<TNode, TPolygon, TPosition>> GetLayouts(Graph<int> graph, int minimumLayouts = 10)
		{
			// MapDescription = mapDescription;
			Graph = graph;

			iterationsCount = 0;
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			var stack = new Stack<LayoutNode>();
			var fullLayouts = new List<Layout>();
			var graphChains = GetChains(graph);
			var initialLayout = GetInitialLayout(graphChains[0]);

			stack.Push(new LayoutNode() { Layout = initialLayout, NumberOfChains = 0 });

			while (stack.Count > 0)
			{
				var layoutNode = stack.Pop();
				var extendedLayouts = GetExtendedLayouts(layoutNode.Layout, graphChains[layoutNode.NumberOfChains], layoutNode.NumberOfChains == graphChains.Count);

				if (layoutNode.NumberOfChains + 1 == graphChains.Count)
				{
					foreach (var layout in extendedLayouts)
					{
						if (fullLayouts.TrueForAll(x =>
							x.GetDifference(layout) > 3 * MinimumDifference)
						)
						{
							fullLayouts.Add(layout);
						}
					}
				}
				else
				{
					foreach (var extendedLayout in extendedLayouts)
					{
						stack.Push(new LayoutNode() { Layout = extendedLayout, NumberOfChains = layoutNode.NumberOfChains + 1 });
					}
				}

				if (fullLayouts.Count >= minimumLayouts)
				{
					break;
				}
			}

			stopwatch.Stop();

			Console.WriteLine($"{fullLayouts.Count} layouts generated");
			Console.WriteLine($"Total time: {stopwatch.ElapsedMilliseconds} ms");
			Console.WriteLine($"Total iterations: {iterationsCount}");
			Console.WriteLine($"Iterations per second: {(int) (iterationsCount / (stopwatch.ElapsedMilliseconds / 1000f))}");

			return fullLayouts.Select(x => (ILayout<TNode, TPolygon, TPosition>) x).ToList();
		}

		private List<Layout> GetExtendedLayouts(Layout layout, List<int> chain, bool lastChain)
		{
			// TODO: change this whole section
			var t = 1f;
			var ratio = 0.9f;
			var cycles = 50;
			var trialsPerCycle = 500;
			var k = 2f;
				
			var layouts = new List<Layout>();
			var currentLayout = AddChainToLayout(layout, chain);

			for (var i = 0; i < cycles; i++)
			{
				for (var j = 0; j < trialsPerCycle; j++)
				{
					iterationsCount++;
					var perturbedLayout = PerturbLayout(currentLayout, chain, out var energyDelta); // TODO: locally perturb the layout

					OnPerturbed?.Invoke((ILayout < TNode, TPolygon, TPosition >) perturbedLayout);

					// TODO: should probably check only the perturbed node - other nodes did not change
					if (IsLayoutValid(perturbedLayout))
					{
						OnValid?.Invoke((ILayout<TNode, TPolygon, TPosition>)perturbedLayout);

						// TODO: wouldn't it be too slow to compare againts all?
						if (layouts.TrueForAll(x => x.GetDifference(perturbedLayout) > MinimumDifference))
						{
							layouts.Add(perturbedLayout);

							if (layouts.Count >= 15)
							{
								return layouts;
							}
						}
					}

					if (energyDelta < 0)
					{
						currentLayout = perturbedLayout;
					} else if (Random.NextDouble() < Math.Pow(Math.E, -energyDelta / (k * t)))
					{
						currentLayout = perturbedLayout;
					}
				}

				t = t * ratio;
			}

			return layouts;
		}

		protected virtual List<List<int>> GetChains(Graph<int> graph)
		{
			throw new NotImplementedException();
		}

		protected abstract Layout PerturbLayout(Layout layout, List<int> chain, out float energyDelta);

		protected abstract Layout AddChainToLayout(Layout layout, List<int> chain);

		protected abstract Layout GetInitialLayout(List<int> chain);

		protected abstract bool IsLayoutValid(Layout layout);

		private struct LayoutNode
		{
			public Layout Layout;

			public int NumberOfChains;
		}
	}
}
