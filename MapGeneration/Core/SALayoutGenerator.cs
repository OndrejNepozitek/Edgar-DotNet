namespace MapGeneration.Core
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using GeneralAlgorithms.Algorithms.Graphs.GraphDecomposition;
	using GeneralAlgorithms.DataStructures.Graphs;
	using Interfaces;

	public class SALayoutGenerator<TNode> : ILayoutGenerator<TNode>
	{
		private readonly IGraphDecomposer<int> graphDecomposer = new GraphDecomposer<int>();
		private Random random;

		private IMapDescription<TNode> mapDescription;
		private FastGraph<TNode> graph;

		// Debug and benchmark variables
		private int iterationsCount;
		private bool withDebugOutput;
		private long timeFirst;
		private long timeTen;
		private int layoutsCount;

		// Events
		public event Action<ILayout<TNode, Configuration>> OnPerturbed;
		public event Action<ILayout<TNode, Configuration>> OnValid;
		public event Action<ILayout<TNode, Configuration>> OnValidAndDifferent;

		private double minimumDifference = 300; // TODO: change

		public IList<IMapLayout<TNode>> GetLayouts(IMapDescription<TNode> mapDescription, int numberOfLayouts = 10)
		{
			this.mapDescription = mapDescription;
			graph = mapDescription.GetGraph();

			iterationsCount = 0;
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			var stack = new Stack<LayoutNode>();
			var fullLayouts = new List<Layout>();
			var graphChains = graphDecomposer.GetChains(graph);
			var initialLayout = new Layout(graph);

			stack.Push(new LayoutNode { Layout = AddChainToLayout(initialLayout, graphChains[0]), NumberOfChains = 0 });

			if (withDebugOutput)
			{
				Console.WriteLine("--- Simulation has started ---");
			}

			while (stack.Count > 0)
			{
				var layoutNode = stack.Pop();
				var extendedLayouts = GetExtendedLayouts(layoutNode.Layout, graphChains[layoutNode.NumberOfChains]);

				if (layoutNode.NumberOfChains + 1 == graphChains.Count)
				{
					foreach (var layout in extendedLayouts)
					{
						if (fullLayouts.TrueForAll(x =>
							GetDifference(x, layout) > 2 * minimumDifference)
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
					var sorted = extendedLayouts
						.Select(x => AddChainToLayout(x, graphChains[layoutNode.NumberOfChains + 1]))
						.OrderByDescending(x => x.GetEnergy());


					foreach (var extendedLayout in sorted)
					{
						stack.Push(new LayoutNode() { Layout = extendedLayout, NumberOfChains = layoutNode.NumberOfChains + 1 });
					}
				}

				if (fullLayouts.Count >= numberOfLayouts)
				{
					break;
				}
			}

			stopwatch.Stop();
			timeTen = stopwatch.ElapsedMilliseconds;
			layoutsCount = fullLayouts.Count;

			if (withDebugOutput)
			{
				Console.WriteLine($"{fullLayouts.Count} layouts generated");
				Console.WriteLine($"Total time: {stopwatch.ElapsedMilliseconds} ms");
				Console.WriteLine($"Total iterations: {iterationsCount}");
				Console.WriteLine($"Iterations per second: {(int)(iterationsCount / (stopwatch.ElapsedMilliseconds / 1000f))}");
			}

			// AddDoors(fullLayouts); TODO: how?

			return fullLayouts.Select(ConvertLayout).ToList();
		}

		private List<Layout> GetExtendedLayouts(Layout layout, List<int> chain)
		{
			var cycles = 50;
			var trialsPerCycle = 500;

			var p0 = 0.2d;
			var p1 = 0.01d;
			var t0 = -1d / Math.Log(p0);
			var t1 = -1d / Math.Log(p1);
			var ratio = Math.Pow(t1 / t0, 1d / (cycles - 1));
			var deltaEAvg = 0d;
			var acceptedSolutions = 1;

			var t = t0;

			var layouts = new List<Layout>();
			var originalLayout = layout; //AddChainToLayout(layout, chain);
			var currentLayout = originalLayout;

			#region Debug output

			if (withDebugOutput)
			{
				Console.WriteLine($"Initial energy: {currentLayout.GetEnergy()}");
			}

			#endregion

			var numFailures = 0;

			for (var i = 0; i < cycles; i++)
			{
				var wasAccepted = false;

				#region Random restarts

				if (numFailures > 8 && random.Next(0, 2) == 0)
				{
					if (withDebugOutput)
					{
						Console.WriteLine($"Break, we got {numFailures} failures");
					}
					break;
				}

				if (numFailures > 6 && random.Next(0, 3) == 0)
				{
					if (withDebugOutput)
					{
						Console.WriteLine($"Break, we got {numFailures} failures");
					}
					break;
				}

				if (numFailures > 4 && random.Next(0, 5) == 0)
				{
					if (withDebugOutput)
					{
						Console.WriteLine($"Break, we got {numFailures} failures");
					}
					break;
				}

				if (numFailures > 2 && random.Next(0, 7) == 0)
				{
					if (withDebugOutput)
					{
						Console.WriteLine($"Break, we got {numFailures} failures");
					}
					break;
				}

				#endregion

				for (var j = 0; j < trialsPerCycle; j++)
				{
					iterationsCount++;
					var perturbedLayout = PerturbLayout(currentLayout, chain, out var energyDelta);

					// OnPerturbed?.Invoke((ILayout<TNode>) perturbedLayout);

					// TODO: can we check the energy instead?
					if (IsLayoutValid(perturbedLayout))
					{
						// OnValid?.Invoke((ILayout<TNode, TPolygon, TPosition, IntLine>)perturbedLayout);

						// TODO: wouldn't it be too slow to compare againts all?
						if (layouts.TrueForAll(x => GetDifference(layout, x, chain) > 2 * minimumDifference))
						{
							wasAccepted = true;
							// AddDoors(new List<Layout>() { perturbedLayout });
							layouts.Add(perturbedLayout);
							// OnValidAndDifferent?.Invoke((ILayout<TNode, TPolygon, TPosition, IntLine>)perturbedLayout);

							#region Debug output

							if (withDebugOutput)
							{
								Console.WriteLine($"Found layout, cycle {i}, trial {j}, energy {perturbedLayout.GetEnergy()}");
							}

							#endregion

							if (layouts.Count >= 15)
							{
								#region Debug output

								if (withDebugOutput)
								{
									Console.WriteLine($"Returning {layouts.Count} partial layouts");
								}

								#endregion

								return layouts;
							}
						}

					}

					var deltaAbs = Math.Abs(energyDelta);
					var accept = false;

					if (energyDelta > 0)
					{
						if (i == 0 && j == 0)
						{
							deltaEAvg = deltaAbs * 15;
						}

						var p = Math.Pow(Math.E, -deltaAbs / (deltaEAvg * t));
						if (random.NextDouble() < p)
							accept = true;
					}
					else
					{
						accept = true;
					}

					if (accept)
					{
						acceptedSolutions++;
						currentLayout = perturbedLayout;
						deltaEAvg = (deltaEAvg * (acceptedSolutions - 1) + deltaAbs) / acceptedSolutions;
					}

				}

				if (!wasAccepted)
				{
					numFailures++;
				}

				t = t * ratio;
			}

			#region Debug output

			if (withDebugOutput)
			{
				Console.WriteLine($"Returning {layouts.Count} partial layouts");
			}

			#endregion

			return layouts;
		}

		private Layout AddChainToLayout(Layout layout, List<int> chain)
		{
			throw new NotImplementedException();
		}

		private Layout PerturbLayout(Layout layout, List<int> chain, out double energyDelta)
		{
			throw new NotImplementedException();
		}

		private bool IsLayoutValid(Layout layout)
		{
			throw new NotImplementedException();
		}

		private double GetDifference(Layout first, Layout second, List<int> chain = null)
		{
			throw new NotImplementedException();
		}

		private IMapLayout<TNode> ConvertLayout(Layout layout)
		{
			throw new NotImplementedException();
		}

		private struct LayoutNode
		{
			public Layout Layout;

			public int NumberOfChains;
		}
	}
}