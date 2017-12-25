namespace MapGeneration.Core
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using Common;
	using GeneralAlgorithms.Algorithms.Graphs.GraphDecomposition;
	using GeneralAlgorithms.DataStructures.Graphs;
	using Grid.Fast;
	using Interfaces;

	public class SALayoutGenerator<TNode> : ILayoutGenerator<TNode>
	{
		private readonly IGraphDecomposer<TNode> graphDecomposer = new DummyGraphDecomposer<TNode>();

		private IMapDescription<TNode> mapDescription;
		private FastGraph<TNode> graph;

		private int iterationsCount;

		public IList<ILayout<TNode>> GetLayouts(IMapDescription<TNode> mapDescription, int numberOfLayouts = 10)
		{
			this.mapDescription = mapDescription;
			graph = mapDescription.GetGraph();

			iterationsCount = 0;
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			var stack = new Stack<LayoutNode>();
			var fullLayouts = new List<Layout>();
			var graphChains = graphDecomposer.GetChains(graph);
			var initialLayout = new Layout(graph.VerticesCount);

			stack.Push(new LayoutNode { Layout = AddChainToLayout(initialLayout, graphChains[0]), NumberOfChains = 0 });

			if (WithDebug)
			{
				Console.WriteLine("--- Simulation has started ---");
			}

			while (stack.Count > 0)
			{
				var layoutNode = stack.Pop();
				var extendedLayouts = GetExtendedLayouts(layoutNode.Layout, graphChains[layoutNode.NumberOfChains], layoutNode.NumberOfChains == graphChains.Count);

				if (layoutNode.NumberOfChains + 1 == graphChains.Count)
				{
					foreach (var layout in extendedLayouts)
					{
						if (fullLayouts.TrueForAll(x =>
							x.GetDifference(layout) > 2 * MinimumDifference)
						)
						{
							if (fullLayouts.Count == 0)
							{
								timeFirst = stopwatch.ElapsedMilliseconds;
							}

							fullLayouts.Add(layout);
						}
					}
				}
				else
				{
					var sorted = extendedLayouts.Select(x => AddChainToLayout(x, graphChains[layoutNode.NumberOfChains + 1]))
						.OrderByDescending(x => x.GetEnergy());


					foreach (var extendedLayout in sorted)
					{
						stack.Push(new AbstractLayoutGenerator<,,>.LayoutNode() { Layout = extendedLayout, NumberOfChains = layoutNode.NumberOfChains + 1 });
					}
				}

				if (fullLayouts.Count >= minimumLayouts)
				{
					break;
				}
			}

			stopwatch.Stop();
			timeTen = stopwatch.ElapsedMilliseconds;
			layoutsCount = fullLayouts.Count;

			if (WithDebug)
			{
				Console.WriteLine($"{fullLayouts.Count} layouts generated");
				Console.WriteLine($"Total time: {stopwatch.ElapsedMilliseconds} ms");
				Console.WriteLine($"Total iterations: {iterationsCount}");
				Console.WriteLine($"Iterations per second: {(int)(iterationsCount / (stopwatch.ElapsedMilliseconds / 1000f))}");
			}

			AddDoors(fullLayouts);

			return fullLayouts.Select(x => (ILayout<,,,,>)x).ToList();
		}

		private struct LayoutNode
		{
			public ILayout<TNode> Layout;

			public int NumberOfChains;
		}
	}
}