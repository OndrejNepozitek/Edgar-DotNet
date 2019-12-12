using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MapGeneration.Benchmarks;
using MapGeneration.Core.LayoutEvolvers;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Utils.PerformanceAnalysis;
using Newtonsoft.Json.Linq;

namespace MapGeneration.MetaOptimization.Evolution.SAConfigurationEvolution
{
    public class DataVisualization
    {
        private const int ColumnSize = -11;
        private const int FirstColumnSize = -25;
        private TextWriter writer;

        private List<Split> splits = new List<Split>()
        {
            new Split("Best 50%", 0, 0.5),
            new Split("Best 70-80%", 0.7, 0.8),
            new Split("Worst 50%", 0.5, 1),
            new Split("Worst 20%", 0.8, 1),
            new Split("Worst 10%", 0.9, 1),
            new Split("All", 0, 1),
        };

        public void Analyze(List<GeneratorRun<List<SimulatedAnnealingEventArgs>>> runs, TextWriter writer)
        {
            this.writer = writer;

            runs.Sort((x1, x2) => x1.Iterations.CompareTo(x2.Iterations));
            var runInfos = runs.Select(AnalyzeRun).ToList();

            PrintHeader();
            PrintRow("iterations average", runInfos.Select(x => (double)x.Run.Iterations).ToList());

            var chainsCount = runInfos.First().ChainInfos.Count;
            for (int i = 0; i < chainsCount; i++)
            {
                var attemptsOnSuccess = runInfos.Select(x => (double)x.ChainInfos[i].AttemptsOnSuccess).ToList();
                var failedRuns = runInfos.Select(x => (double)x.ChainInfos[i].FailedRuns).ToList();
                var randomRestarts = runInfos.Select(x => (double)x.ChainInfos[i].RandomRestarts).ToList();
                var outOfIterations = runInfos.Select(x => (double)x.ChainInfos[i].OutOfIterations).ToList();
                var iterations = runInfos.Select(x => (double)x.ChainInfos[i].Iterations).ToList();

                PrintRow();
                PrintRow($"chain {i}");
                PrintRow("attempts on success", attemptsOnSuccess);
                PrintRow("failed runs", failedRuns);
                PrintRow("random restarts", randomRestarts);
                PrintRow("out of iterations", outOfIterations);
                PrintRow("iterations", iterations);
            }
        }

        private void PrintHeader(string name = "")
        {
            writer.Write($"{name,FirstColumnSize} |");

            foreach (var split in splits)
            {
                writer.Write($" {split.Name,ColumnSize} |");
            }

            writer.WriteLine();
        }

        private void PrintRow(string name, List<double> values)
        {
            writer.Write($"{name,FirstColumnSize} |");

            foreach (var split in splits)
            {
                writer.Write($" {split.GetSplit(values).Average(),ColumnSize:F} |");
            }

            writer.WriteLine();
        }

        private void PrintRow(string name = "")
        {
            writer.Write($"{name,FirstColumnSize} |");

            foreach (var split in splits)
            {
                writer.Write($" {"",ColumnSize} |");
            }

            writer.WriteLine();
        }

        private class Split
        {
            public string Name { get; set; }

            public double Start { get; set; }

            public double End { get; set; }

            public Split(string name, double start, double end)
            {
                Name = name;
                Start = start;
                End = end;
            }

            public IEnumerable<double> GetSplit(List<double> values)
            {
                return values.Skip((int)(values.Count * Start)).Take((int)(values.Count * (End - Start)));
            }
        }

        private RunInfo AnalyzeRun(GeneratorRun<List<SimulatedAnnealingEventArgs>> run)
        {
            var runInfo = new RunInfo()
            {
                ChainInfos = new List<ChainInfo>(),
                Run = run,
            };

            var simulatedAnnealingEvents = run.AdditionalData;
            var chainsCount = simulatedAnnealingEvents.Max(x => x.ChainNumber) + 1;

            for (int i = 0; i < chainsCount; i++)
            {
                runInfo.ChainInfos.Add(AnalyzeChain(i, simulatedAnnealingEvents));
            }

            return runInfo;
        }

        private ChainInfo AnalyzeChain(int chainNumber, List<SimulatedAnnealingEventArgs> simulatedAnnealingEvents)
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

            return new ChainInfo()
            {
                AttemptsOnSuccess = attemptsOnSuccess,
                FailedRuns = failedRuns,
                RandomRestarts = randomRestarts,
                OutOfIterations = outOfIterations,
                Iterations = numberOfIterations,
            };
        }

        private class AdditionalData
        {
            public List<SimulatedAnnealingEventArgs> SimulatedAnnealingEventArgs { get; set; }
        }
    }
}