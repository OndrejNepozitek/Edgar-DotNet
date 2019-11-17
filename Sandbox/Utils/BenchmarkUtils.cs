using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MapGeneration.Benchmarks;
using MapGeneration.Benchmarks.ResultSaving;
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

            resultSaver.SaveResult(scenarioResult, $"{group}_{name}");

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

            foreach (var inputResult in scenarioResult.InputResults)
            {
                var referenceInputResult =
                    referenceResult.InputResults.Single(x => x.InputName == inputResult.InputName);
                var runsEqual = RunsEqual(inputResult.Runs, referenceInputResult.Runs);

                var averageTime = inputResult.Runs.Average(x => x.Time);
                var referenceAverageTime = referenceInputResult.Runs.Average(x => x.Time);

                if (withConsole)
                {
                    Console.WriteLine($"{inputResult.InputName} - equal: {runsEqual}, time average actual/ref {averageTime / 1000:##.00}s/{referenceAverageTime / 1000:##.00}s");
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

        private static bool RunsEqual(IList<GeneratorRun> runs1, IList<GeneratorRun> runs2)
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