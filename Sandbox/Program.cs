namespace Sandbox
{
	using System;
	using System.Collections.Generic;
	using System.IO;
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
	using MapGeneration.Interfaces.Benchmarks;
	using MapGeneration.Interfaces.Core.LayoutGenerator;
	using MapGeneration.Interfaces.Utils;
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

			var layoutGenerator = LayoutGeneratorFactory.GetDefaultChainBasedGenerator();
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

			Benchmark();
			// Application.Run(new GeneratorWindow(settings));
		}

		private static void Benchmark()
		{
			var scale = new IntVector2(1, 1);
			var offsets = new List<int>() { 2 };
			const bool enableCorridors = true;

			var mapDescriptions = new List<Tuple<string, MapDescription<int>>>()
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

			var benchmark = new Benchmark<int>();
			var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

			using (var sw = new StreamWriter(time + ".txt"))
			{
				using (var dw = new StreamWriter(time + "_debug.txt"))
				{

					var layoutGenerator = enableCorridors ? (IChainBasedLayoutGenerator<MapDescription<int>, int>) LayoutGeneratorFactory.GetChainBasedGeneratorWithCorridors(offsets, true) : LayoutGeneratorFactory.GetDefaultChainBasedGenerator();

					layoutGenerator.InjectRandomGenerator(new Random(0));
					// layoutGenerator.SetLayoutValidityCheck(false);

					var scenario = BenchmarkScenario.CreateScenarioFor(layoutGenerator);
					scenario.SetRunsCount(2);

					// Measure the difference between old and new approaces
					//{
					//	var setups = scenario.MakeSetupsGroup();

					//	setups.AddSetup("Old", (generator) =>
					//	{
					//		generator.SetChainDecompositionCreator(mapDescription => new OriginalChainDecomposition());
					//		generator.SetGeneratorPlannerCreator(mapDescription => new SlowGeneratorPlanner<Layout<Configuration<EnergyData>, BasicEnergyData>>());
					//		generator.SetLayoutEvolverCreator((mapDescription, layoutOperations) =>
					//		{
					//			var evolver =
					//				new SimulatedAnnealingEvolver<Layout<Configuration<EnergyData>, BasicEnergyData>, int,
					//					Configuration<EnergyData>>(layoutOperations);
					//			evolver.Configure(50, 500);
					//			evolver.SetRandomRestarts(true, SimulatedAnnealingEvolver<Layout<Configuration<EnergyData>, BasicEnergyData>, int, Configuration<EnergyData>>.RestartSuccessPlace.OnAccepted, false, 0.5f);

					//			return evolver;
					//		});

					//		generator.InjectRandomGenerator(new Random(0));
					//	});
					//	setups.AddSetup("New", (generator) =>
					//	{
					//		generator.SetChainDecompositionCreator(mapDescription =>
					//				new BreadthFirstChainDecomposition<int>(new GraphDecomposer<int>()));
					//		generator.SetGeneratorPlannerCreator(mapDescription => new BasicGeneratorPlanner<Layout<Configuration<EnergyData>, BasicEnergyData>>());
					//		generator.SetLayoutEvolverCreator((mapDescription, layoutOperations) =>
					//		{
					//			var evolver =
					//				new SimulatedAnnealingEvolver<Layout<Configuration<EnergyData>, BasicEnergyData>, int,
					//					Configuration<EnergyData>>(layoutOperations);

					//			return evolver;
					//		});

					//		generator.InjectRandomGenerator(new Random(0));
					//	});
					//}

					benchmark.Execute(layoutGenerator, scenario, mapDescriptions, 80, 1, sw, dw);
				}
			}
		}
	}
}
