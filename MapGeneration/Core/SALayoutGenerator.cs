namespace MapGeneration.Core
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Threading;
	using Benchmarks;
	using ConfigurationSpaces;
	using Doors;
	using GeneralAlgorithms.Algorithms.Common;
	using GeneralAlgorithms.Algorithms.Graphs.GraphDecomposition;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Graphs;
	using GeneralAlgorithms.DataStructures.Polygons;
	using GraphDecomposition;
	using Interfaces;
	using Utils;

	public class SALayoutGenerator<TNode> : ILayoutGenerator<TNode>, IRandomInjectable, IBenchmarkable
	{
		private IChainDecomposition<int> chainDecomposition = new BasicChainsDecomposition<int>(new GraphDecomposer<int>());
		private IConfigurationSpaces<int, IntAlias<GridPolygon>, Configuration> configurationSpaces;
		private readonly ConfigurationSpacesGenerator configurationSpacesGenerator = new ConfigurationSpacesGenerator(new PolygonOverlap(), DoorHandler.DefaultHandler, new OrthogonalLineIntersection(), new GridPolygonUtils());
		private LayoutOperations<int, Layout, Configuration, IntAlias<GridPolygon>> layoutOperations;
		private Random random = new Random(0);

		private IMapDescription<TNode> mapDescription;
		private FastGraph<TNode> graph;

		// Debug and benchmark variables
		private int iterationsCount;
		private bool withDebugOutput;
		private long timeFirst;
		private long timeTen;
		private int layoutsCount;
		protected bool BenchmarkEnabled;
		private bool perturbPositionAfterShape;
		private bool lazyProcessing;

		private bool sigmaFromAvg;
		private int sigmaScale;

		private int avgSize;
		private bool differenceFromAvg;
		private float differenceScale;

		private CancellationToken? cancellationToken;

		// Events
		public event Action<IMapLayout<TNode>> OnPerturbed;
		public event Action<IMapLayout<TNode>> OnValid;
		public event Action<IMapLayout<TNode>> OnValidAndDifferent;
		public event Action<IMapLayout<TNode>> OnFinal;

		private double minimumDifference = 200; // TODO: change
		private double shapePerturbChance = 0.4f;

		public IList<IMapLayout<TNode>> GetLayouts(IMapDescription<TNode> mapDescription, int numberOfLayouts = 10)
		{
			return GetLayouts2(mapDescription, numberOfLayouts);

			// TODO: should not be done like this
			configurationSpaces = configurationSpacesGenerator.Generate((MapDescription<TNode>) mapDescription); 
			layoutOperations = new LayoutOperations<int, Layout, Configuration, IntAlias<GridPolygon>>(configurationSpaces, new PolygonOverlap(), 15800f);
			configurationSpaces.InjectRandomGenerator(random);
			layoutOperations.InjectRandomGenerator(random);

			this.mapDescription = mapDescription;
			graph = mapDescription.GetGraph();

			iterationsCount = 0;
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			var stack = new Stack<LayoutNode>();
			var fullLayouts = new List<Layout>();

			var graphChains = chainDecomposition.GetChains(graph);
			var initialLayout = new Layout(graph);

			stack.Push(new LayoutNode { Layout = AddChainToLayout(initialLayout, graphChains[0]), NumberOfChains = 0 });

			if (withDebugOutput)
			{
				Console.WriteLine("--- Simulation has started ---");
			}

			while (stack.Count > 0)
			{
				var layoutNode = stack.Pop();
				var extendedLayouts = GetExtendedLayouts(layoutNode.Layout, graphChains[layoutNode.NumberOfChains], layoutNode.NumberOfChains);

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

			return fullLayouts.Select(x => ConvertLayout(x)).ToList();
		}

		private IList<IMapLayout<TNode>> GetLayouts2(IMapDescription<TNode> mapDescription, int numberOfLayouts = 10)
		{
			// TODO: should not be done like this
			configurationSpaces = configurationSpacesGenerator.Generate((MapDescription<TNode>)mapDescription);
			var avgArea = GetAverageArea(configurationSpaces.GetAllShapes());
			avgSize = GetAverageSize(configurationSpaces.GetAllShapes());
			var sigma = sigmaFromAvg ? sigmaScale * avgSize : 15800f;
			layoutOperations = new LayoutOperations<int, Layout, Configuration, IntAlias<GridPolygon>>(configurationSpaces, new PolygonOverlap(), sigma);
			configurationSpaces.InjectRandomGenerator(random);
			layoutOperations.InjectRandomGenerator(random);

			this.mapDescription = mapDescription;
			graph = mapDescription.GetGraph();

			iterationsCount = 0;
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			var stack = new Stack<SAInstance>();
			var fullLayouts = new List<Layout>();

			var graphChains = chainDecomposition.GetChains(graph);
			var initialLayout = new Layout(graph);

			stack.Push(new SAInstance() { Layouts = GetExtendedLayouts(AddChainToLayout(initialLayout, graphChains[0]), graphChains[0], 0).GetEnumerator(), NumberOfChains = 0 });

			if (withDebugOutput)
			{
				Console.WriteLine("--- Simulation has started ---");
			}

			while (stack.Count > 0)
			{
				if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
					break;

				if (iterationsCount > 600000)
				{
					break;
				}

				List<Layout> extendedLayouts;
				var saInstance = stack.Peek();

				if (lazyProcessing)
				{
					var hasMoreLayouts = saInstance.Layouts.MoveNext();
					var layout = saInstance.Layouts.Current;

					if (!hasMoreLayouts)
					{
						stack.Pop();
					}

					if (layout == null)
						continue;

					extendedLayouts = new List<Layout>() { layout };
				}
				else
				{
					extendedLayouts = new List<Layout>();
					var hasMoreLayouts = saInstance.Layouts.MoveNext();

					do
					{
						var layout = saInstance.Layouts.Current;
						if (layout == null)
							break;

						extendedLayouts.Add(layout);
						hasMoreLayouts = saInstance.Layouts.MoveNext();

					} while (hasMoreLayouts);

					stack.Pop();
				}
				
				if (saInstance.NumberOfChains + 1 == graphChains.Count)
				{
					foreach (var layout in extendedLayouts)
					{
						if (IsDifferentEnough(layout, fullLayouts))
						{
							if (fullLayouts.Count == 0)
							{
								timeFirst = stopwatch.ElapsedMilliseconds;
							}

							fullLayouts.Add(layout);
							OnFinal?.Invoke(ConvertLayout(layout));
						}
					}
				}
				else
				{
					var sorted = extendedLayouts
						.Select(x => AddChainToLayout(x, graphChains[saInstance.NumberOfChains + 1]))
						.OrderByDescending(x => x.GetEnergy());


					foreach (var extendedLayout in sorted)
					{
						stack.Push(new SAInstance() { Layouts = GetExtendedLayouts(extendedLayout, graphChains[saInstance.NumberOfChains + 1], saInstance.NumberOfChains + 1).GetEnumerator(), NumberOfChains = saInstance.NumberOfChains + 1 });
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

			return fullLayouts.Select(x => ConvertLayout(x)).ToList();
		}

		private IEnumerable<Layout> GetExtendedLayouts(Layout layout, List<int> chain, int chainNumber)
		{
			const int cycles = 50;
			const int trialsPerCycle = 500;

			const double p0 = 0.2d;
			const double p1 = 0.01d;
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

				if (chainNumber != 0)
				{
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
				}

				#endregion
				
				for (var j = 0; j < trialsPerCycle; j++)
				{
					if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
						yield break;

					iterationsCount++;
					var perturbedLayout = PerturbLayout(currentLayout, chain, out var energyDelta);

					OnPerturbed?.Invoke(ConvertLayout(perturbedLayout, false));

					// TODO: can we check the energy instead?
					if (IsLayoutValid(perturbedLayout))
					{
						OnValid?.Invoke(ConvertLayout(perturbedLayout));

						// TODO: wouldn't it be too slow to compare againts all?
						if (IsDifferentEnough(perturbedLayout, layouts))
						{
							// AddDoors(new List<Layout>() { perturbedLayout });
							layouts.Add(perturbedLayout);
							OnValidAndDifferent?.Invoke(ConvertLayout(perturbedLayout));
							wasAccepted = true;

							yield return perturbedLayout;

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

								yield break;
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
		}

		private Layout AddChainToLayout(Layout layout, List<int> chain)
		{
			layout = layout.Clone();

			foreach (var node in chain)
			{
				layoutOperations.AddNodeGreedily(layout, node);
			}

			layoutOperations.RecomputeValidityVectors(layout);
			layoutOperations.RecomputeEnergy(layout);

			return layout;
		}

		private Layout PerturbLayout(Layout layout, List<int> chain, out double energyDelta)
		{
			// TODO: sometimes perturb a node that is not in the current chain?

			var energy = layout.GetEnergy();
			var newLayout = random.NextDouble() <= shapePerturbChance ? layoutOperations.PerturbShape(layout, chain, true, perturbPositionAfterShape) : layoutOperations.PerturbPosition(layout, chain, true);

			var newEnergy = newLayout.GetEnergy();
			energyDelta = newEnergy - energy;

			return newLayout;
		}

		private bool IsLayoutValid(Layout layout)
		{
			return layout.GetEnergy() == 0; // TODO: may it cause problems?
		}

		private bool IsDifferentEnough(Layout layout, List<Layout> layouts, List<int> chain = null)
		{
			if (!differenceFromAvg)
			{
				return layouts.All(x => GetDifference(layout, x) >= 2 * minimumDifference);
			}

			return layouts.All(x => IsDifferentEnough(layout, x, chain));
		}

		private bool IsDifferentEnough(Layout first, Layout second, List<int> chain = null)
		{
			var diff = 0d;

			var nodes = chain ?? first.Graph.Vertices;
			foreach (var node in nodes)
			{
				if (first.GetConfiguration(node, out var c1) && second.GetConfiguration(node, out var c2))
				{
					diff += (float)(Math.Pow(
						                IntVector2.ManhattanDistance(c1.Shape.BoundingRectangle.Center + c1.Position,
							                c2.Shape.BoundingRectangle.Center + c2.Position), 2) * (ReferenceEquals(c1.Shape, c2.Shape) ? 1 : 4));
				}
			}

			diff = diff / (nodes.Count() * avgSize);

			return differenceScale * diff >= 1;
		}

		private double GetDifference(Layout first, Layout second, List<int> chain = null)
		{
			var diff = 0f;

			foreach (var node in chain ?? first.Graph.Vertices)
			{
				if (first.GetConfiguration(node, out var c1) && second.GetConfiguration(node, out var c2))
				{
					diff += (float)(Math.Pow(
						IntVector2.ManhattanDistance(c1.Shape.BoundingRectangle.Center + c1.Position,
							c2.Shape.BoundingRectangle.Center + c2.Position), 2) * (ReferenceEquals(c1.Shape, c2.Shape) ? 1 : 4));
				}
			}

			/*for (var i = 0; i < first.Graph.VerticesCount; i++)
			{
				if (first.GetConfiguration(i, out var c1) && second.GetConfiguration(i, out var c2))
				{
					diff += (float)Math.Pow(
						IntVector2.ManhattanDistance(c1.Shape.BoundingRectangle.Center + c1.Position,
							c2.Shape.BoundingRectangle.Center + c2.Position), 2);
				}
			}*/

			return diff;
		}

		private IMapLayout<TNode> ConvertLayout(Layout layout, bool addRooms = true)
		{
			var rooms = new List<IRoom<TNode>>();
			var roomsDict = new Dictionary<int, Room<TNode>>();

			foreach (var vertex in graph.Vertices)
			{
				if (layout.GetConfiguration(vertex, out var configuration))
				{
					var room = new Room<TNode>(graph.GetVertex(vertex), configuration.Shape, configuration.Position);
					rooms.Add(room);

					if (!addRooms)
						continue;

					var doors = new List<Tuple<TNode, OrthogonalLine>>();
					room.Doors = doors;
					
					roomsDict[vertex] = room;
				}
			}

			if (addRooms)
			{
				foreach (var vertex in graph.Vertices)
				{
					if (layout.GetConfiguration(vertex, out var configuration))
					{
						var neighbours = graph.GetNeighbours(vertex);

						foreach (var neighbour in neighbours)
						{
							if (layout.GetConfiguration(neighbour, out var neighbourConfiguration) && neighbour > vertex)
							{
								var doorChoices = GetDoors(configuration, neighbourConfiguration);
								var randomChoice = doorChoices.GetRandom(random);

								roomsDict[vertex].Doors.Add(Tuple.Create(graph.GetVertex(neighbour), randomChoice));
								roomsDict[neighbour].Doors.Add(Tuple.Create(graph.GetVertex(vertex), randomChoice));
							}
						}
					}
				}
			}

			return new MapLayout<TNode>(rooms);
		}

		private List<OrthogonalLine> GetDoors(Configuration configuration1, Configuration configuration2)
		{
			return GetDoors(configuration2.Position - configuration1.Position,
				configurationSpaces.GetConfigurationSpace(configuration1.ShapeContainer, configuration2.ShapeContainer))
				.Select(x => x + configuration1.Position).ToList();
		}

		private List<OrthogonalLine> GetDoors(IntVector2 position, ConfigurationSpace configurationSpace)
		{
			var doors = new List<OrthogonalLine>();

			foreach (var doorInfo in configurationSpace.ReverseDoors)
			{
				var line = doorInfo.Item1;
				var doorLine = doorInfo.Item2;

				var index = line.Contains(position);

				if (index == -1)
					continue;

				var offset = line.Length - doorLine.Line.Length;
				var numberOfPositions = Math.Min(Math.Min(offset, Math.Min(index, line.Length - index)), doorLine.Line.Length) + 1;

				if (numberOfPositions == 0)
					throw new InvalidOperationException();

				for (var i = 0; i < numberOfPositions; i++)
				{
					var doorStart = doorLine.Line.GetNthPoint(Math.Max(0, index - offset) + i);
					var doorEnd = doorStart + doorLine.Length * doorLine.Line.GetDirectionVector();

					doors.Add(new OrthogonalLine(doorStart, doorEnd));
				}
			}

			if (doors.Count == 0)
				throw new InvalidOperationException();

			return doors;
		}

		public void EnableDebugOutput(bool enable)
		{
			withDebugOutput = enable;
		}

		private struct LayoutNode
		{
			public Layout Layout;

			public int NumberOfChains;
		}

		private class SAInstance
		{
			public IEnumerator<Layout> Layouts;

			public int NumberOfChains;
		}

		public void InjectRandomGenerator(Random random)
		{
			this.random = random;
			layoutOperations?.InjectRandomGenerator(random);
		}

		long IBenchmarkable.TimeFirst => timeFirst;

		long IBenchmarkable.TimeTen => timeTen;

		int IBenchmarkable.IterationsCount => iterationsCount;

		int IBenchmarkable.LayoutsCount => layoutsCount;

		void IBenchmarkable.EnableBenchmark(bool enable)
		{
			BenchmarkEnabled = enable;
		}

		public void SetChainDecomposition(IChainDecomposition<int> chainDecomposition)
		{
			this.chainDecomposition = chainDecomposition;
		}

		public void EnablePerturbPositionAfterShape(bool enable)
		{
			perturbPositionAfterShape = enable;
		}

		public void EnableLazyProcessing(bool enable)
		{
			lazyProcessing = enable;
		}

		public void EnableSigmaFromAvg(bool enable, int scale = 0)
		{
			if (enable && scale == 0)
				throw new InvalidOperationException();

			sigmaFromAvg = enable;
			sigmaScale = scale;
		}

		public void EnableDifferenceFromAvg(bool enable, float scale = 0)
		{
			if (enable && scale == 0)
				throw new InvalidOperationException();

			differenceFromAvg = enable;
			differenceScale = scale;
		}

		protected int GetAverageSize(IEnumerable<IntAlias<GridPolygon>> polygons)
		{
			return (int) polygons.Select(x => x.Value.BoundingRectangle).Average(x => (x.Width + x.Height) / 2);
		}

		protected int GetAverageArea(IEnumerable<IntAlias<GridPolygon>> polygons)
		{
			return (int) polygons.Select(x => x.Value.BoundingRectangle).Average(x => x.Area);
		}

		public void SetCancellationToken(CancellationToken cancellationToken)
		{
			this.cancellationToken = cancellationToken;
		} 
	}
}