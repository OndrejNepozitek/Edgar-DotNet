using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Edgar.Legacy.Utils.MetaOptimization.Stats;

namespace Edgar.Legacy.Utils.MetaOptimization.Visualizations
{
    public class ChainStatsVisualization<TGeneratorStats>
        where TGeneratorStats : IChainsStats, IBasicStats
    {
        private const int ColumnSize = -11;
        private const int FirstColumnSize = -30;
        private List<NamedData> splits;
        private TextWriter writer;

        public void Visualize(IGeneratorEvaluation<TGeneratorStats> generatorEvaluation, TextWriter writer)
        {
            this.writer = writer;
            splits = new List<NamedData>()
            {
                new NamedData("Best 50%", generatorEvaluation.GetAverageStatistics(new DataSplit(0, 0.5))),
                new NamedData("Best 70-80%", generatorEvaluation.GetAverageStatistics(new DataSplit(0.7, 0.8))),
                new NamedData("Worst 50%", generatorEvaluation.GetAverageStatistics(new DataSplit(0.5, 1))),
                new NamedData("Worst 20%", generatorEvaluation.GetAverageStatistics(new DataSplit(0.8, 1))),
                new NamedData("Worst 10%", generatorEvaluation.GetAverageStatistics(new DataSplit(0.9, 1))),
                new NamedData("All", generatorEvaluation.GetAverageStatistics(new DataSplit(0, 1))),
            };

            PrintHeader();
            PrintRow("iterations average", x => x.Iterations);

            var chainsCount = splits.First().Data.ChainsStats.Count;
            for (int i = 0; i < chainsCount; i++)
            {
                var chainNumber = i;

                PrintRow();
                PrintRow($"chain {chainNumber}");
                PrintRow("attempts on success", x => x.ChainsStats[chainNumber].AttemptsOnSuccess);
                PrintRow("failed runs", x => x.ChainsStats[chainNumber].FailedRuns);
                PrintRow("random restarts", x => x.ChainsStats[chainNumber].RandomRestarts);
                PrintRow("avg s2 failures on success", x => x.ChainsStats[chainNumber].AverageStageTwoFailuresOnSuccess);
                PrintRow("max s2 failures on success", x => x.ChainsStats[chainNumber].MaxStageTwoFailuresOnSuccess);
                PrintRow("out of iterations", x => x.ChainsStats[chainNumber].OutOfIterations);
                PrintRow("max iterations on success", x => x.ChainsStats[chainNumber].MaxIterationsOnSuccess);
                PrintRow("avg iterations on success", x => x.ChainsStats[chainNumber].AverageIterationsOnSuccess);
                PrintRow("iterations", x => x.ChainsStats[chainNumber].Iterations);
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

        private void PrintRow(string name, Func<TGeneratorStats, double> valueSelector)
        {
            writer.Write($"{name,FirstColumnSize} |");

            foreach (var split in splits)
            {
                writer.Write($" {valueSelector(split.Data),ColumnSize:F} |");
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

        private class NamedData
        {
            public TGeneratorStats Data { get; }

            public string Name { get; }

            public NamedData(string name, TGeneratorStats data)
            {
                Data = data;
                Name = name;
            }
        }
    }
}