using System.Threading.Tasks;
using MapGeneration.Benchmarks.GeneratorRunners;
using MapGeneration.Benchmarks.ResultSaving;
using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using Sandbox.Features;

namespace Sandbox
{
	using System;
	using System.Collections.Generic;
	using System.IO;
    using System.Windows.Forms;
    using GeneralAlgorithms.DataStructures.Common;
	using GUI;
	using MapGeneration.Benchmarks;
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

            new CorridorConfigurationSpaces().Run();
            // new SimulatedAnnealingParameters().Run();
            // new TwoStageGeneration().Run();
            // CompareWithReference();
            // var task = RunBenchmark();
            // task.Wait();
            // CompareOldAndNew();
            // RunExample();
            // ConvertToXml();

            // Main();
        }

        public static void CompareWithReference()
        {
            var scale = new IntVector2(1, 1);
            var offsets = new List<int>() { 2 };

            // var mapDescriptions = GetMapDescriptionsSet(scale, false, offsets);
            var mapDescriptions = GetMapDescriptionsSet(scale, false, offsets);
            // mapDescriptions.AddRange(GetMapDescriptionsSet(scale, true, offsets));

            var benchmarkRunner = BenchmarkRunner.CreateForNodeType<int>();

            var scenario = BenchmarkScenario.CreateCustomForNodeType<int>(
                "Basic",
                input =>
                {
                    {
                        var layoutGenerator = new DungeonGeneratorOld<int>(input.MapDescription);
                        layoutGenerator.InjectRandomGenerator(new Random(0));

                        return new LambdaGeneratorRunner(() =>
                        {
                            var layouts = layoutGenerator.GenerateLayout();

                            return new GeneratorRun(layouts != null, layoutGenerator.TimeTotal, layoutGenerator.IterationsCount);
                        });
                    }
                });

            var scenarioResult = benchmarkRunner.Run(scenario, mapDescriptions, 500);

            var resultSaver = new BenchmarkResultSaver();
            resultSaver.SaveResult(scenarioResult);

            BenchmarkUtils.IsEqualToReference(scenarioResult, "BenchmarkResults/1576777328_Basic_Reference.json");
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
				MapDescriptionOld = mapDescription,
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
        public static async Task RunBenchmark()
        {
            //var mapDescriptions = GetMapDescriptionsSet(scale, enableCorridors, offsets);
            var mapDescriptions = GetInputsForThesis(false);
            var benchmarkRunner = BenchmarkRunner.CreateForNodeType<int>();
            
            var scenario = BenchmarkScenario.CreateForNodeType<int>(
                "Test", 
                input =>
                {
                    var layoutGenerator = LayoutGeneratorFactory.GetDefaultChainBasedGenerator<int>();
                    layoutGenerator.InjectRandomGenerator(new Random(0));
                    layoutGenerator.SetLayoutValidityCheck(false);

                    return layoutGenerator;
                });

            var scenarioResult = benchmarkRunner.Run(scenario, mapDescriptions, 10);

            var resultSaver = new BenchmarkResultSaver();
            // await resultSaver.SaveAndUpload(scenarioResult, "name", "group");
            resultSaver.SaveResult(scenarioResult);
        }

        /// <summary>
        /// Runs a prepared benchmark.
        /// </summary>
        //public static async Task RunBenchmarkOld()
		//{
  //          var scale = new IntVector2(1, 1);
  //          var offsets = new List<int>() { 2 };
  //          const bool enableCorridors = false;

  //          //var mapDescriptions = GetMapDescriptionsSet(scale, enableCorridors, offsets);
  //          var mapDescriptions = GetInputsForThesis(false);

  //          var layoutGenerator = LayoutGeneratorFactory.GetDefaultChainBasedGenerator<int>();
  //          // var layoutGenerator = LayoutGeneratorFactory.GetChainBasedGeneratorWithCorridors<int>(offsets);

  //          var benchmark = BenchmarkOld.CreateFor(layoutGenerator);

  //          layoutGenerator.InjectRandomGenerator(new Random(0));
  //          layoutGenerator.SetLayoutValidityCheck(false);

  //          //layoutGenerator.SetChainDecompositionCreator(mapDescription => new OldChainDecomposition<int>(new GraphDecomposer<int>()));
  //          //// layoutGenerator.SetChainDecompositionCreator(mapDescription => new BreadthFirstChainDecomposition<int>(new GraphDecomposer<int>(), false));
  //          // layoutGenerator.SetGeneratorPlannerCreator(mapDescription => new SlowGeneratorPlanner<Layout<Configuration<EnergyData>, BasicEnergyData>>());
  //          //layoutGenerator.SetLayoutEvolverCreator((mapDescription, layoutOperations) =>
  //          //{
  //          //	var evolver =
  //          //		new SimulatedAnnealingEvolver<Layout<Configuration<EnergyData>, BasicEnergyData>, int,
  //          //			Configuration<EnergyData>>(layoutOperations);
  //          //	evolver.Configure(50, 500);
  //          //	evolver.SetRandomRestarts(true, SimulatedAnnealingEvolver<Layout<Configuration<EnergyData>, BasicEnergyData>, int, Configuration<EnergyData>>.RestartSuccessPlace.OnAccepted, false, 0.5f);

  //          //	return evolver;
  //          //});

  //          var scenario = BenchmarkScenarioOld.CreateScenarioFor(layoutGenerator);
  //          scenario.SetRunsCount(2);

  //          var setups = scenario.MakeSetupsGroup();
  //          setups.AddSetup("Fixed generator", (generator) => generator.InjectRandomGenerator(new Random(0)));

  //          benchmark.Execute(layoutGenerator, scenario, mapDescriptions, 100);
  //      }

        public static List<GeneratorInput<MapDescriptionOld<int>>> GetInputsForThesis(bool withCorridors)
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

			var inputs = new List<GeneratorInput<MapDescriptionOld<int>>>();
			var configLoader = new ConfigLoader();

			foreach (var file in files)
			{
				var filename = $"{path}{file}.yml";

				using (var sr = new StreamReader(filename))
				{
					var mapDescription = configLoader.LoadMapDescription(sr);
					inputs.Add(new GeneratorInput<MapDescriptionOld<int>>(file, mapDescription));
				}
			}

			return inputs;
		}

        public static List<GeneratorInput<MapDescriptionOld<int>>> GetMapDescriptionsSet(IntVector2 scale, bool enableCorridors, List<int> offsets = null)
        {
            var inputs = new List<GeneratorInput<MapDescriptionOld<int>>>()
            {
                new GeneratorInput<MapDescriptionOld<int>>("Example 1 (fig. 1)", new MapDescriptionOld<int>()
                    .SetupWithGraph(GraphsDatabase.GetExample1())
                    .AddClassicRoomShapes(scale)
                    .AddCorridorRoomShapes(offsets, enableCorridors)),
                new GeneratorInput<MapDescriptionOld<int>>("Example 2 (fig. 7 top)",
                    new MapDescriptionOld<int>()
                        .SetupWithGraph(GraphsDatabase.GetExample2())
                        .AddClassicRoomShapes(scale)
                        .AddCorridorRoomShapes(offsets, enableCorridors)),
                new GeneratorInput<MapDescriptionOld<int>>("Example 3 (fig. 7 bottom)",
                    new MapDescriptionOld<int>()
                        .SetupWithGraph(GraphsDatabase.GetExample3())
                        .AddClassicRoomShapes(scale)
                        .AddCorridorRoomShapes(offsets, enableCorridors)),
                new GeneratorInput<MapDescriptionOld<int>>("Example 4 (fig. 8)",
                    new MapDescriptionOld<int>()
                        .SetupWithGraph(GraphsDatabase.GetExample4())
                        .AddClassicRoomShapes(scale)
                        .AddCorridorRoomShapes(offsets, enableCorridors)),
                new GeneratorInput<MapDescriptionOld<int>>("Example 5 (fig. 9)",
                    new MapDescriptionOld<int>()
                        .SetupWithGraph(GraphsDatabase.GetExample5())
                        .AddClassicRoomShapes(scale)
                        .AddCorridorRoomShapes(offsets, enableCorridors)),
            };

            if (enableCorridors)
            {
                inputs.ForEach(x => x.Name += " wc");
            }

            return inputs;
        }
    }
}
