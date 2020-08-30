using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Legacy.Benchmarks;

namespace Edgar.Legacy.Utils.Statistics
{
    public class RandomGraphStatistics
    {
        private const int ColumnSize = 13;
        private const double successThreshold = 0.95;
        private const int MinimumNumberOfResults = 10;

        public static void PrintAverageTime(List<List<BenchmarkScenarioResult>> results, bool onlySuccessful, bool showLatexCode, bool onlySuccessfulRuns)
        {
            Console.WriteLine();
            Console.WriteLine("Average time");
            PrintStats(results, result =>
            {
                var successfulResults = result.BenchmarkResults.Where(x => !onlySuccessful || IsSuccessful(x)).ToList();

                if (successfulResults.Count < MinimumNumberOfResults)
                {
                    return double.NaN;
                }

                return successfulResults.Where(x => x.Runs.Any(run => !onlySuccessfulRuns || run.IsSuccessful)).Average(x => x.Runs.Where(run => !onlySuccessfulRuns || run.IsSuccessful).Average(run => run.Time) / 1000);
            }, showLatexCode);
        }

        public static void PrintSuccessRate(List<List<BenchmarkScenarioResult>> results, bool showLatexCode)
        {
            Console.WriteLine();
            Console.WriteLine("Success rate");
            PrintStats(results, result =>
            {
                if (result.BenchmarkResults.Count < MinimumNumberOfResults)
                {
                    return double.NaN;
                }

                var successfulCount = result.BenchmarkResults.Count(IsSuccessful);
                return successfulCount / (double) result.BenchmarkResults.Count;
            }, showLatexCode);
        }

        private static bool IsSuccessful(BenchmarkResult result)
        {
            var successfulCount = result.Runs.Count(x => x.IsSuccessful);
            var ratio = successfulCount / (double) result.Runs.Count;
            return ratio > successThreshold;
        }

        public static void PrintStats(List<List<BenchmarkScenarioResult>> results, Func<BenchmarkScenarioResult, double> metric, bool showLatexCode)
        {
            {
                var edgesResults = results[0];
                Console.Write($"{"",ColumnSize}");

                for (int j = 1; j <= edgesResults.Count; j++)
                {
                    var vertices = $"{j * 10}-{j * 10 + 9} vert";

                    Console.Write($"{vertices,ColumnSize}");
                }

                Console.WriteLine();
            }

            for (int i = 0; i < results.Count; i++)
            {
                var edgesResults = results[i];
                var edges = i;

                Console.Write($"{edges + " edges",ColumnSize}");

                for (int j = 0; j < edgesResults.Count; j++)
                {
                    var result = edgesResults[j];
                    var vertices = $"{j}-{j + 9}";

                    var metricResult = metric(result);
                    Console.Write($"{metricResult,ColumnSize:F}");
                }

                Console.WriteLine();
            }

            Console.WriteLine();

            //for (int i = 0; i < results.Count; i++)
            //{
            //    var edgesResults = results[i];
            //    var edges = i;

            //    for (int j = 0; j < edgesResults.Count; j++)
            //    {
            //        var result = edgesResults[j];
            //        var metricResult = metric(result);

            //        Console.WriteLine($"{j} {i} {metricResult:F}");
            //    }
            //}

            if (showLatexCode)
            {
                for (int i = 0; i < results[0].Count; i++)
                {
                    for (int j = 0; j < results.Count; j++)
                    {
                        var result = results[j][i];
                        var metricResult = metric(result);

                        Console.WriteLine($"{i} {j} {metricResult:F}");
                    }

                    Console.WriteLine();
                }
            }

            Console.WriteLine();
        }
    }
}