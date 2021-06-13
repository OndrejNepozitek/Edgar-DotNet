using System;
using System.Collections.Generic;
using System.IO;
using Edgar.Benchmarks;
using Edgar.Benchmarks.AdditionalData;
using Edgar.Benchmarks.GeneratorRunners;
using Edgar.Benchmarks.Legacy;
using Edgar.Benchmarks.Legacy.ResultSaving;
using Edgar.GraphBasedGenerator.Common;
using Edgar.Legacy.Core.LayoutEvolvers.SimulatedAnnealing;
using Edgar.Legacy.Core.LayoutGenerators.DungeonGenerator;
using Edgar.Legacy.Core.LayoutGenerators.PlatformersGenerator;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.Core.MapDescriptions.Interfaces;
using Edgar.Legacy.Utils.MapDrawing;
using Newtonsoft.Json;
using Sandbox.Utils;

namespace Sandbox.Features
{
    public class PlatformersFeature
    {
        public void Run()
        {
            var inputs = new List<GeneratorInput<IMapDescription<int>>>();


            var settings = new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                TypeNameHandling = TypeNameHandling.All,
            };

            inputs.Add(new GeneratorInput<IMapDescription<int>>(
                "DeadCells Ramparts",
                JsonConvert.DeserializeObject<MapDescription<int>>(
                    File.ReadAllText("Resources/MapDescriptions/deadCells_ramparts.json"), settings)));
            inputs.Add(new GeneratorInput<IMapDescription<int>>(
                "DeadCells Underground",
                JsonConvert.DeserializeObject<MapDescription<int>>(
                    File.ReadAllText("Resources/MapDescriptions/deadCells_underground.json"), settings)));

            var layoutDrawer = new SVGLayoutDrawer<int>();

            var benchmarkRunner = new BenchmarkRunnerLegacy<IMapDescription<int>>();
            var benchmarkScenario = new BenchmarkScenario<IMapDescription<int>>("Platformers", input =>
            {
                var configuration = new DungeonGeneratorConfiguration<int>()
                {
                    RepeatModeOverride = RoomTemplateRepeatMode.NoImmediate,
                    ThrowIfRepeatModeNotSatisfied = true,
                    SimulatedAnnealingConfiguration = new SimulatedAnnealingConfigurationProvider(new SimulatedAnnealingConfiguration()
                    {
                        HandleTreesGreedily = true,
                    })
                };
                // var layoutGenerator = new PlatformersGenerator<int>(input.MapDescription, configuration);
                var layoutGenerator = new PlatformersGenerator<int>(input.MapDescription, configuration);
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
                        // GeneratedLayout = layout,
                    };

                    var generatorRun = new GeneratorRun<AdditionalRunData>(layout != null, layoutGenerator.TimeTotal, layoutGenerator.IterationsCount, additionalData);

                    return generatorRun;
                });
            });

            var scenarioResult = benchmarkRunner.Run(benchmarkScenario, inputs, 200);
            var resultSaver = new BenchmarkResultSaver();
            resultSaver.SaveResultDefaultLocation(scenarioResult);
        }
    }
}