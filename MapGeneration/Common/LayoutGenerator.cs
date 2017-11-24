namespace MapGeneration.Common
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using DataStructures.Graphs;
	using Interfaces;

	public abstract class LayoutGenerator<TLayout, TPolygon, TNode> : ILayoutGenerator<TLayout, TPolygon, TNode>
		where TNode : IComparable<TNode>
		where TLayout : ILayout<TPolygon>
	{
		protected Random Random = new Random();
		protected IGraph<TNode> Graph;
		protected Action<TLayout> action;
		private int iterationsCount;

		public IList<TLayout> GetLayouts(IGraph<TNode> graph, Action<TLayout> action, int minimumLayouts = 10)
		{
			Graph = graph;
			this.action = action;
			iterationsCount = 0;
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			var stack = new Stack<LayoutNode>();
			var fullLayouts = new List<TLayout>();
			var graphChains = GetChains(graph);
			var initialLayout = GetInitialLayout(graphChains[0]);

			stack.Push(new LayoutNode() { Layout = initialLayout, NumberOfChains = 0 });

			while (stack.Count > 0)
			{
				var layoutNode = stack.Pop();
				var extendedLayouts = GetExtendedLayouts(layoutNode.Layout, graphChains[layoutNode.NumberOfChains], layoutNode.NumberOfChains == graphChains.Count);

				if (layoutNode.NumberOfChains + 1 == graphChains.Count)
				{
					fullLayouts.AddRange(extendedLayouts);
				}
				else
				{
					foreach (var extendedLayout in extendedLayouts)
					{
						stack.Push(new LayoutNode(){ Layout = extendedLayout, NumberOfChains = layoutNode.NumberOfChains + 1 });
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
			Console.WriteLine($"Iterations per second: {iterationsCount / (stopwatch.ElapsedMilliseconds / 1000)}");

			return fullLayouts;
		}

		private List<TLayout> GetExtendedLayouts(TLayout layout, List<TNode> chain, bool lastChain)
		{
			// TODO: change this whole section
			var t = 0.6f;
			var ratio = 0.9f;
			var cycles = 50;
			var trialsPerCycle = 500;
			var k = 2f;
			var minimumDifference = 50;
				
			var layouts = new List<TLayout>();
			var currentLayout = AddChainToLayout(layout, chain);

			for (var i = 0; i < cycles; i++)
			{
				for (var j = 0; j < trialsPerCycle; j++)
				{
					iterationsCount++;
					var perturbedLayout = PerturbLayout(currentLayout, chain, out var energyDelta); // TODO: locally perturb the layout

					// TODO: should probably check only the perturbed node - other nodes did not change
					if (IsLayoutValid(perturbedLayout))
					{
						// TODO: wouldn't it be too slow to compare againts all?
						if (layouts.TrueForAll(x => x.GetDifference(perturbedLayout) > minimumDifference))
						{
							layouts.Add(perturbedLayout);
							action(perturbedLayout);

							if (layouts.Count > 20)
							{
								return layouts;
							}
						}
					}

					//var energyOriginal = currentLayout.GetEnergy();
					//var energyPerturbed = perturbedLayout.GetEnergy();
					//var energyDelta =  energyPerturbed - energyOriginal;

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

		protected virtual List<List<TNode>> GetChains(IGraph<TNode> graph)
		{
			throw new NotImplementedException();
		}

		protected abstract TLayout PerturbLayout(TLayout layout, List<TNode> chain, out float energyDelta);

		protected abstract TLayout AddChainToLayout(TLayout layout, List<TNode> chain);

		protected abstract TLayout GetInitialLayout(List<TNode> chain);

		protected abstract bool IsLayoutValid(TLayout layout);

		private struct LayoutNode
		{
			public TLayout Layout;

			public int NumberOfChains;
		}
	}
}
