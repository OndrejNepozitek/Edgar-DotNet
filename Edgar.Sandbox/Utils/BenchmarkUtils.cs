using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Edgar.Legacy.Benchmarks;
using Edgar.Legacy.Benchmarks.Interfaces;
using Edgar.Legacy.Benchmarks.ResultSaving;
using Newtonsoft.Json;

namespace Sandbox.Utils
{
    public static class BenchmarkUtils
    {
        public static async Task SaveAndUpload(this BenchmarkResultSaver resultSaver, BenchmarkScenarioResult scenarioResult, string name, string group)
        {
            if (resultSaver == null) throw new ArgumentNullException(nameof(resultSaver));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (group == null) throw new ArgumentNullException(nameof(group));

            resultSaver.SaveResultDefaultLocation(scenarioResult, $"{group}_{name}");

            var uploadConfig = GetDefaultUploadConfig();
            await resultSaver.UploadManualResult(scenarioResult, uploadConfig, new ManualInfo()
            {
                Group = group,
                Name = name,
            });
        }

        private static UploadConfig GetDefaultUploadConfig()
        {
            var url = File.ReadAllText("manualResultUrl.txt");

            return new UploadConfig(url);
        }

        public static BenchmarkScenarioResult LoadBenchmarkScenarioResult(string path)
        {
            var resultText = File.ReadAllText(path);
            var result = JsonConvert.DeserializeObject<BenchmarkScenarioResult>(resultText);

            return result;
        }

        public static bool IsEqualToReference(BenchmarkScenarioResult scenarioResult, string referencePath, bool withConsole = true)
        {
            var referenceResultText = File.ReadAllText(referencePath);
            var referenceResult = JsonConvert.DeserializeObject<BenchmarkScenarioResult>(referenceResultText);

            return IsEqualToReference(scenarioResult, referenceResult, withConsole);
        }

        private static bool IsEqualToReference(BenchmarkScenarioResult scenarioResult, BenchmarkScenarioResult referenceResult, bool withConsole = true)
        {
            if (withConsole)
            {
                Console.WriteLine();
                Console.WriteLine("Compare to reference result");
            }

            var allEqual = true;

            foreach (var benchmarkResult in scenarioResult.BenchmarkResults)
            {
                var referenceBenchmarkResult = referenceResult.BenchmarkResults.SingleOrDefault(x => x.InputName == benchmarkResult.InputName);

                if (referenceBenchmarkResult == null)
                {
                    Console.WriteLine($"{benchmarkResult.InputName} - equal: not found");
                    continue;
                }

                var runsEqual = RunsEqual(benchmarkResult.Runs.Cast<IGeneratorRun>().ToList(), referenceBenchmarkResult.Runs.Cast<IGeneratorRun>().ToList()); // TODO: ugly

                var averageTime = benchmarkResult.Runs.Average(x => x.Time);
                var referenceAverageTime = referenceBenchmarkResult.Runs.Average(x => x.Time);

                var averageIterations = benchmarkResult.Runs.Average(x => x.Iterations) / 1000;
                var referenceAverageIterations = referenceBenchmarkResult.Runs.Average(x => x.Iterations) / 1000;

                if (withConsole)
                {
                    Console.WriteLine($"{benchmarkResult.InputName} - equal: {runsEqual}, time average {referenceAverageTime / 1000:##.00}s -> {averageTime / 1000:##.00}s, iterations average {referenceAverageIterations:F}k -> {averageIterations:F}k");
                }
                
                if (!runsEqual)
                {
                    allEqual = false;
                }
            }

            if (withConsole)
            {
                var originalColor = Console.ForegroundColor;
                Console.WriteLine();
                Console.ForegroundColor = allEqual ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed;
                Console.WriteLine($"All equal: {allEqual}");
                Console.ForegroundColor = originalColor;
            }

            return allEqual;
        }

        private static bool RunsEqual(IList<IGeneratorRun> runs1, IList<IGeneratorRun> runs2)
        {
            if (runs1.Count == 0 || runs2.Count == 0)
            {
                throw new InvalidOperationException();
            }

            for (int i = 0; i < Math.Min(runs1.Count, runs2.Count); i++)
            {
                if (runs1[i].Iterations != runs2[i].Iterations)
                {
                    return false;
                }
            }

            return true;
        }
    }
}