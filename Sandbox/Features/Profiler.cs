using System;
using System.Collections.Generic;
using System.IO;
using MapGeneration.Benchmarks;
using MapGeneration.Benchmarks.AdditionalData;
using MapGeneration.Benchmarks.GeneratorRunners;
using MapGeneration.Benchmarks.ResultSaving;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using MapGeneration.Core.LayoutGenerators.PlatformersGenerator;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.MapDescriptions.Interfaces;
using MapGeneration.Utils.MapDrawing;
using Newtonsoft.Json;

namespace Sandbox.Features
{
    public class Profiler
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
                "Gungeon 1_1",
                JsonConvert.DeserializeObject<MapDescription<int>>(
                    File.ReadAllText("Resources/MapDescriptions/gungeon_1_1.json"), settings)));

            var layoutDrawer = new SVGLayoutDrawer<int>();

            var benchmarkRunner = new BenchmarkRunner<IMapDescription<int>>();
            var benchmarkScenario = new BenchmarkScenario<IMapDescription<int>>("Gungeon", input =>
            {
                var configuration = new DungeonGeneratorConfiguration<int>()
                {
                    RepeatModeOverride = RepeatMode.NoImmediate,
                    ThrowIfRepeatModeNotSatisfied = true,
                    SimulatedAnnealingConfiguration = new SimulatedAnnealingConfigurationProvider(new SimulatedAnnealingConfiguration()
                    {
                        HandleTreesGreedily = true,
                    })
                };
                // var layoutGenerator = new PlatformersGenerator<int>(input.MapDescription, configuration);


                //return new LambdaGeneratorRunner(() =>
                //{
                //    var layouts = layoutGenerator.GenerateLayout();

                //    return new GeneratorRun(layouts != null, layoutGenerator.TimeTotal, layoutGenerator.IterationsCount);
                //});
                return new LambdaGeneratorRunner(() =>
                {
                    var layoutGenerator = new DungeonGenerator<int>(input.MapDescription, configuration);
                    layoutGenerator.InjectRandomGenerator(new Random(0));

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