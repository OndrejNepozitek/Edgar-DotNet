using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using MapGeneration.Benchmarks;
using MapGeneration.Benchmarks.GeneratorRunners;
using MapGeneration.Benchmarks.ResultSaving;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using MapGeneration.Interfaces.Benchmarks;
using MapGeneration.Interfaces.Core.MapDescriptions;
using MapGeneration.Interfaces.Core.MapLayouts;
using MapGeneration.MetaOptimization.Mutations;
using MapGeneration.MetaOptimization.Visualizations;
using MapGeneration.Utils.MapDrawing;
using MapGeneration.Utils.Statistics;

namespace MapGeneration.MetaOptimization.Evolution.DungeonGeneratorEvolution
{
    public class DungeonGeneratorEvolution : ConfigurationEvolution<DungeonGeneratorConfiguration, Individual>
    {
        private readonly GeneratorInput<IMapDescription<int>> generatorInput;

        private readonly BenchmarkRunner<IMapDescription<int>> benchmarkRunner = new BenchmarkRunner<IMapDescription<int>>();
        private readonly SVGLayoutDrawer<int> layoutDrawer = new SVGLayoutDrawer<int>();
        private readonly EntropyCalculator entropyCalculator = new EntropyCalculator();
        private readonly LayoutsClustering<IMapLayout<int>> layoutsClustering = new LayoutsClustering<IMapLayout<int>>();
        private readonly List<int> corridorOffsets;

        public DungeonGeneratorEvolution(
            GeneratorInput<IMapDescription<int>> generatorInput,
            List<IPerformanceAnalyzer<DungeonGeneratorConfiguration, Individual>> analyzers, EvolutionOptions options, List<int> corridorOffsets)
            : base(analyzers, options, GetResultsDirectory(generatorInput))
        {
            this.generatorInput = generatorInput;
            this.corridorOffsets = corridorOffsets;
        }

        private static string GetResultsDirectory(GeneratorInput<IMapDescription<int>> generatorInput)
        {
            return $"DungeonGeneratorEvolutions/{new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()}_{generatorInput.Name}/";
        }

        protected override Individual EvaluateIndividual(Individual individual)
        {
            Logger.Write($"Evaluating individual {individual}");

            var scenario = new BenchmarkScenario<IMapDescription<int>>("SimulatedAnnealingParameters",
                input =>
                {
                    // Setup early stopping
                    if (individual.Parent != null)
                    {
                        individual.Configuration.EarlyStopIfIterationsExceeded = 25 * (int) individual.Parent.Fitness;
                    }
                    else
                    {
                        individual.Configuration.EarlyStopIfIterationsExceeded = null;
                    }

                    var layoutGenerator = new DungeonGenerator<int>(input.MapDescription, individual.Configuration, corridorOffsets);
                    layoutGenerator.InjectRandomGenerator(new Random(0));

                    var generatorRunner = new LambdaGeneratorRunner(() =>
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
                            GeneratedLayout = layout,
                        };

                        var generatorRun = new GeneratorRun<AdditionalRunData>(layout != null, layoutGenerator.TimeTotal, layoutGenerator.IterationsCount, additionalData);

                        return generatorRun;
                    });

                    if (individual.Parent != null)
                    {
                        return new EarlyStoppingGeneratorRunner(generatorRunner, individual.Parent.Fitness, (successful, time, iterations) => new GeneratorRun<AdditionalRunData>(successful, time, iterations, null));
                    }
                    else
                    {
                        return generatorRunner;
                    }
                });

            var scenarioResult = benchmarkRunner.Run(scenario, new List<GeneratorInput<IMapDescription<int>>>() { generatorInput }, Options.EvaluationIterations, new BenchmarkOptions()
            {
                WithConsoleOutput = false,
            });
            var generatorRuns = scenarioResult
                .InputResults
                .First()
                .Runs
                .Cast<IGeneratorRun<AdditionalRunData>>()
                .ToList();

            var generatorEvaluation = new GeneratorEvaluation(generatorRuns); // TODO: ugly
            individual.ConfigurationEvaluation = generatorEvaluation;
            individual.Fitness = generatorRuns.Average(x => x.Iterations);
            individual.SuccessRate = generatorRuns.Count(x => x.IsSuccessful) / (double)generatorRuns.Count;

            var layouts = generatorRuns
                .Where(x => x.IsSuccessful)
                .Select(x => x.AdditionalData.GeneratedLayout)
                .ToList();

            var roomTemplatesEntropy = entropyCalculator.ComputeAverageRoomTemplatesEntropy(generatorInput.MapDescription, layouts);
            var averageRoomTemplateSize = LayoutsDistance.GetAverageRoomTemplateSize(generatorInput.MapDescription);
            var positionOnlyClusters = layoutsClustering.GetClusters(layouts, LayoutsDistance.PositionOnlyDistance, averageRoomTemplateSize);
            var positionAndShapeClusters = layoutsClustering.GetClusters(layouts, (x1, x2) => LayoutsDistance.PositionAndShapeDistance(x1, x2, averageRoomTemplateSize), averageRoomTemplateSize);

            Logger.WriteLine($" - fitness {individual.Fitness:F}, entropy {roomTemplatesEntropy:F}, clusters {positionOnlyClusters.Count}/{positionAndShapeClusters.Count}, success rate {individual.SuccessRate * 100:F}%");

            Directory.CreateDirectory($"{ResultsDirectory}/{individual.Id}");

            for (int i = 0; i < generatorRuns.Count; i++)
            {
                var generatorRun = generatorRuns[i];

                if (generatorRun.IsSuccessful)
                {
                    var layout = generatorRun.AdditionalData.GeneratedLayout;
                    var svg = layoutDrawer.DrawLayout(layout, 800);
                    File.WriteAllText($"{ResultsDirectory}/{individual.Id}/{i}.svg", svg);
                    generatorRun.AdditionalData.GeneratedLayout = null;
                    generatorRun.AdditionalData.GeneratedLayoutSvg = svg;
                }
            }

            var resultSaver = new BenchmarkResultSaver();
            resultSaver.SaveResult(scenarioResult, $"{individual.Id}_benchmarkResults", ResultsDirectory, withDatetime: false);

            using (var file =
                new StreamWriter($@"{ResultsDirectory}{individual.Id}_visualization.txt"))
            {
                if (generatorRuns.Any(x => x.IsSuccessful))
                {
                    var dataVisualization = new ChainStatsVisualization<GeneratorData>();
                    dataVisualization.Visualize(generatorEvaluation, file);
                }
            }

            return individual;
        }

        protected override Individual CreateInitialIndividual(int id, DungeonGeneratorConfiguration configuration)
        {
            return new Individual(id, configuration);
        }

        protected override Individual CreateIndividual(int id, Individual parent, IMutation<DungeonGeneratorConfiguration> mutation)
        {
            return new Individual(id, parent, mutation);
        }
    }
}