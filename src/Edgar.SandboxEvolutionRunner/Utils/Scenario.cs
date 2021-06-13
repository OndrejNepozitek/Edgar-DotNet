﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Edgar.Benchmarks;
using Edgar.Benchmarks.GeneratorRunners;
using Edgar.Benchmarks.Interfaces;
using Edgar.Benchmarks.Legacy;
using Edgar.Benchmarks.Legacy.ResultSaving;
using Edgar.Legacy.Core.LayoutEvolvers.SimulatedAnnealing;
using Edgar.Legacy.Core.LayoutGenerators.DungeonGenerator;
using Edgar.Legacy.Core.MapDescriptions.Interfaces;
using Edgar.Legacy.Utils;
using Edgar.Legacy.Utils.Logging;
using Edgar.Legacy.Utils.Logging.Handlers;
using Edgar.Legacy.Utils.MapDrawing;
using Edgar.Legacy.Utils.MetaOptimization.Evolution.DungeonGeneratorEvolution;
using Newtonsoft.Json;
using SandboxEvolutionRunner.Evolution;

namespace SandboxEvolutionRunner.Utils
{
    public abstract class Scenario
    {
        protected Options Options;
        protected string DirectoryFullPath;
        protected string Directory;
        protected Logger Logger;
        protected MapDescriptionLoader MapDescriptionLoader;

        protected virtual List<List<int>> GetCorridorOffsets()
        {
            return Options.CorridorOffsets.Select(x => x.Split(",").Select(int.Parse).ToList()).ToList();
        }

        public virtual List<NamedMapDescription> GetMapDescriptions(List<NamedGraph> namedGraphs = null)
        {
            return MapDescriptionLoader.GetMapDescriptions(namedGraphs);
        }

        protected virtual BenchmarkScenarioResult RunBenchmark(IEnumerable<DungeonGeneratorInput<int>> inputs, int iterations, string name)
        {
            var benchmarkRunner = new BenchmarkRunnerLegacy<IMapDescription<int>>();
            var benchmarkScenario = new BenchmarkScenario<IMapDescription<int>>(name, GetGeneratorRunnerFactory);

            var resultSaver = new BenchmarkResultSaver();

            var scenarioResult = benchmarkRunner.Run(benchmarkScenario, inputs, iterations, new BenchmarkOptions()
            {
                WithConsolePreview = Options.WithConsolePreview,
                MultiThreaded = Options.MaxThreads > 1,
                MaxDegreeOfParallelism = Options.MaxThreads,
                WithFileOutput = false,
            });
            resultSaver.SaveResultDefaultLocation(scenarioResult, directory: DirectoryFullPath, name: $"{Directory}_{name}", withDatetime: false);

            return scenarioResult;
        }

        protected virtual Task RunBenchmarkAsync(IEnumerable<DungeonGeneratorInput<int>> inputs, int iterations, string name)
        {
            var task = new Task(() => { RunBenchmark(inputs, iterations, name); }, TaskCreationOptions.LongRunning);
            task.Start();

            return task;
            return Task.Run(() => { RunBenchmark(inputs, iterations, name); });
        }

        protected virtual Task RunBenchmarkAsync(IEnumerable<NamedMapDescription> mapDescriptions, Func<NamedMapDescription, DungeonGeneratorConfiguration<int>> configurationFactory, int iterations, string name)
        {
            return RunBenchmarkAsync(mapDescriptions.Select(x => GetInput(x, configurationFactory)), iterations, name);
        }

        protected virtual BenchmarkScenarioResult RunBenchmark(IEnumerable<NamedMapDescription> mapDescriptions, Func<NamedMapDescription, DungeonGeneratorConfiguration<int>> configurationFactory, int iterations, string name)
        {
            return RunBenchmark(mapDescriptions.Select(x => GetInput(x, configurationFactory)), iterations, name);
        }

        protected virtual DungeonGeneratorConfiguration<int> GetBasicConfiguration(NamedMapDescription namedMapDescription)
        {
            return new DungeonGeneratorConfiguration<int>()
            {
                RoomsCanTouch = Options.CanTouch || !namedMapDescription.IsWithCorridors,
                EarlyStopIfIterationsExceeded = Options.EarlyStopIterations,
                EarlyStopIfTimeExceeded = Options.EarlyStopTime != null ? TimeSpan.FromMilliseconds(Options.EarlyStopTime.Value) : default(TimeSpan?), 
            };
        }

        protected virtual DungeonGeneratorInput<int> GetInput(NamedMapDescription namedMapDescription, Func<NamedMapDescription, DungeonGeneratorConfiguration<int>> configurationFactory)
        {
            return new DungeonGeneratorInput<int>(namedMapDescription.Name, namedMapDescription.MapDescription, configurationFactory(namedMapDescription));
        }

        protected virtual IGeneratorRunner GetGeneratorRunnerFactory(GeneratorInput<IMapDescription<int>> input)
        {
            var layoutDrawer = new SVGLayoutDrawer<int>();

            var dungeonGeneratorInput = (DungeonGeneratorInput<int>) input;
            var layoutGenerator = new DungeonGenerator<int>(input.MapDescription, dungeonGeneratorInput.Configuration);
            layoutGenerator.InjectRandomGenerator(new Random(0));

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
                    GeneratedLayoutSvg = layout != null ? layoutDrawer.DrawLayout(layout, 800, forceSquare: true) : null,
                    GeneratedLayout = layout,
                };

                var generatorRun = new GeneratorRun<AdditionalRunData<int>>(layout != null, layoutGenerator.TimeTotal, layoutGenerator.IterationsCount, additionalData);

                return generatorRun;
            });
        }

        public virtual void Run(Options options)
        {
            Options = options;
            Directory = FileNamesHelper.PrefixWithTimestamp(Options.Name);
            DirectoryFullPath = Path.Combine("DungeonGeneratorEvolutions", Directory);
            System.IO.Directory.CreateDirectory(DirectoryFullPath);
            Logger = new Logger(new ConsoleLoggerHandler(), new FileLoggerHandler(Path.Combine(DirectoryFullPath, "log.txt")));
            MapDescriptionLoader = new BetterMapDescriptionLoader(options, Options.RoomTemplatesSet);

            Run();
        }

        protected abstract void Run();
    }
}