﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Edgar.GraphBasedGenerator;
using Edgar.SandboxEvolutionRunner.Benchmarks.GraphBasedGenerator.Generators;
using MapGeneration.Benchmarks;
using MapGeneration.Benchmarks.ResultSaving;
using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using MapGeneration.Core.MapDescriptions.Interfaces;
using MapGeneration.Utils;
using MapGeneration.Utils.Logging;
using MapGeneration.Utils.Logging.Handlers;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using SandboxEvolutionRunner.Evolution;
using SandboxEvolutionRunner.Utils;
using ScottPlot;

namespace Edgar.SandboxEvolutionRunner.Benchmarks.GraphBasedGenerator
{
    public abstract class Benchmark
    {
        protected string DirectoryFullPath;
        protected string Directory;
        protected Logger Logger;
        protected Options Options;
        
        public virtual void Run(Options options)
        {
            Options = options;
            Directory = FileNamesHelper.PrefixWithTimestamp(Options.Name);
            DirectoryFullPath = Path.Combine("DungeonGeneratorEvolutions", Directory);
            System.IO.Directory.CreateDirectory(DirectoryFullPath);
            Logger = new Logger(new ConsoleLoggerHandler(), new FileLoggerHandler(Path.Combine(DirectoryFullPath, "log.txt")));

            Run();
        }

        protected abstract void Run();

        protected OldGraphBasedGeneratorFactory<TNode> GetOldGenerator<TNode>(BenchmarkOptions options, bool withInit = false)
        {
            return new OldGraphBasedGeneratorFactory<TNode>(new DungeonGeneratorConfiguration<TNode>()
            {
                EarlyStopIfTimeExceeded = options.EarlyStopTime != null ? TimeSpan.FromMilliseconds(options.EarlyStopTime.Value) : default(TimeSpan?)
            }, withInit);
        }

        protected void RunBenchmark<TNode>(List<BenchmarkScenario<TNode>> scenarios, List<ILevelGeneratorFactory<TNode>> generators)
        {
            var results = new List<BenchmarkScenarioResult>();

            foreach (var benchmarkScenario in scenarios)
            {
                foreach (var levelGeneratorFactory in generators)
                {
                    var result = RunBenchmark(benchmarkScenario, levelGeneratorFactory, Options.FinalEvaluationIterations);
                    results.Add(result);
                }
            }

            PlotResults<TNode>("All", results);
        }

        protected void LoadFromFolder<TNode>()
        {
            var results = new List<BenchmarkScenarioResult>();

            foreach (var path in System.IO.Directory.GetFiles(@"C:\Users\ondra\OneDrive\Dokumenty\Unity\Benchmarks\DungeonGeneratorEvolutions\1596449602_"))
            {
                var resultText = File.ReadAllText(path);
                var result = JsonConvert.DeserializeObject<BenchmarkScenarioResult>(resultText);
                results.Add(result);
            }

            PlotResults<TNode>("unity", results);
        }

        protected void PlotResults<TNode>(string name, List<BenchmarkScenarioResult> results)
        {
            var plt = new Plot(600, 400);
            var xValues = DataGen.Consecutive(results[0].BenchmarkResults.Count);

            foreach (var result in results)
            {
                var times = result.BenchmarkResults.Select(x => x.Runs.Average(y => y.Time / 1000d)).ToList().OrderBy(x => x);
                plt.PlotScatter(xValues, times.ToArray(), label: result.Name);
            }

            plt.Legend(fontSize: 10);
            plt.SaveFig(Path.Combine(DirectoryFullPath, $"{name}.png"));

        }

        protected virtual BenchmarkScenarioResult RunBenchmark<TNode>(BenchmarkScenario<TNode> scenario, ILevelGeneratorFactory<TNode> generator, int iterations)
        {
            var scenarioResult = BenchmarkRunner.Run(scenario, generator, iterations, new MapGeneration.Benchmarks.BenchmarkOptions()
            {
                WithConsolePreview = Options.WithConsolePreview,
                MultiThreaded = Options.MaxThreads > 1,
                MaxDegreeOfParallelism = Options.MaxThreads,
                WithFileOutput = false,
            }); 

            var resultSaver = new BenchmarkResultSaver();
            resultSaver.SaveResultDefaultLocation(scenarioResult, directory: DirectoryFullPath, name: $"{Directory}_{scenario.Name}_{generator.Name}", withDatetime: false);

            return scenarioResult;
        }
    }
}