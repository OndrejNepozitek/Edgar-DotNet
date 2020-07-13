using System;
using System.Collections.Generic;
using System.IO;
using MapGeneration.Benchmarks;
using MapGeneration.Utils.Statistics;

namespace Sandbox.Features
{
    public class ThesisPlots
    {
        public void Run()
        {
            RunBoxPlot();
            // RunRandomGraphsStats();
        }

        public void RunRandomGraphsStats()
        {
            // var folder = @"1588669377_NumberOfEdges_edges_0_3";
            // var folder = @"1588712706_NumberOfEdges_1_edges_0_3";
            // var folder = @"1588712706_NumberOfEdges_3_edges_0_3";
            // var folder = @"1588848834_NumberOfEdges_1_edges_0_3";
            var folder = @"1588927280_NumberOfEdges_1_edges_0_3";
            // var folder = @"1588875883_NumberOfEdges_3_edges_0_3";
            var results = new List<List<BenchmarkScenarioResult>>();

            for (int i = 0; i <= 5; i++)
            {
                var edges = $"e_{i}_{i}";
                var resultsEdges = new List<BenchmarkScenarioResult>();
                results.Add(resultsEdges);

                for (int j = 1; j <= 4; j++)
                {
                    var vertices = $"v_{10 * j}_{10 * j + 9}";
                    var resultName = $"{edges}_{vertices}";
                    var result = LoadResult(folder, resultName);
                    resultsEdges.Add(result);
                }
            }

            RandomGraphStatistics.PrintAverageTime(results, false, true, true);
            RandomGraphStatistics.PrintSuccessRate(results, true);
        }

        public void RunBoxPlot()
        {
            // var folder = @"1586506670_OneImprovementDisabled";
            var folder = @"1590239906_OneImprovementEnabled";
            // var folder = @"1590241123_OneImprovementDisabled";
            var excludeOutliers = false;

            //var resultOld = LoadResult(folder, "Old");
            //var resultNew = LoadResult(folder, "ChainDecomposition");

            //var resultOld = LoadResult(folder, "Old");
            //var resultNew = LoadResult(folder, "New");
            //var resultWithoutChainDecomposition = LoadResult(folder, "WithoutChainDecomposition");
            //var resultWithoutGreedyTrees = LoadResult(folder, "WithoutGreedyTrees");
            //var resultWithoutMaxIterations = LoadResult(folder, "WithoutMaxIterations");

            var resultOld = LoadResult(folder, "Old");
            var resultNew = LoadResult(folder, "New");
            var resultWithoutChainDecomposition = LoadResult(folder, "ChainDecomposition");
            var resultWithoutGreedyTrees = LoadResult(folder, "GreedyTrees");
            var resultWithoutMaxIterations = LoadResult(folder, "MaxIterations");

            //OutputPlot(BoxPlotHelper.GetBoxPlotValues(resultWithoutChainDecomposition, resultOld, excludeOutliers));
            //OutputPlot(BoxPlotHelper.GetBoxPlotValues(resultWithoutGreedyTrees, resultOld, excludeOutliers));
            //OutputPlot(BoxPlotHelper.GetBoxPlotValues(resultWithoutMaxIterations, resultOld, excludeOutliers));
            //OutputPlot(BoxPlotHelper.GetBoxPlotValues(resultNew, resultOld, excludeOutliers));

            var differences = BoxPlotHelper.GetTimeDifferences(resultNew, resultOld);
            for (int i = 0; i < differences.Count; i++)
            {
                Console.WriteLine($"{i} & {differences[i]} \\\\");
            }

            Console.WriteLine();

            differences.Sort();
            differences.Reverse();
            for (int i = 0; i < differences.Count; i++)
            {
                Console.WriteLine($"{i} & {differences[i]} \\\\");
            }

            Console.WriteLine();

            for (int i = 0; i < differences.Count; i++)
            {
                if (i % 2 == 0)
                {
                    Console.WriteLine($"{i} & {differences[i]} \\\\");
                }
            }

            Console.WriteLine();

            for (int i = 0; i < differences.Count; i++)
            {
                if (i % 2 == 1)
                {
                    Console.WriteLine($"{i} & {differences[i]} \\\\");
                }
            }
        }

        private void OutputPlot(BoxPlotValues values)
        {
            Console.WriteLine(@"    \addplot+[");
            Console.WriteLine(@"    boxplot prepared={");
            Console.WriteLine($"        median={values.Median},");
            Console.WriteLine($"        upper quartile={values.Q3},");
            Console.WriteLine($"        lower quartile={values.Q1},");
            Console.WriteLine($"        upper whisker={values.Maximum},");
            Console.WriteLine($"        lower whisker={values.Minimum},");
            Console.WriteLine(@"    },");
            Console.WriteLine(@"    ] coordinates {};");
        }

        private BenchmarkScenarioResult LoadResult(string folder, string name)
        {
            var file = $"{folder}_{name}.json";
            var path = Path.Combine(@"D:\ProceduralLevelGenerator-data\Random", folder, file);

            return Utils.BenchmarkUtils.LoadBenchmarkScenarioResult(path);
        }
    }
}