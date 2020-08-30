using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Edgar.Legacy.Benchmarks;
using Edgar.Legacy.Benchmarks.GeneratorRunners;
using Edgar.Legacy.Benchmarks.Interfaces;
using Edgar.Legacy.Benchmarks.ResultSaving;
using Edgar.Legacy.Core.LayoutEvolvers.SimulatedAnnealing;
using Edgar.Legacy.Core.LayoutGenerators.DungeonGenerator;
using Edgar.Legacy.Core.MapDescriptions.Interfaces;
using Edgar.Legacy.Core.MapLayouts;
using Edgar.Legacy.Utils.MapDrawing;
using Edgar.Legacy.Utils.MetaOptimization.Mutations;
using Edgar.Legacy.Utils.MetaOptimization.Visualizations;
using Edgar.Legacy.Utils.Statistics;

namespace Edgar.Legacy.Utils.MetaOptimization.Evolution.DungeonGeneratorEvolution
{
    public class DungeonGeneratorEvolution<TNode> : ConfigurationEvolution<DungeonGeneratorConfiguration<TNode>, Individual<TNode>>
    {
        private readonly IMapDescription<TNode> mapDescription;

        private readonly BenchmarkRunner<IMapDescription<TNode>> benchmarkRunner = new BenchmarkRunner<IMapDescription<TNode>>();
        private readonly SVGLayoutDrawer<TNode> layoutDrawer = new SVGLayoutDrawer<TNode>();
        private readonly EntropyCalculator entropyCalculator = new EntropyCalculator();
        private readonly LayoutsClustering<MapLayout<TNode>> layoutsClustering = new LayoutsClustering<MapLayout<TNode>>();

        public DungeonGeneratorEvolution(
            IMapDescription<TNode> mapDescription,
            List<IPerformanceAnalyzer<DungeonGeneratorConfiguration<TNode>, Individual<TNode>>> analyzers, EvolutionOptions options, string resultsDirectory)
            : base(analyzers, options, resultsDirectory)
        {
            this.mapDescription = mapDescription;
        }

        private static string GetResultsDirectory(GeneratorInput<IMapDescription<int>> generatorInput)
        {
            return $"DungeonGeneratorEvolutions/{new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()}_{generatorInput.Name}/";
        }

        protected override Individual<TNode> EvaluateIndividual(Individual<TNode> individual)
        {
            Logger.WriteLine($"Evaluating individual {individual}");

            var scenario = new BenchmarkScenario<IMapDescription<TNode>>("SimulatedAnnealingParameters",
                input =>
                {
                    // Setup early stopping
                    if (individual.Parent != null)
                    {
                        individual.Configuration.EarlyStopIfIterationsExceeded = Math.Min(50 * (int) individual.Parent.Iterations, InitialIndividual.Configuration.EarlyStopIfIterationsExceeded ?? int.MaxValue);
                    }
                    else
                    {
                        // TODO: how to do this?
                        // individual.Configuration.EarlyStopIfIterationsExceeded = null;
                    }

                    var layoutGenerator = new DungeonGenerator<TNode>(input.MapDescription, individual.Configuration);
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

                        var additionalData = new AdditionalRunData<TNode>()
                        {
                            GeneratedLayout = layout,
                            SimulatedAnnealingEventArgs = simulatedAnnealingArgsContainer,
                        };

                        var generatorRun = new GeneratorRun<AdditionalRunData<TNode>>(layout != null, layoutGenerator.TimeTotal, layoutGenerator.IterationsCount, additionalData);

                        return generatorRun;
                    });

                    if (individual.Parent != null)
                    {
                        return new EarlyStoppingGeneratorRunner(generatorRunner, individual.Parent.Iterations, (successful, time, iterations) => new GeneratorRun<AdditionalRunData<TNode>>(successful, time, iterations, null));
                    }
                    else
                    {
                        return generatorRunner;
                    }
                });

            var scenarioResult = benchmarkRunner.Run(scenario, new List<GeneratorInput<IMapDescription<TNode>>>() { new GeneratorInput<IMapDescription<TNode>>("DungeonGeneratorEvolution", mapDescription) }, Options.EvaluationIterations, new BenchmarkOptions()
            {
                WithConsoleOutput = false,
                WithFileOutput = false,
            });
            var generatorRuns = scenarioResult
                .BenchmarkResults
                .First()
                .Runs
                .Cast<IGeneratorRun<AdditionalRunData<TNode>>>()
                .ToList();

            var generatorEvaluation = new GeneratorEvaluation<AdditionalRunData<TNode>>(generatorRuns); // TODO: ugly
            individual.ConfigurationEvaluation = generatorEvaluation;

            individual.Iterations = generatorRuns.Average(x => x.Iterations);
            individual.Time = generatorRuns.Average(x => x.Time);
            individual.Fitness = Options.FitnessType == FitnessType.Iterations ? individual.Iterations : individual.Time;

            individual.SuccessRate = generatorRuns.Count(x => x.IsSuccessful) / (double)generatorRuns.Count;

            var layouts = generatorRuns
                .Where(x => x.IsSuccessful)
                .Select(x => x.AdditionalData.GeneratedLayout)
                .ToList();

            var layoutsForClustering = layouts.Take(Math.Min(layouts.Count, 250)).ToList();

            var roomTemplatesEntropy = entropyCalculator.ComputeAverageRoomTemplatesEntropy(mapDescription, layouts);
            var averageRoomTemplateSize = LayoutsDistance.GetAverageRoomTemplateSize(mapDescription);
            var positionOnlyClusters = layoutsClustering.GetClusters(layoutsForClustering, LayoutsDistance.PositionOnlyDistance, averageRoomTemplateSize);
            var positionAndShapeClusters = layoutsClustering.GetClusters(layoutsForClustering, (x1, x2) => LayoutsDistance.PositionAndShapeDistance(x1, x2, averageRoomTemplateSize), averageRoomTemplateSize);

            var summary = "";

            var fitnessDifferenceParent = individual.Parent != null ? 
                StatisticsUtils.DifferenceToReference(individual, individual.Parent, x => x.Fitness) : 0;
            var fitnessDifferenceTotal =
                StatisticsUtils.DifferenceToReference(individual, InitialIndividual, x => x.Fitness);

            var iterationsDifferenceParent = individual.Parent != null ? 
                StatisticsUtils.DifferenceToReference(individual, individual.Parent, x => x.Iterations) : 0;
            var iterationsDifferenceTotal =
                StatisticsUtils.DifferenceToReference(individual, InitialIndividual, x => x.Iterations);

            var timeDifferenceParent = individual.Parent != null ? 
                StatisticsUtils.DifferenceToReference(individual, individual.Parent, x => x.Time) : 0;
            var timeDifferenceTotal =
                StatisticsUtils.DifferenceToReference(individual, InitialIndividual, x => x.Time);

            summary += $"  fitness {individual.Fitness:F}, {(fitnessDifferenceParent > 0 ? "+" : "")}{fitnessDifferenceParent:F}%, {(fitnessDifferenceTotal > 0 ? "+" : "")}{fitnessDifferenceTotal:F}% total  \n";
            summary += $"  iterations {individual.Iterations:F}, {(iterationsDifferenceParent > 0 ? "+" : "")}{iterationsDifferenceParent:F}%, {(iterationsDifferenceTotal > 0 ? "+" : "")}{iterationsDifferenceTotal:F}% total  \n";
            summary += $"  time {individual.Time:F}, {(timeDifferenceParent > 0 ? "+" : "")}{timeDifferenceParent:F}%, {(timeDifferenceTotal > 0 ? "+" : "")}{timeDifferenceTotal:F}% total  \n";
            summary += $"  entropy {roomTemplatesEntropy:F}, clusters {positionOnlyClusters.Count}/{positionAndShapeClusters.Count} \n";
            summary += $"  success rate {individual.SuccessRate * 100:F}%\n";


            Logger.WriteLine(summary);

            // Directory.CreateDirectory($"{ResultsDirectory}/{individual.Id}");

            for (int i = 0; i < generatorRuns.Count; i++)
            {
                var generatorRun = generatorRuns[i];

                if (generatorRun.IsSuccessful)
                {
                    var layout = generatorRun.AdditionalData.GeneratedLayout;
                    var svg = layoutDrawer.DrawLayout(layout, 800, forceSquare: true);
                    // File.WriteAllText($"{ResultsDirectory}/{individual.Id}/{i}.svg", svg);
                    generatorRun.AdditionalData.GeneratedLayout = null;
                    generatorRun.AdditionalData.GeneratedLayoutSvg = svg;
                }
            }

            var resultSaver = new BenchmarkResultSaver();
            resultSaver.SaveResultDefaultLocation(scenarioResult, $"{individual.Id}_benchmarkResults", ResultsDirectory, withDatetime: false);

            using (var file =
                new StreamWriter(Path.Combine(ResultsDirectory, $"{individual.Id}_visualization.txt")))
            {
                file.WriteLine(summary);

                if (generatorRuns.Any(x => x.IsSuccessful))
                {
                    var dataVisualization = new ChainStatsVisualization<GeneratorData>();
                    dataVisualization.Visualize(generatorEvaluation, file);
                }
            }

            return individual;
        }

        protected override Individual<TNode> CreateInitialIndividual(int id, DungeonGeneratorConfiguration<TNode> configuration)
        {
            return new Individual<TNode>(id, configuration);
        }

        protected override Individual<TNode> CreateIndividual(int id, Individual<TNode> parent, IMutation<DungeonGeneratorConfiguration<TNode>> mutation)
        {
            return new Individual<TNode>(id, parent, mutation);
        }
    }
}