using System;
using System.IO;
using MapGeneration.Benchmarks;
using MapGeneration.Utils.Statistics;

namespace Sandbox.Features
{
    public class ThesisPlots
    {
        public void Run()
        {
            var folder = @"1586506670_OneImprovementDisabled";
            var excludeOutliers = true;

            var resultOld = LoadResult(folder, "Old");
            var resultNew = LoadResult(folder, "New");
            var resultWithoutChainDecomposition = LoadResult(folder, "WithoutChainDecomposition");
            var resultWithoutGreedyTrees = LoadResult(folder, "WithoutGreedyTrees");
            var resultWithoutMaxIterations = LoadResult(folder, "WithoutMaxIterations");

            OutputPlot(BoxPlotHelper.GetBoxPlotValues(resultWithoutChainDecomposition, resultOld, excludeOutliers));
            OutputPlot(BoxPlotHelper.GetBoxPlotValues(resultWithoutGreedyTrees, resultOld, excludeOutliers));
            OutputPlot(BoxPlotHelper.GetBoxPlotValues(resultWithoutMaxIterations, resultOld, excludeOutliers));
            OutputPlot(BoxPlotHelper.GetBoxPlotValues(resultNew, resultOld, excludeOutliers));

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