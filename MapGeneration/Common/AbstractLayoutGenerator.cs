namespace MapGeneration.Common
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Diagnostics;
	using System.Linq;
	using GeneralAlgorithms.DataStructures.Graphs;
	using Interfaces;
	using Grid;
	using Grid.Fast;
	using Utils.Benchmarks;

	public abstract class AbstractLayoutGenerator<TNode, TPolygon, TPosition> : ILayoutGenerator<TNode, TPolygon, TPosition>, IBenchmarkable
	{
		protected Random Random = new Random(0);
		protected int MinimumDifference = 200;
		protected MapDescription<TNode, TPolygon> MapDescription;
		protected Graph<int> Graph;
		public event Action<ILayout<TNode, TPolygon, TPosition>> OnPerturbed;
		public event Action<ILayout<TNode, TPolygon, TPosition>> OnValid;

		private int iterationsCount;
		private long timeFirst;
		private long timeTen;
		private int layoutsCount;
		protected bool BenchmarkEnabled;
		protected bool WithDebug;

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

			stack.Push(new LayoutNode() { Layout = AddChainToLayout(initialLayout, graphChains[0]), NumberOfChains = 0 });

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
						stack.Push(new LayoutNode() { Layout = extendedLayout, NumberOfChains = layoutNode.NumberOfChains + 1 });
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

			return fullLayouts.Select(x => (ILayout<TNode, TPolygon, TPosition>) x).ToList();
		}

		private List<Layout> GetExtendedLayouts(Layout layout, List<int> chain, bool lastChain)
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

			if (WithDebug)
			{
				Console.WriteLine($"Initial energy: {currentLayout.GetEnergy()}");
			}

			var numFailures = 0;

			for (var i = 0; i < cycles; i++)
			{
				var wasAccepted = false;

				if (numFailures > 8 && Random.Next(0, 2) == 0)
				{
					if (WithDebug)
					{
						Console.WriteLine($"Break, we got {numFailures} failures");
					}
					break;
				}

				if (numFailures > 6 && Random.Next(0, 3) == 0)
				{
					if (WithDebug)
					{
						Console.WriteLine($"Break, we got {numFailures} failures");
					}
					break;
				}

				if (numFailures > 4 && Random.Next(0, 5) == 0)
				{
					if (WithDebug)
					{
						Console.WriteLine($"Break, we got {numFailures} failures");
					}
					break;
				}

				if (numFailures > 2 && Random.Next(0, 7) == 0)
				{
					if (WithDebug)
					{
						Console.WriteLine($"Break, we got {numFailures} failures");
					}
					break;
				}

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
						if (layouts.TrueForAll(x => x.GetDifference(perturbedLayout, chain) > 2 * MinimumDifference))
						{
							wasAccepted = true;
							layouts.Add(perturbedLayout);

							if (WithDebug)
							{
								Console.WriteLine($"Found layout, cycle {i}, trial {j}, energy {perturbedLayout.GetEnergy()}");
							}
							
							if (layouts.Count >= 15)
							{
								if (WithDebug)
								{
									Console.WriteLine($"Returning {layouts.Count} partial layouts");	
								}

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
						if (Random.NextDouble() < p)
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

			if (WithDebug)
			{
				Console.WriteLine($"Returning {layouts.Count} partial layouts");
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

		long IBenchmarkable.TimeFirst => timeFirst;

		long IBenchmarkable.TimeTen => timeTen;

		int IBenchmarkable.IterationsCount => iterationsCount;

		int IBenchmarkable.LayoutsCount => layoutsCount;

		void IBenchmarkable.EnableBenchmark(bool enable)
		{
			BenchmarkEnabled = enable;
		}

		public void EnableDebug(bool enable)
		{
			WithDebug = enable;
		}
	}
}
