using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GeneralAlgorithms.DataStructures.Common;
using MapGeneration.Benchmarks;
using MapGeneration.Benchmarks.ResultSaving;
using MapGeneration.Utils;
using Newtonsoft.Json;

namespace Sandbox.Features
{
    public class TwoStageGeneration
    {
        public void Run()
        {
            RunBenchmarkAndCompare();
        }

        private void RunBenchmarkAndCompare()
        {
            var scale = new IntVector2(1, 1);
            var offsets = new List<int>() { 2 };

            var mapDescriptions = Program.GetMapDescriptionsSet(scale, true, offsets);
            mapDescriptions.AddRange(Program.GetMapDescriptionsSet(scale, false, offsets));

            var benchmarkRunner = BenchmarkRunner.CreateForNodeType<int>();

            var scenario = BenchmarkScenario.CreateForNodeType<int>(
                "TwoStage",
                input =>
                {
                    if (input.MapDescription.IsWithCorridors)
                    {
                        var layoutGenerator = LayoutGeneratorFactory.GetChainBasedGeneratorWithCorridors<int>(offsets, false);
                        layoutGenerator.InjectRandomGenerator(new Random(0));
                        layoutGenerator.SetLayoutValidityCheck(false);

                        return layoutGenerator;
                    }
                    else
                    {
                        var layoutGenerator = LayoutGeneratorFactory.GetDefaultChainBasedGenerator<int>();
                        layoutGenerator.InjectRandomGenerator(new Random(0));
                        layoutGenerator.SetLayoutValidityCheck(false);

                        return layoutGenerator;
                    }
                });

            var scenarioResult = benchmarkRunner.Run(scenario, mapDescriptions, 10);

            var resultSaver = new BenchmarkResultSaver();
            // await resultSaver.SaveAndUpload(scenarioResult, "name", "group");
            resultSaver.SaveResult(scenarioResult);

            CompareToReference(scenarioResult);
        }

        private void CompareToReference(BenchmarkScenarioResult scenarioResult)
        {
            Console.WriteLine();
            Console.WriteLine("Compare to reference result");

            var referenceResultText = File.ReadAllText("BenchmarkResults/1573636758_TwoStage_Reference.json");
            var referenceResult = JsonConvert.DeserializeObject<BenchmarkScenarioResult>(referenceResultText);
            var allEqual = true;

            foreach (var inputResult in scenarioResult.InputResults)
            {
                var referenceInputResult =
                    referenceResult.InputResults.Single(x => x.InputName == inputResult.InputName);
                var runsEqual = RunsEqual(inputResult.Runs, referenceInputResult.Runs);

                Console.WriteLine($"{inputResult.InputName} - equal: {runsEqual}");

                if (!runsEqual)
                {
                    allEqual = false;
                }
            }

            var originalColor = Console.ForegroundColor;
            Console.WriteLine();
            Console.ForegroundColor = allEqual ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed;
            Console.WriteLine($"All equal: {allEqual}");
            Console.ForegroundColor = originalColor;
        }

        private bool RunsEqual(IList<GeneratorRun> runs1, IList<GeneratorRun> runs2)
        {
            if (runs1.Count != runs2.Count)
            {
                return false;
            }

            for (int i = 0; i < runs1.Count; i++)
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