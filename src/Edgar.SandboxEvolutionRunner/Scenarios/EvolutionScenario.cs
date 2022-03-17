using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Edgar.Legacy.Core.LayoutGenerators.DungeonGenerator;
using Edgar.Legacy.Core.MapDescriptions.Interfaces;
using Edgar.Legacy.Utils;
using Edgar.Legacy.Utils.MetaOptimization.Evolution;
using Edgar.Legacy.Utils.MetaOptimization.Evolution.DungeonGeneratorEvolution;
using Edgar.Legacy.Utils.MetaOptimization.Mutations;
using Edgar.Legacy.Utils.MetaOptimization.Mutations.ChainDecomposition;
using Edgar.Legacy.Utils.MetaOptimization.Mutations.ChainMerge;
using Edgar.Legacy.Utils.MetaOptimization.Mutations.ChainOrder;
using Edgar.Legacy.Utils.MetaOptimization.Mutations.MaxBranching;
using Edgar.Legacy.Utils.MetaOptimization.Mutations.MaxIterations;
using Edgar.Legacy.Utils.MetaOptimization.Mutations.MaxStageTwoFailures;
using Edgar.Legacy.Utils.Statistics;
using SandboxEvolutionRunner.Evolution;
using SandboxEvolutionRunner.Utils;

namespace SandboxEvolutionRunner.Scenarios
{
    public class EvolutionScenario : Scenario
    {
        protected virtual List<IPerformanceAnalyzer<DungeonGeneratorConfiguration<int>, Individual<int>>>
            GetAnalyzers(Input input)
        {
            var allAnalyzers =
                new Dictionary<string, Func<IMapDescription<int>,
                    IPerformanceAnalyzer<DungeonGeneratorConfiguration<int>, Individual<int>>>>()
                {
                    {
                        "MaxStageTwoFailures",
                        (_) => new MaxStageTwoFailuresAnalyzer<DungeonGeneratorConfiguration<int>, GeneratorData>()
                    },
                    {
                        "MaxIterations",
                        (_) => new MaxIterationsAnalyzer<DungeonGeneratorConfiguration<int>, GeneratorData>()
                    },
                    {
                        "ChainMerge",
                        (_) => new ChainMergeAnalyzer<DungeonGeneratorConfiguration<int>, int, GeneratorData>()
                    },
                    {
                        "ChainOrder",
                        (_) => new ChainOrderAnalyzer<DungeonGeneratorConfiguration<int>, int, GeneratorData>()
                    },
                    {
                        "MaxBranching",
                        (_) => new MaxBranchingAnalyzer<DungeonGeneratorConfiguration<int>, GeneratorData>()
                    },
                    {
                        "ChainDecomposition",
                        (mapDescription) =>
                            new ChainDecompositionAnalyzer<DungeonGeneratorConfiguration<int>, int, GeneratorData>(
                                mapDescription)
                    },
                };

            // Select analyzers
            var analyzers =
                Options.Mutations.Count() != 0
                    ? Options
                        .Mutations
                        .Select(x => allAnalyzers[x])
                        .ToList()
                    : allAnalyzers.Values.ToList();

            return analyzers.Select(x => x(input.MapDescription)).ToList();
        }

        protected virtual List<Result> RunEvolution(List<Input> inputs)
        {
            var resultsDict = new Dictionary<Input, Result>();
            var partitioner = Partitioner.Create(inputs, EnumerablePartitionerOptions.NoBuffering);

            Parallel.ForEach(partitioner, new ParallelOptions {MaxDegreeOfParallelism = Options.MaxThreads}, input =>
            {
                lock (Logger)
                {
                    Logger.WriteLine($"Started {input.Name}");
                }

                var analyzers = GetAnalyzers(input);
                var result = RunEvolution(input, Options, analyzers);

                lock (Logger)
                {
                    Logger.WriteLine($"Ended {input.Name}");
                }


                lock (resultsDict)
                {
                    resultsDict[input] = result;
                }
            });

            var results = inputs.Select(x => resultsDict[x]).ToList();

            return results;
        }

        protected virtual Result RunEvolution(Input input, Options options,
            List<IPerformanceAnalyzer<DungeonGeneratorConfiguration<int>, Individual<int>>> analyzers)
        {
            var evolution = new DungeonGeneratorEvolution<int>(input.MapDescription, analyzers, new EvolutionOptions()
            {
                MaxPopulationSize = options.Eval ? 2 : 20,
                MaxMutationsPerIndividual = 20,
                EvaluationIterations = options.EvolutionIterations,
                WithConsoleOutput = false,
                AllowWorseThanInitial = !options.Eval,
                AllowRepeatingConfigurations = !options.Eval,
                FitnessType = options.FitnessType,
            }, Path.Combine(DirectoryFullPath, FileNamesHelper.PrefixWithTimestamp(input.Name)));

            var result = evolution.Evolve(input.Configuration);

            return new Result()
            {
                Input = input,
                NewConfiguration = result.BestConfiguration,
                Individuals = result.AllIndividuals,
            };
        }

        protected virtual List<Input> GetInputs(List<NamedMapDescription> namedMapDescriptions)
        {
            return namedMapDescriptions.Select(x => new Input()
            {
                Configuration = GetInitialConfiguration(x),
                MapDescription = x.MapDescription,
                Name = x.Name,
            }).ToList();
        }

        protected virtual DungeonGeneratorConfiguration<int> GetInitialConfiguration(
            NamedMapDescription namedMapDescription)
        {
            return GetBasicConfiguration(namedMapDescription);
        }

        protected virtual void AnalyzeMutations(List<Result> results)
        {
            var stats = new Dictionary<IMutation<DungeonGeneratorConfiguration<int>>, List<Program.MutationStats>>();

            foreach (var result in results)
            {
                var mutatedIndividuals = result.Individuals.Where(x => x.Mutations.Count != 0).ToList();
                var mutatedIndividualsGroups = mutatedIndividuals.GroupBy(x => x.Fitness).ToList();
                mutatedIndividualsGroups.Sort((x1, x2) => x1.Key.CompareTo(x2.Key));

                for (var i = 0; i < mutatedIndividualsGroups.Count; i++)
                {
                    var group = mutatedIndividualsGroups[i];

                    foreach (var individual in group)
                    {
                        var mutation = individual.Mutations.Last();
                        var difference =
                            StatisticsUtils.DifferenceToReference(individual.Fitness, individual.Parent.Fitness);

                        if (!stats.ContainsKey(mutation))
                        {
                            stats[mutation] = new List<Program.MutationStats>();
                        }

                        stats[mutation].Add(new Program.MutationStats()
                        {
                            Difference = difference,
                            Input = result.Input.Name,
                            Order = i + 1,
                        });
                    }
                }
            }

            foreach (var pair in stats)
            {
                var mutation = pair.Key;
                var mutationStats = pair.Value;

                Logger.WriteLine(mutation);

                foreach (var mutationStat in mutationStats)
                {
                    Logger.WriteLine(
                        $"{mutationStat.Input}, diff {mutationStat.Difference:F}%, order {mutationStat.Order}");
                }

                Logger.WriteLine($"Average difference {mutationStats.Average(x => x.Difference):F}");
                Logger.WriteLine($"Average order {mutationStats.Average(x => x.Order):F}");

                Logger.WriteLine();
            }
        }

        protected override void Run()
        {
            var mapDescriptions = GetMapDescriptions();
            var inputs = GetInputs(mapDescriptions);
            var results = RunEvolution(inputs);

            AnalyzeMutations(results);


            var inputsNewConfigurations = results
                .Select(x => new DungeonGeneratorInput<int>(x.Input.Name, x.Input.MapDescription, x.NewConfiguration))
                .ToList();
            var inputsOldConfigurations = results
                .Select(x =>
                    new DungeonGeneratorInput<int>(x.Input.Name, x.Input.MapDescription, x.Input.Configuration))
                .ToList();

            RunBenchmark(inputsNewConfigurations, Options.FinalEvaluationIterations, "NewConfigurations");
            RunBenchmark(inputsOldConfigurations, Options.FinalEvaluationIterations, "OldConfigurations");
        }
    }
}