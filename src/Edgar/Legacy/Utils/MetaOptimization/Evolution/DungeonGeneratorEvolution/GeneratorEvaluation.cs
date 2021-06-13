﻿using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Benchmarks.Interfaces;
using Edgar.Legacy.Core.LayoutEvolvers.Interfaces;
using Edgar.Legacy.Core.LayoutEvolvers.SimulatedAnnealing;
using Edgar.Legacy.Utils.MetaOptimization.Stats;

namespace Edgar.Legacy.Utils.MetaOptimization.Evolution.DungeonGeneratorEvolution
{
    public class GeneratorEvaluation<TRunData> : IGeneratorEvaluation<GeneratorData>
        where TRunData : ISimulatedAnnealingData
    {
        private readonly List<GeneratorData> generatorData;

        public GeneratorEvaluation(List<IGeneratorRun<TRunData>> generatorRuns)
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
                    AverageStageTwoFailuresOnSuccess = data.Average(x => x.ChainsStats[i].AverageStageTwoFailuresOnSuccess),
                    MaxStageTwoFailuresOnSuccess = data.Max(x => x.ChainsStats[i].MaxStageTwoFailuresOnSuccess),
                });
            }

            averageData.Iterations = data.Average(x => x.Iterations);
            averageData.Time = data.Average(x => x.Time);

            return averageData;
        }

        private GeneratorData AnalyzeRun(IGeneratorRun<TRunData> run)
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
                .Where(x => x.ResetsIterationsSinceLastEvent)
                .Where(x => x.ChainNumber == chainNumber)
                .Sum(x => x.IterationsSinceLastEvent);
            var maxIterationsOnSuccess = simulatedAnnealingEvents
                .Where(x => x.ChainNumber == chainNumber && x.Type == SimulatedAnnealingEventType.LayoutGenerated)
                .Max(x => x.IterationsSinceLastEvent);
            var averageIterationsOnSuccess = simulatedAnnealingEvents
                .Where(x => x.ChainNumber == chainNumber && x.Type == SimulatedAnnealingEventType.LayoutGenerated)
                .Average(x => x.IterationsSinceLastEvent);

            var stageTwoFailuresBeforeSuccess = GetStageTwoFailuresBeforeSuccess(chainNumber, simulatedAnnealingEvents);
            var averageStageTwoFailuresOnSuccess = stageTwoFailuresBeforeSuccess
                .Average(x => x.Count);
            var maxStageTwoFailuresOnSuccess = stageTwoFailuresBeforeSuccess
                .Max(x => x.Count);

            return new ChainStats()
            {
                AttemptsOnSuccess = attemptsOnSuccess,
                FailedRuns = failedRuns,
                RandomRestarts = randomRestarts,
                OutOfIterations = outOfIterations,
                Iterations = numberOfIterations,
                MaxIterationsOnSuccess = maxIterationsOnSuccess,
                AverageIterationsOnSuccess = averageIterationsOnSuccess,
                AverageStageTwoFailuresOnSuccess = averageStageTwoFailuresOnSuccess,
                MaxStageTwoFailuresOnSuccess = maxStageTwoFailuresOnSuccess,
            };
        }

        private List<List<SimulatedAnnealingEventArgs>> GetStageTwoFailuresBeforeSuccess(int chainNumber, List<SimulatedAnnealingEventArgs> simulatedAnnealingEvents)
        {
            var result = new List<List<SimulatedAnnealingEventArgs>>();

            var stageTwoFailures = new List<SimulatedAnnealingEventArgs>();
            foreach (var simulatedAnnealingEvent in simulatedAnnealingEvents)
            {
                if (simulatedAnnealingEvent.Type == SimulatedAnnealingEventType.LayoutGenerated)
                {
                    result.Add(stageTwoFailures);
                    stageTwoFailures = new List<SimulatedAnnealingEventArgs>();
                } else if (simulatedAnnealingEvent.ChainNumber != chainNumber && stageTwoFailures.Count != 0)
                {
                    stageTwoFailures = new List<SimulatedAnnealingEventArgs>();
                }

                if (simulatedAnnealingEvent.ChainNumber == chainNumber &&
                    simulatedAnnealingEvent.Type == SimulatedAnnealingEventType.StageTwoFailure)
                {
                    stageTwoFailures.Add(simulatedAnnealingEvent);
                }
            }

            return result;
        }

        private class AdditionalData
        {
            public List<SimulatedAnnealingEventArgs> SimulatedAnnealingEventArgs { get; set; }
        }
    }
}