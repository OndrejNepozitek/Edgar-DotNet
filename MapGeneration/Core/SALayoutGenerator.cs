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
	using SimulatedAnnealing;
	using SimulatedAnnealing.GeneratorPlanner;
	using Utils;

	public class SALayoutGenerator<TNode> : ILayoutGenerator<TNode>, IRandomInjectable, IBenchmarkable
	{
		private IChainDecomposition<int> chainDecomposition = new BreadthFirstLongerChainsDecomposition<int>(new GraphDecomposer<int>());
		private IConfigurationSpaces<int, IntAlias<GridPolygon>, Configuration> configurationSpaces;
		private readonly ConfigurationSpacesGenerator configurationSpacesGenerator = new ConfigurationSpacesGenerator(new PolygonOverlap(), DoorHandler.DefaultHandler, new OrthogonalLineIntersection(), new GridPolygonUtils());
		private LayoutOperations<int, Layout, Configuration, IntAlias<GridPolygon>> layoutOperations;
		private Random random = new Random(0);

		private IMapDescription<TNode> mapDescription;
		private FastGraph<TNode> graph;
		private IGeneratorPlanner generatorPlanner = new LazyGeneratorPlanner();

		private readonly Stopwatch stopwatch = new Stopwatch();
		private readonly Action<Layout> firstLayoutTimer;

		// Debug and benchmark variables
		private int iterationsCount;
		private bool withDebugOutput;
		private long timeFirst;
		private long timeTen;
		private int layoutsCount;
		protected bool BenchmarkEnabled;
		private bool perturbPositionAfterShape;
		private bool lazyProcessing;
		private bool perturbOutsideChain;
		private float perturbOutsideChainProb;

		private bool sigmaFromAvg;
		private int sigmaScale;

		private int avgSize;

		private CancellationToken? cancellationToken;

		private SAContext context;

		// Events
		public event Action<IMapLayout<TNode>> OnPerturbed;
		public event Action<IMapLayout<TNode>> OnValid;
		public event Action<IMapLayout<TNode>> OnValidAndDifferent;
		public event Action<IMapLayout<TNode>> OnFinal;

		private double minimumDifference = 200; // TODO: change
		private double shapePerturbChance = 0.4f;

		public SALayoutGenerator()
		{
			firstLayoutTimer = layout =>
			{
				if (timeFirst == -1)
				{
					timeFirst = stopwatch.ElapsedMilliseconds;
				}
			};
		}

		public IList<IMapLayout<TNode>> GetLayouts(IMapDescription<TNode> mapDescription, int numberOfLayouts = 10)
		{
			// TODO: should not be done like this
			configurationSpaces = configurationSpacesGenerator.Generate((MapDescription<TNode>)mapDescription);
			var avgArea = GetAverageArea(configurationSpaces.GetAllShapes());
			avgSize = GetAverageSize(configurationSpaces.GetAllShapes());
			var sigma = sigmaFromAvg ? sigmaScale * avgSize : 15800f;
			layoutOperations = new LayoutOperations<int, Layout, Configuration, IntAlias<GridPolygon>>(configurationSpaces, new PolygonOverlap(), sigma);
			configurationSpaces.InjectRandomGenerator(random);
			layoutOperations.InjectRandomGenerator(random);
			context = new SAContext();

			this.mapDescription = mapDescription;
			graph = mapDescription.GetGraph();

			iterationsCount = 0;
			timeFirst = -1;
			stopwatch.Restart();

			var fullLayouts = new List<Layout>();

			var graphChains = chainDecomposition.GetChains(graph);
			var initialLayout = new Layout(graph);

			if (withDebugOutput)
			{
				Console.WriteLine("--- Simulation has started ---");
			}

			generatorPlanner.OnLayoutGenerated -= firstLayoutTimer;
			generatorPlanner.OnLayoutGenerated += firstLayoutTimer;

			// TODO: fix chain number
			fullLayouts = generatorPlanner.Generate(initialLayout, graphChains,
				(layout, chain) => GetExtendedLayouts(AddChainToLayout(layout, chain), chain, -1), context, numberOfLayouts);

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

			var numberOfFailures = 0;

			for (var i = 0; i < cycles; i++)
			{
				var wasAccepted = false;

				#region Random restarts

				if (enableRandomRestarts)
				{
					if (ShouldRestart(numberOfFailures))
					{
						break;
					}
				}

				#endregion
				
				for (var j = 0; j < trialsPerCycle; j++)
				{
					if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
						yield break;

					// TODO: should not be done like this
					iterationsCount++;
					context.IterationsCount++;

					var perturbedLayout = PerturbLayout(currentLayout, chain, out var energyDelta);

					OnPerturbed?.Invoke(ConvertLayout(perturbedLayout, false));

					// TODO: can we check the energy instead?
					if (IsLayoutValid(perturbedLayout))
					{
						#region Random restarts
						if (enableRandomRestarts && randomRestartsSuccessPlace == RestartSuccessPlace.OnValid)
						{
							wasAccepted = true;

							if (randomRestartsResetCounter)
							{
								numberOfFailures = 0;
							}
						}
						#endregion

						OnValid?.Invoke(ConvertLayout(perturbedLayout));

						// TODO: wouldn't it be too slow to compare againts all?
						if (IsDifferentEnough(perturbedLayout, layouts))
						{
							layouts.Add(perturbedLayout);
							OnValidAndDifferent?.Invoke(ConvertLayout(perturbedLayout));

							#region Random restarts
							if (enableRandomRestarts && randomRestartsSuccessPlace == RestartSuccessPlace.OnValidAndDifferent)
							{
								wasAccepted = true;

								if (randomRestartsResetCounter)
								{
									numberOfFailures = 0;
								}
							}
							#endregion

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

						#region Random restarts
						if (enableRandomRestarts && randomRestartsSuccessPlace == RestartSuccessPlace.OnAccepted)
						{
							wasAccepted = true;

							if (randomRestartsResetCounter)
							{
								numberOfFailures = 0;
							}
						}
						#endregion
					}

				}

				if (!wasAccepted)
				{
					numberOfFailures++;
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
			Layout newLayout;

			if (perturbOutsideChain && random.NextDouble() < perturbOutsideChainProb)
			{
				var chooseFrom = graph.Vertices.Where(x => layout.GetConfiguration(x, out var _)).Where(x => !chain.Contains(x)).ToList();
				newLayout = random.NextDouble() <= shapePerturbChance ? layoutOperations.PerturbShape(layout, chooseFrom, true, perturbPositionAfterShape) : layoutOperations.PerturbPosition(layout, chooseFrom, true);
			}
			else
			{
				newLayout = random.NextDouble() <= shapePerturbChance ? layoutOperations.PerturbShape(layout, chain, true, perturbPositionAfterShape) : layoutOperations.PerturbPosition(layout, chain, true);
			}

			if (enableLayoutValidityCheck)
			{
				CheckLayoutValidity(newLayout);
			}

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
			if (!enableDifferenceFromAverageSize)
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
						                5 * IntVector2.ManhattanDistance(c1.Shape.BoundingRectangle.Center + c1.Position,
							                c2.Shape.BoundingRectangle.Center + c2.Position) / (float) avgSize, 2) * (ReferenceEquals(c1.Shape, c2.Shape) ? 1 : 4));
				}
			}

			diff = diff / (nodes.Count());

			return differenceFromAverageScale * diff >= 1;
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

		string IBenchmarkable.GetPlannerLog()
		{
			return generatorPlanner.GetLog();
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

		public void EnablePerturbOutsideChain(bool enable, float chance = 0)
		{
			if (enable && chance == 0)
				throw new InvalidOperationException();

			perturbOutsideChain = enable;
			perturbOutsideChainProb = chance;
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

		#region Generator settings

		#region Generator planners

		public void SetGeneratorPlanner(IGeneratorPlanner planner)
		{
			generatorPlanner = planner;
			generatorPlanner.OnLayoutGenerated += firstLayoutTimer;
		}

		#endregion

		#region Random restarts

		private bool enableRandomRestarts = true;
		private RestartSuccessPlace randomRestartsSuccessPlace = RestartSuccessPlace.OnValidAndDifferent;
		private bool randomRestartsResetCounter = false;
		private float randomRestartsScale = 1;
		private List<int> randomRestartProbabilities = new List<int>() {2, 3, 5, 7};

		public void SetRandomRestarts(bool enable, RestartSuccessPlace successPlace = RestartSuccessPlace.OnValidAndDifferent, bool resetCounter = false, float scale = 1f)
		{
			enableRandomRestarts = enable;
			randomRestartsSuccessPlace = successPlace;
			randomRestartsResetCounter = resetCounter;

			if (scale < 1)
				throw new ArgumentException();

			randomRestartsScale = scale;
			randomRestartProbabilities = (new List<int>() { 2, 3, 5, 7 }).Select(x => (int)(x * randomRestartsScale)).ToList();
		}

		private bool ShouldRestart(int numberOfFailures)
		{
			// ReSharper disable once ReplaceWithSingleAssignment.False
			var shouldRestart = false;

			if (numberOfFailures > 8 && random.Next(0, randomRestartProbabilities[0]) == 0)
			{
				shouldRestart = true;
			} else if (numberOfFailures > 6 && random.Next(0, randomRestartProbabilities[1]) == 0)
			{
				shouldRestart = true;
			} else if (numberOfFailures > 4 && random.Next(0, randomRestartProbabilities[2]) == 0)
			{
				shouldRestart = true;
			} else if (numberOfFailures > 2 && random.Next(0, randomRestartProbabilities[3]) == 0)
			{
				shouldRestart = true;
			}

			if (shouldRestart && withDebugOutput)
			{
				Console.WriteLine($"Break, we got {numberOfFailures} failures");
			}

			return shouldRestart;
		}

		public enum RestartSuccessPlace
		{
			OnValid, OnValidAndDifferent, OnAccepted
		}

		#endregion

		#region Difference from average size

		private bool enableDifferenceFromAverageSize = true;
		private float differenceFromAverageScale = 0.4f;

		public void SetDifferenceFromAverageSize(bool enable, float scale = 1)
		{
			if (enable && scale <= 0)
				throw new ArgumentException("Scale must be greater than zero", nameof(scale));

			enableDifferenceFromAverageSize = enable;
			differenceFromAverageScale = scale;
		}

		#endregion

		#region Chain decomposition

		public void SetChainDecomposition(IChainDecomposition<int> chainDecomposition)
		{
			this.chainDecomposition = chainDecomposition;
		}

		#endregion

		#endregion

		#region Debug options

		#region Check perturbed layout validity

		private bool enableLayoutValidityCheck;

		public void SetLayoutValidityCheck(bool enable)
		{
			enableLayoutValidityCheck = enable;
		}

		/// <summary>
		/// Checks whether energies and validity vectors are the same as if they are all recomputed.
		/// </summary>
		/// <remarks>
		/// This check significantly slows down the generator.
		/// </remarks>
		/// <param name="layout"></param>
		private void CheckLayoutValidity(Layout layout)
		{
			var copy = layout.Clone();

			layoutOperations.RecomputeEnergy(copy);
			layoutOperations.RecomputeValidityVectors(copy);

			foreach (var vertex in graph.Vertices)
			{
				var isInLayout = layout.GetConfiguration(vertex, out var configurationLayout);
				var isInCopy = copy.GetConfiguration(vertex, out var configurationCopy);

				if (isInLayout != isInCopy)
					throw new InvalidOperationException("Vertices must be either set in both or absent in both");

				// Skip the check if the configuration is not set
				if (!isInLayout)
					continue;

				if (!configurationCopy.Equals(configurationLayout))
					throw new InvalidOperationException("Configurations must be equal");
			}
		}

		#endregion

		#endregion

	}
}