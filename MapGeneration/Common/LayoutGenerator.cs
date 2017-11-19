namespace MapGeneration.LayoutGenerators
{
	using System;
	using System.Collections.Generic;
	using DataStructures.Graphs;
	using Interfaces;

	public abstract class LayoutGenerator<TLayout, TPolygon, TNode> : ILayoutGenerator<TLayout, TPolygon, TNode>
		where TNode : IComparable<TNode>
		where TLayout : ILayout<TPolygon>
	{
		protected Random random = new Random();

		public IList<TLayout> GetLayouts(IGraph<TNode> graph, int minimumLayouts = 10)
		{
			var stack = new Stack<LayoutNode>();
			var fullLayouts = new List<TLayout>();
			var graphChains = GetChains(graph);
			var initialLayout = GetInitialLayout(graphChains[0]);

			stack.Push(new LayoutNode() { Layout = initialLayout, NumberOfChains = 1 });

			while (stack.Count > 0)
			{
				var layoutNode = stack.Pop();
				var extendedLayouts = GetExtendedLayouts(layoutNode.Layout, graphChains[layoutNode.NumberOfChains]);

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

			return fullLayouts;
		}

		private List<TLayout> GetExtendedLayouts(TLayout layout, List<TNode> chain)
		{
			var t = 0d;
			var ratio = 0d;
			var cycles = 100;
			var trialsPerCycle = 100;
			var k = 1d;
			var minimumDifference = 0f;
				
			var layouts = new List<TLayout>();
			var currentLayout = AddChainToLayout(layout, chain);

			for (var i = 0; i < cycles; i++)
			{
				for (var j = 0; j < trialsPerCycle; j++)
				{
					var perturbedLayout = PerturbLayout(currentLayout, chain); // TODO: locally perturb the layout

					// TODO: should probably check only the perturbed node - other nodes did not change
					if (IsLayoutValid(perturbedLayout))
					{
						// TODO: wouldn't it be too slow to compare againts all?
						if (layouts.TrueForAll(x => x.GetDifference(perturbedLayout) > minimumDifference))
						{
							layouts.Add(perturbedLayout);
						}
					}

					var energyOriginal = currentLayout.GetEnergy();
					var energyPerturbed = perturbedLayout.GetEnergy();
					var energyDelta =  energyPerturbed - energyOriginal;

					if (energyDelta < 0)
					{
						currentLayout = perturbedLayout;
					} else if (random.NextDouble() < Math.Pow(Math.E, -energyDelta / (k * t)))
					{
						currentLayout = perturbedLayout;
					}
				}

				t = t * ratio;
			}

			return layouts;
		}

		private List<List<TNode>> GetChains(IGraph<TNode> graph)
		{
			throw new NotImplementedException();
		}

		private struct LayoutNode
		{
			public TLayout Layout;

			public int NumberOfChains;
		}

		protected abstract TLayout PerturbLayout(TLayout layout, List<TNode> chain);

		protected abstract TLayout AddChainToLayout(TLayout layout, List<TNode> chain);

		protected abstract TLayout GetInitialLayout(List<TNode> chain);

		protected abstract bool IsLayoutValid(TLayout layout);
	}
}
