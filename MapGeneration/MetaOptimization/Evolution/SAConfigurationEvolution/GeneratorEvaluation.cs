using System;
using System.Collections.Generic;
using System.Linq;
using MapGeneration.Benchmarks;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Interfaces.Benchmarks;
using MapGeneration.MetaOptimization.Stats;

namespace MapGeneration.MetaOptimization.Evolution.SAConfigurationEvolution
{
    public class GeneratorEvaluation : IGeneratorEvaluation<GeneratorData>
    {
        private readonly List<GeneratorData> generatorData;

        public GeneratorEvaluation(List<IGeneratorRun<AdditionalRunData>> generatorRuns)
        {
            generatorData = generatorRuns
                .Where(x => x.IsSuccessful)
                .Select(AnalyzeRun)
                .ToList();
            generatorData.Sort((x1, x2) => x1.Iterations.CompareTo(x2.Iterations));
        }

        public GeneratorData GetAverageStatistics(DataSplit dataSplit)
        {
            var data = generatorData
                .Skip(Math.Min(generatorData.Count - 1, (int)(generatorData.Count * dataSplit.Start)))
                .Take(Math.Max(1, (int)(generatorData.Count * (dataSplit.End - dataSplit.Start))))
                .ToList();
            var averageData = new GeneratorData();

            for (int i = 0; i < data[0].ChainsStats.Count; i++)
            {
                averageData.ChainsStats.Add(new ChainStats()
                {
                    AttemptsOnSuccess = data.Average(x => x.ChainsStats[i].AttemptsOnSuccess),
                    FailedRuns = data.Average(x => x.ChainsStats[i].FailedRuns),
                    RandomRestarts = data.Average(x => x.ChainsStats[i].RandomRestarts),
                    OutOfIterations = data.Average(x => x.ChainsStats[i].OutOfIterations),
                    Iterations = data.Average(x => x.ChainsStats[i].Iterations),
                    MaxIterationsOnSuccess = data.Max(x => x.ChainsStats[i].MaxIterationsOnSuccess),
                    AverageIterationsOnSuccess = data.Average(x => x.ChainsStats[i].AverageIterationsOnSuccess),
                });
            }

            averageData.Iterations = data.Average(x => x.Iterations);
            averageData.Time = data.Average(x => x.Time);

            return averageData;
        }

        private GeneratorData AnalyzeRun(IGeneratorRun<AdditionalRunData> run)
        {
            var generatorData = new GeneratorData()
            {
                Iterations = run.Iterations,
                Time = run.Time,
            };

            var simulatedAnnealingEvents = run.AdditionalData.SimulatedAnnealingEventArgs;
            var chainsCount = simulatedAnnealingEvents.Max(x => x.ChainNumber) + 1;

            for (int i = 0; i < chainsCount; i++)
            {
                generatorData.ChainsStats.Add(AnalyzeChain(i, simulatedAnnealingEvents));
            }

            return generatorData;
        }

        private ChainStats AnalyzeChain(int chainNumber, List<SimulatedAnnealingEventArgs> simulatedAnnealingEvents)
        {
            var attemptsOnSuccess = simulatedAnnealingEvents
                .Where(x => x.Type == SimulatedAnnealingEventType.LayoutGenerated)
                .ToList()
                .FindLast(x => x.ChainNumber == chainNumber)
                .LayoutsGenerated;
            var randomRestarts = simulatedAnnealingEvents
                .Count(x => x.Type == SimulatedAnnealingEventType.RandomRestart && x.ChainNumber == chainNumber);
            var outOfIterations = simulatedAnnealingEvents
                .Count(x => x.Type == SimulatedAnnealingEventType.OutOfIterations && x.ChainNumber == chainNumber);
            var failedRuns = outOfIterations + randomRestarts;
            var numberOfIterations = simulatedAnnealingEvents
                .Where(x => x.ChainNumber == chainNumber)
                .Sum(x => x.IterationsSinceLastEvent);
            var maxIterationsOnSuccess = simulatedAnnealingEvents
                .Where(x => x.ChainNumber == chainNumber && x.Type == SimulatedAnnealingEventType.LayoutGenerated)
                .Max(x => x.IterationsSinceLastEvent);
            var averageIterationsOnSuccess = simulatedAnnealingEvents
                .Where(x => x.ChainNumber == chainNumber && x.Type == SimulatedAnnealingEventType.LayoutGenerated)
                .Average(x => x.IterationsSinceLastEvent);

            return new ChainStats()
            {
                AttemptsOnSuccess = attemptsOnSuccess,
                FailedRuns = failedRuns,
                RandomRestarts = randomRestarts,
                OutOfIterations = outOfIterations,
                Iterations = numberOfIterations,
                MaxIterationsOnSuccess = maxIterationsOnSuccess,
                AverageIterationsOnSuccess = averageIterationsOnSuccess,
            };
        }

        private class AdditionalData
        {
            public List<SimulatedAnnealingEventArgs> SimulatedAnnealingEventArgs { get; set; }
        }
    }
}