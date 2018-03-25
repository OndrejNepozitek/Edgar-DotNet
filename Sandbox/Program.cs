namespace Sandbox
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;
	using Examples;
	using GeneralAlgorithms.Algorithms.Graphs.GraphDecomposition;
	using GeneralAlgorithms.DataStructures.Common;
	using GUI;
	using MapGeneration.Benchmarks;
	using MapGeneration.Core.ChainDecompositions;
	using MapGeneration.Core.Configurations;
	using MapGeneration.Core.Configurations.EnergyData;
	using MapGeneration.Core.GeneratorPlanners;
	using MapGeneration.Core.LayoutEvolvers;
	using MapGeneration.Core.Layouts;
	using MapGeneration.Core.MapDescriptions;
	using MapGeneration.Utils;
	using Utils;

	internal static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			RunBenchmark();
			// CompareOldAndNew();
			// RunGui();
		}

		public static void RunGui()
		{
			var layoutGenerator = LayoutGeneratorFactory.GetDefaultChainBasedGenerator<int>();
			// var layoutGenerator = LayoutGeneratorFactory.GetChainBasedGeneratorWithCorridors(new List<int>() {1});
			layoutGenerator.InjectRandomGenerator(new Random(0));

			// var mapDescription = new BasicsExample().GetMapDescription();
			// var mapDescription = new CorridorsExample().GetMapDescription();
			var mapDescription = new DifferentShapesExample().GetMapDescription();

			var settings = new GeneratorSettings
			{
				MapDescription = mapDescription,
				LayoutGenerator = layoutGenerator,

				NumberOfLayouts = 10,

				ShowPartialValidLayouts = false,
				ShowPartialValidLayoutsTime = 500,
			};

			Application.Run(new GeneratorWindow(settings));
		}

		public static void RunBenchmark()
		{
			var scale = new IntVector2(1, 1);
			var offsets = new List<int>() { 2 };
			const bool enableCorridors = false;

			var mapDescriptions = GetMapDescriptionsSet(scale, enableCorridors, offsets);

			var layoutGenerator = LayoutGeneratorFactory.GetDefaultChainBasedGenerator<int>();
			//var layoutGenerator = LayoutGeneratorFactory.GetChainBasedGeneratorWithCorridors<int>(offsets);

			var benchmark = Benchmark.CreateFor(layoutGenerator);

			layoutGenerator.InjectRandomGenerator(new Random(0));
			layoutGenerator.SetLayoutValidityCheck(false);

			var scenario = BenchmarkScenario.CreateScenarioFor(layoutGenerator);
			scenario.SetRunsCount(2);

			Benchmark.WithDefaultFiles((sw, dw) =>
			{
				benchmark.Execute(layoutGenerator, scenario, mapDescriptions, 80, 1, sw, dw);
			});
		}

		public static void CompareOldAndNew()
		{
			var mapDescriptions = GetMapDescriptionsSet(new IntVector2(1, 1), false);
			var layoutGenerator = LayoutGeneratorFactory.GetDefaultChainBasedGenerator<int>();
			var benchmark = Benchmark.CreateFor(layoutGenerator);

			layoutGenerator.InjectRandomGenerator(new Random(0));
			layoutGenerator.SetLayoutValidityCheck(false);

			var scenario = BenchmarkScenario.CreateScenarioFor(layoutGenerator);
			scenario.SetRunsCount(2);

			// Measure the difference between old and new approaches
			{
				var setups = scenario.MakeSetupsGroup();

				setups.AddSetup("Old", (generator) =>
				{
					generator.SetChainDecompositionCreator(mapDescription => new OriginalChainDecomposition());
					generator.SetGeneratorPlannerCreator(mapDescription => new SlowGeneratorPlanner<Layout<Configuration<EnergyData>, BasicEnergyData>>());
					generator.SetLayoutEvolverCreator((mapDescription, layoutOperations) =>
					{
						var evolver =
							new SimulatedAnnealingEvolver<Layout<Configuration<EnergyData>, BasicEnergyData>, int,
								Configuration<EnergyData>>(layoutOperations);
						evolver.Configure(50, 500);
						evolver.SetRandomRestarts(true, SimulatedAnnealingEvolver<Layout<Configuration<EnergyData>, BasicEnergyData>, int, Configuration<EnergyData>>.RestartSuccessPlace.OnAccepted, false, 0.5f);

						return evolver;
					});

					generator.InjectRandomGenerator(new Random(0));
				});
				setups.AddSetup("New", (generator) =>
				{
					generator.SetChainDecompositionCreator(mapDescription =>
							new BreadthFirstChainDecomposition<int>(new GraphDecomposer<int>()));
					generator.SetGeneratorPlannerCreator(mapDescription => new BasicGeneratorPlanner<Layout<Configuration<EnergyData>, BasicEnergyData>>());
					generator.SetLayoutEvolverCreator((mapDescription, layoutOperations) =>
					{
						var evolver =
							new SimulatedAnnealingEvolver<Layout<Configuration<EnergyData>, BasicEnergyData>, int,
								Configuration<EnergyData>>(layoutOperations);

						return evolver;
					});

					generator.InjectRandomGenerator(new Random(0));
				});
			}

			Benchmark.WithDefaultFiles((sw, dw) =>
			{
				benchmark.Execute(layoutGenerator, scenario, mapDescriptions, 80, 1, sw, dw);
			});
		}

		public static List<Tuple<string, MapDescription<int>>> GetMapDescriptionsSet(IntVector2 scale, bool enableCorridors, List<int> offsets = null)
		{
			return new List<Tuple<string, MapDescription<int>>>()
			{
				new Tuple<string, MapDescription<int>>("Example 1 (fig. 1)",
					new MapDescription<int>()
						.SetupWithGraph(GraphsDatabase.GetExample1())
						.AddClassicRoomShapes(scale)
						.AddCorridorRoomShapes(offsets, enableCorridors)
				),
				new Tuple<string, MapDescription<int>>("Example 2 (fig. 7 top)",
					new MapDescription<int>()
						.SetupWithGraph(GraphsDatabase.GetExample2())
						.AddClassicRoomShapes(scale)
						.AddCorridorRoomShapes(offsets, enableCorridors)
				),
				new Tuple<string, MapDescription<int>>("Example 3 (fig. 7 bottom)",
					new MapDescription<int>()
						.SetupWithGraph(GraphsDatabase.GetExample3())
						.AddClassicRoomShapes(scale)
						.AddCorridorRoomShapes(offsets, enableCorridors)
				),
				new Tuple<string, MapDescription<int>>("Example 4 (fig. 8)",
					new MapDescription<int>()
						.SetupWithGraph(GraphsDatabase.GetExample4())
						.AddClassicRoomShapes(scale)
						.AddCorridorRoomShapes(offsets, enableCorridors)
				),
				new Tuple<string, MapDescription<int>>("Example 5 (fig. 9)",
					new MapDescription<int>()
						.SetupWithGraph(GraphsDatabase.GetExample5())
						.AddClassicRoomShapes(scale)
						.AddCorridorRoomShapes(offsets, enableCorridors)
				),
			};
		}
	}
}
