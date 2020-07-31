using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GeneralAlgorithms.Algorithms.Common;
using GeneralAlgorithms.Algorithms.Polygons;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Graphs;
using GeneralAlgorithms.DataStructures.Polygons;
using GUI;
using MapGeneration.Benchmarks;
using MapGeneration.Benchmarks.AdditionalData;
using MapGeneration.Benchmarks.GeneratorRunners;
using MapGeneration.Benchmarks.Interfaces;
using MapGeneration.Benchmarks.ResultSaving;
using MapGeneration.Core.Doors.DoorModes;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.MapDescriptions.Interfaces;
using MapGeneration.MetaOptimization.Evolution.DungeonGeneratorEvolution;
using MapGeneration.MetaOptimization.Stats;
using MapGeneration.MetaOptimization.Visualizations;
using MapGeneration.Utils;
using MapGeneration.Utils.MapDrawing;
using MapGeneration.Utils.Statistics;
using Newtonsoft.Json;
using Sandbox.Utils;

namespace Sandbox.Features
{
    public class CorridorConfigurationSpaces
    {
        private void ShowVisualization()
        {
            var input = Program.GetMapDescriptionsSet(new Vector2Int(1, 1), true, new List<int>() {2, 4, 6}, false)[4];

            var layoutGenerator = new DungeonGenerator<int>(input.MapDescription, input.Configuration);
            layoutGenerator.InjectRandomGenerator(new Random(0));

            var settings = new GeneratorSettings
            {
                LayoutGenerator = layoutGenerator,

                NumberOfLayouts = 10,

                ShowPartialValidLayouts = false,
                ShowPartialValidLayoutsTime = 500,

                ShowPerturbedLayouts = true,
                ShowPerturbedLayoutsTime = 1000,

                ShowFinalLayouts = true,
            };

            Application.Run(new GeneratorWindow(settings));
        }

        public void Run()
        {
            //ShowDifferentCorridorTypes();
            //return;

            //ShowVisualization();
            //return;

            var inputs = new List<DungeonGeneratorInput<int>>();
            inputs.AddRange(Program.GetMapDescriptionsSet(new Vector2Int(1, 1), false, null, true));
            inputs.AddRange(Program.GetMapDescriptionsSet(new Vector2Int(1, 1), true, new List<int>() { 2 }, true));
            inputs.AddRange(Program.GetMapDescriptionsSet(new Vector2Int(1, 1), true, new List<int>() { 2 }, false));
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
                var layoutGenerator = new DungeonGenerator<int>(input.MapDescription, dungeonGeneratorInput.Configuration);
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

                    var additionalData = new AdditionalRunData()
                    {
                        SimulatedAnnealingEventArgs = simulatedAnnealingArgsContainer,
                        GeneratedLayoutSvg = layoutDrawer.DrawLayout(layout, 800, forceSquare: true),
                        GeneratedLayout = layout,
                    };

                    var generatorRun = new GeneratorRun<AdditionalRunData>(layout != null, layoutGenerator.TimeTotal, layoutGenerator.IterationsCount, additionalData);

                    return generatorRun;
                });
            });

            var scenarioResult = benchmarkRunner.Run(benchmarkScenario, inputs, 5);
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

            Utils.BenchmarkUtils.IsEqualToReference(scenarioResult, "BenchmarkResults/1580072563_CorridorConfigurationSpaces_Reference.json");
        }
    }
}