namespace Sandbox
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using System.Windows.Forms;
	using Examples;
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
	using MapGeneration.Utils.ConfigParsing;
	using Utils;

	internal static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			GuiExceptionHandler.SetupCatching();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			RunBenchmark();
			// CompareOldAndNew();
			// RunExample();
			// ConvertToXml();
		}

		/// <summary>
		/// Runs one of the example presented in the Tutorial.
		/// </summary>
		public static void RunExample()
		{
			var configLoader = new ConfigLoader();
			var layoutGenerator = LayoutGeneratorFactory.GetDefaultChainBasedGenerator<int>();
			// var layoutGenerator = LayoutGeneratorFactory.GetChainBasedGeneratorWithCorridors<int>(new List<int>() {1});
			layoutGenerator.InjectRandomGenerator(new Random(0));

			// var mapDescription = new BasicsExample().GetMapDescription();
			// var mapDescription = configLoader.LoadMapDescription("Resources/Maps/tutorial_basicDescription.yml");

			// var mapDescription = new DifferentShapesExample().GetMapDescription();
			// var mapDescription = configLoader.LoadMapDescription("Resources/Maps/tutorial_differentShapes.yml");

			// var mapDescription = new DIfferentProbabilitiesExample().GetMapDescription();
			var mapDescription = configLoader.LoadMapDescription("Resources/Maps/tutorial_differentProbabilities.yml");

			// var mapDescription = new CorridorsExample().GetMapDescription();
			// var mapDescription = configLoader.LoadMapDescription("Resources/Maps/tutorial_corridors.yml");

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

		/// <summary>
		/// Runs a prepared benchmark.
		/// </summary>
		public static void RunBenchmark()
		{
			var scale = new IntVector2(1, 1);
			var offsets = new List<int>() { 2 };
			const bool enableCorridors = false;

			//var mapDescriptions = GetMapDescriptionsSet(scale, enableCorridors, offsets);
			var mapDescriptions = GetMapDescriptionsForThesis(false);

			var layoutGenerator = LayoutGeneratorFactory.GetDefaultChainBasedGenerator<int>();
			// var layoutGenerator = LayoutGeneratorFactory.GetChainBasedGeneratorWithCorridors<int>(offsets);

			var benchmark = Benchmark.CreateFor(layoutGenerator);

			layoutGenerator.InjectRandomGenerator(new Random(0));
			layoutGenerator.SetLayoutValidityCheck(false);

			//layoutGenerator.SetChainDecompositionCreator(mapDescription => new OldChainDecomposition<int>(new GraphDecomposer<int>()));
			//// layoutGenerator.SetChainDecompositionCreator(mapDescription => new BreadthFirstChainDecomposition<int>(new GraphDecomposer<int>(), false));
			// layoutGenerator.SetGeneratorPlannerCreator(mapDescription => new SlowGeneratorPlanner<Layout<Configuration<EnergyData>, BasicEnergyData>>());
			//layoutGenerator.SetLayoutEvolverCreator((mapDescription, layoutOperations) =>
			//{
			//	var evolver =
			//		new SimulatedAnnealingEvolver<Layout<Configuration<EnergyData>, BasicEnergyData>, int,
			//			Configuration<EnergyData>>(layoutOperations);
			//	evolver.Configure(50, 500);
			//	evolver.SetRandomRestarts(true, SimulatedAnnealingEvolver<Layout<Configuration<EnergyData>, BasicEnergyData>, int, Configuration<EnergyData>>.RestartSuccessPlace.OnAccepted, false, 0.5f);

			//	return evolver;
			//});

			var scenario = BenchmarkScenario.CreateScenarioFor(layoutGenerator);
			scenario.SetRunsCount(2);

			var setups = scenario.MakeSetupsGroup();
			setups.AddSetup("Fixed generator", (generator) => generator.InjectRandomGenerator(new Random(0)));

			Benchmark.WithDefaultFiles((sw, dw) =>
			{
				benchmark.Execute(layoutGenerator, scenario, mapDescriptions, 100, 1, sw, dw);
			});
		}

		/// <summary>
		/// Benchmark our speed improvements.
		/// </summary>
		public static void CompareOldAndNew()
		{
			//var mapDescriptions = GetMapDescriptionsSet(new IntVector2(1, 1), false);
			var mapDescriptions = GetMapDescriptionsForThesis(false);

			var layoutGenerator = LayoutGeneratorFactory.GetDefaultChainBasedGenerator<int>();
			var benchmark = Benchmark.CreateFor(layoutGenerator);

			layoutGenerator.InjectRandomGenerator(new Random(0));
			layoutGenerator.SetLayoutValidityCheck(false);

			var scenario = BenchmarkScenario.CreateScenarioFor(layoutGenerator);
			scenario.SetRunsCount(1);

			// Measure the difference between old and new approaches
			{
				var setups = scenario.MakeSetupsGroup();

				setups.AddSetup("Old", (generator) =>
				{
					//generator.SetChainDecompositionCreator(mapDescription => new OriginalChainDecomposition());
					generator.SetChainDecompositionCreator(mapDescription => new OldChainDecomposition<int>());
					//generator.SetGeneratorPlannerCreator(mapDescription => new SlowGeneratorPlanner<Layout<Configuration<EnergyData>, BasicEnergyData>>());
					//generator.SetLayoutEvolverCreator((mapDescription, layoutOperations) =>
					//{
					//	var evolver =
					//		new SimulatedAnnealingEvolver<Layout<Configuration<EnergyData>, BasicEnergyData>, int,
					//			Configuration<EnergyData>>(layoutOperations);
					//	evolver.Configure(50, 500);
					//	evolver.SetRandomRestarts(true, SimulatedAnnealingEvolver<Layout<Configuration<EnergyData>, BasicEnergyData>, int, Configuration<EnergyData>>.RestartSuccessPlace.OnAccepted, false, 0.5f);

					//	return evolver;
					//});

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
				setups.AddSetup("New", (generator) =>
				{
					generator.SetChainDecompositionCreator(mapDescription =>
							new BreadthFirstChainDecomposition<int>());
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

		public static List<Tuple<string, MapDescription<int>>> GetMapDescriptionsForThesis(bool withCorridors)
		{
			var path = "Resources/Maps/Thesis/";
			List<string> files;

			if (withCorridors)
			{
				files = new List<string>()
				{
					"backtracking_corridors",
					"generating_one_layout_corridors",
					"example1_corridors",
					"example2_corridors",
					"example3_corridors",
					"game1_corridors",
					"game2_corridors",
				};
			}
			else
			{
				files = new List<string>()
				{
					"backtracking_advanced",
					"generating_one_layout_advanced",
					"example1_advanced",
					"example2_advanced",
					"example3_advanced",
					"game1_basic",
					"game2_basic",
				};
			}

			var descriptions = new List<Tuple<string, MapDescription<int>>>();
			var configLoader = new ConfigLoader();

			foreach (var file in files)
			{
				var filename = $"{path}{file}.yml";

				using (var sr = new StreamReader(filename))
				{
					var description = configLoader.LoadMapDescription(sr);
					descriptions.Add(new Tuple<string, MapDescription<int>>(file, description));
				}
			}

			return descriptions;
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

		public static void ConvertToXml()
		{
			foreach (var filename in Directory.GetFiles("Resources/Maps/Thesis"))
			{
				ConvertToXml(filename);
			}
		}

		public static void ConvertToXml(string filename)
		{
			var configLoader = new ConfigLoader();
			var withoutExt = Path.GetFileNameWithoutExtension(filename);
			var xmlName = $"{withoutExt}.xml";
			var mapDescription = configLoader.LoadMapDescription(filename);
			var graph = mapDescription.GetGraph();

			using (var sw = new StreamWriter($"D:\\Trash\\LevelSyn-1.1\\data\\Thesis\\{xmlName}"))
			{
				sw.WriteLine("<?xml version=\"1.0\" standalone=\"yes\" ?>");
				sw.WriteLine("<Graph>");

				foreach (var vertex in graph.Vertices)
				{
					sw.WriteLine($"  <Node name=\"{vertex}\" />");
				}

				foreach (var edge in graph.Edges)
				{
					sw.WriteLine($"  <Edge node0=\"{edge.From}\" node1=\"{edge.To}\" />");
				}

				sw.WriteLine("</Graph>");
			}
		}
	}
}
