using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MapGeneration.Benchmarks;
using MapGeneration.Benchmarks.GeneratorRunners;
using MapGeneration.Benchmarks.Interfaces;
using MapGeneration.Benchmarks.ResultSaving;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using MapGeneration.Core.MapDescriptions.Interfaces;
using MapGeneration.MetaOptimization.Evolution.DungeonGeneratorEvolution;
using MapGeneration.MetaOptimization.Visualizations;
using MapGeneration.Utils.MapDrawing;
using Sandbox.Examples;

namespace Sandbox
{
	using System;
	using System.Collections.Generic;
    using System.Windows.Forms;
    using GeneralAlgorithms.DataStructures.Common;
	using GUI;
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
			GuiExceptionHandler.SetupCatching();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

            CompareWithReference();
            // new CorridorConfigurationSpaces().Run();
            // new SimulatedAnnealingParameters().Run();
            // new Clustering().Run();
            // new TwoStageGeneration().Run();
            // new PlatformersFeature().Run();
            
            // var task = RunBenchmark();
            // task.Wait();
            // CompareOldAndNew();
            // RunExample();
            // ConvertToXml();

            // Main();
        }

        public static void CompareWithReference()
        {
            var inputs = new List<DungeonGeneratorInput<int>>();
            inputs.AddRange(Program.GetMapDescriptionsSet(new IntVector2(1, 1), false, null, true));
            //inputs.AddRange(Program.GetMapDescriptionsSet(new IntVector2(1, 1), true, new List<int>() { 2 }, true));
            //inputs.AddRange(Program.GetMapDescriptionsSet(new IntVector2(1, 1), true, new List<int>() { 2 }, false));
            //inputs.AddRange(GetMapDescriptionsSet(new IntVector2(1, 1), true, new List<int>() { 2, 4, 6, 8 }, false));
            //inputs.AddRange(GetMapDescriptionsSet(new IntVector2(1, 1), true, new List<int>() { 2, 4, 6 }, false));
            //inputs.AddRange(GetMapDescriptionsSet(new IntVector2(1, 1), true, new List<int>() { 2, 4 }, false));
            //inputs.AddRange(GetMapDescriptionsSet(new IntVector2(1, 1), true, new List<int>() { 2 }, false));

            if (true)
            {
                inputs.Sort((x1, x2) => string.Compare(x1.Name, x2.Name, StringComparison.Ordinal));
            }

            var layoutDrawer = new SVGLayoutDrawer<int>();

            var benchmarkRunner = new BenchmarkRunner<IMapDescription<int>>();
            var benchmarkScenario = new BenchmarkScenario<IMapDescription<int>>("CorridorConfigurationSpaces", input =>
            {
                var dungeonGeneratorInput = (DungeonGeneratorInput<int>) input;
                var layoutGenerator = new DungeonGenerator<int>(input.MapDescription, dungeonGeneratorInput.Configuration, dungeonGeneratorInput.Offsets);
                layoutGenerator.InjectRandomGenerator(new Random(0));

                //return new LambdaGeneratorRunner(() =>
                //{
                //    var layouts = layoutGenerator.GenerateLayout();

                //    return new GeneratorRun(layouts != null, layoutGenerator.TimeTotal, layoutGenerator.IterationsCount);
                //});
                return new LambdaGeneratorRunner(() =>
                {
                    var simulatedAnnealingArgsContainer = new List<SimulatedAnnealingEventArgs>();
                    void SimulatedAnnealingEventHandler(object sender, SimulatedAnnealingEventArgs eventArgs)
                    {
                        simulatedAnnealingArgsContainer.Add(eventArgs);
                    }

                    layoutGenerator.OnSimulatedAnnealingEvent += SimulatedAnnealingEventHandler;
                    var layout = layoutGenerator.GenerateLayout();
                    layoutGenerator.OnSimulatedAnnealingEvent -= SimulatedAnnealingEventHandler;

                    var additionalData = new AdditionalRunData<int>()
                    {
                        SimulatedAnnealingEventArgs = simulatedAnnealingArgsContainer,
                        GeneratedLayoutSvg = layoutDrawer.DrawLayout(layout, 800, forceSquare: true),
                        // GeneratedLayout = layout,
                    };

                    var generatorRun = new GeneratorRun<AdditionalRunData<int>>(layout != null, layoutGenerator.TimeTotal, layoutGenerator.IterationsCount, additionalData);

                    return generatorRun;
                });
            });

            var scenarioResult = benchmarkRunner.Run(benchmarkScenario, inputs, 10);
            var resultSaver = new BenchmarkResultSaver();
            resultSaver.SaveResultDefaultLocation(scenarioResult);

            var directory = $"CorridorConfigurationSpaces/{new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()}";
            Directory.CreateDirectory(directory);

            var dataVisualization = new ChainStatsVisualization<GeneratorData>();
            foreach (var inputResult in scenarioResult.BenchmarkResults)
            {
                using (var file = new StreamWriter($"{directory}/{inputResult.InputName}.txt"))
                {
                    var generatorEvaluation = new GeneratorEvaluation<AdditionalRunData<int>>(inputResult.Runs.Cast<IGeneratorRun<AdditionalRunData<int>>>().ToList()); // TODO: ugly
                    dataVisualization.Visualize(generatorEvaluation, file);
                }
            }

            Utils.BenchmarkUtils.IsEqualToReference(scenarioResult, "BenchmarkResults/1581884301_CorridorConfigurationSpaces_Reference.json");
        }

        /// <summary>
		/// Runs one of the example presented in the Tutorial.
		/// </summary>
		public static void RunExample()
		{
            // var mapDescription = new BasicsExample().GetMapDescription();
            // var mapDescription = new DifferentRoomDescriptionsExample().GetMapDescription();
            var mapDescription = new CorridorsExample().GetMapDescription();

            //// var mapDescription = new DifferentShapesExample().GetMapDescription();
			//// var mapDescription = configLoader.LoadMapDescription("Resources/Maps/tutorial_differentShapes.yml");

			//// var mapDescription = new DIfferentProbabilitiesExample().GetMapDescription();
			//var mapDescription = configLoader.LoadMapDescription("Resources/Maps/tutorial_differentProbabilities.yml");

			//// var mapDescription = new CorridorsExample().GetMapDescription();
			//// var mapDescription = configLoader.LoadMapDescription("Resources/Maps/tutorial_corridors.yml");

            var generator = new DungeonGenerator<int>(mapDescription, new DungeonGeneratorConfiguration<int>()
            {
                RoomsCanTouch = false,
            });
            generator.InjectRandomGenerator(new Random(0));

            var settings = new GeneratorSettings
            {
                MapDescriptionOld = mapDescription,
                LayoutGenerator = generator,

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
            ////var mapDescriptions = GetMapDescriptionsSet(scale, enableCorridors, offsets);
            //var mapDescriptions = GetInputsForThesis(false);
            //var benchmarkRunner = BenchmarkRunner.CreateForNodeType<int>();
            
            //var scenario = BenchmarkScenario.CreateForNodeType<int>(
            //    "Test", 
            //    input =>
            //    {
            //        var layoutGenerator = LayoutGeneratorFactory.GetDefaultChainBasedGenerator<int>();
            //        layoutGenerator.InjectRandomGenerator(new Random(0));
            //        layoutGenerator.SetLayoutValidityCheck(false);

            //        return layoutGenerator;
            //    });

            //var scenarioResult = benchmarkRunner.Run(scenario, mapDescriptions, 10);

            //var resultSaver = new BenchmarkResultSaver();
            //// await resultSaver.SaveAndUpload(scenarioResult, "name", "group");
            //resultSaver.SaveResult(scenarioResult);
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

  //      public static List<GeneratorInput<MapDescriptionOld<int>>> GetInputsForThesis(bool withCorridors)
		//{
		//	var path = "Resources/Maps/Thesis/";
		//	List<string> files;

		//	if (withCorridors)
		//	{
		//		files = new List<string>()
		//		{
		//			"backtracking_corridors",
		//			"generating_one_layout_corridors",
		//			"example1_corridors",
		//			"example2_corridors",
		//			"example3_corridors",
		//			"game1_corridors",
		//			"game2_corridors",
		//		};
		//	}
		//	else
		//	{
		//		files = new List<string>()
		//		{
		//			"backtracking_advanced",
		//			"generating_one_layout_advanced",
		//			"example1_advanced",
		//			"example2_advanced",
		//			"example3_advanced",
		//			"game1_basic",
		//			"game2_basic",
		//		};
		//	}

		//	var inputs = new List<GeneratorInput<MapDescriptionOld<int>>>();
		//	var configLoader = new ConfigLoader();

		//	foreach (var file in files)
		//	{
		//		var filename = $"{path}{file}.yml";

		//		using (var sr = new StreamReader(filename))
		//		{
		//			var mapDescription = configLoader.LoadMapDescription(sr);
		//			inputs.Add(new GeneratorInput<MapDescriptionOld<int>>(file, mapDescription));
		//		}
		//	}

		//	return inputs;
		//}

        public static List<DungeonGeneratorInput<int>> GetMapDescriptionsSet(IntVector2 scale, bool withCorridors, List<int> offsets, bool canTouch, RepeatMode? repeatModeOverride = null, BasicRoomDescription basicRoomDescription = null, string suffix = null)
        {
            var basicRoomTemplates = MapDescriptionUtils.GetBasicRoomTemplates(scale);
            basicRoomDescription = basicRoomDescription ?? new BasicRoomDescription(basicRoomTemplates);

            var corridorRoomTemplates = MapDescriptionUtils.GetCorridorRoomTemplates(offsets);
            var corridorRoomDescription = new CorridorRoomDescription(corridorRoomTemplates);

            var inputs = new List<DungeonGeneratorInput<int>>();

            {
                var mapDescription = MapDescriptionUtils.GetBasicMapDescription(GraphsDatabase.GetExample1(), basicRoomDescription, corridorRoomDescription, withCorridors);
                var configuration = new DungeonGeneratorConfiguration<int>()
                {
                    RoomsCanTouch = canTouch,
                    RepeatModeOverride = repeatModeOverride
                };
                inputs.Add(new DungeonGeneratorInput<int>(
                    MapDescriptionUtils.GetInputName("Example 1 (fig. 1)", scale, withCorridors, offsets, canTouch, suffix),
                    mapDescription,
                    configuration,
                    offsets
                ));
            }

            {
                var mapDescription = MapDescriptionUtils.GetBasicMapDescription(GraphsDatabase.GetExample2(), basicRoomDescription, corridorRoomDescription, withCorridors);
                var configuration = new DungeonGeneratorConfiguration<int>()
                {
                    RoomsCanTouch = canTouch,
                    RepeatModeOverride = repeatModeOverride
                };
                inputs.Add(new DungeonGeneratorInput<int>(
                    MapDescriptionUtils.GetInputName("Example 2 (fig. 7 top)", scale, withCorridors, offsets, canTouch, suffix),
                    mapDescription,
                    configuration,
                    offsets
                ));
            }

            {
                var mapDescription = MapDescriptionUtils.GetBasicMapDescription(GraphsDatabase.GetExample3(), basicRoomDescription, corridorRoomDescription, withCorridors);
                var configuration = new DungeonGeneratorConfiguration<int>()
                {
                    RoomsCanTouch = canTouch,
                    RepeatModeOverride = repeatModeOverride
                };
                inputs.Add(new DungeonGeneratorInput<int>(
                    MapDescriptionUtils.GetInputName("Example 3 (fig. 7 bottom)", scale, withCorridors, offsets, canTouch, suffix),
                    mapDescription,
                    configuration,
                    offsets
                ));
            }

            {
                var mapDescription = MapDescriptionUtils.GetBasicMapDescription(GraphsDatabase.GetExample4(), basicRoomDescription, corridorRoomDescription, withCorridors);
                var configuration = new DungeonGeneratorConfiguration<int>()
                {
                    RoomsCanTouch = canTouch, 
                    RepeatModeOverride = repeatModeOverride
                };
                inputs.Add(new DungeonGeneratorInput<int>(
                    MapDescriptionUtils.GetInputName("Example 4 (fig. 8)", scale, withCorridors, offsets, canTouch, suffix),
                    mapDescription,
                    configuration,
                    offsets
                ));
            }

            {
                var mapDescription = MapDescriptionUtils.GetBasicMapDescription(GraphsDatabase.GetExample5(), basicRoomDescription, corridorRoomDescription, withCorridors);
                var configuration = new DungeonGeneratorConfiguration<int>()
                {
                    RoomsCanTouch = canTouch,
                    RepeatModeOverride = repeatModeOverride
                };
                inputs.Add(new DungeonGeneratorInput<int>(
                    MapDescriptionUtils.GetInputName("Example 5 (fig. 9)", scale, withCorridors, offsets, canTouch, suffix),
                    mapDescription,
                    configuration,
                    offsets
                ));
            }

            return inputs;
        }
    }
}
