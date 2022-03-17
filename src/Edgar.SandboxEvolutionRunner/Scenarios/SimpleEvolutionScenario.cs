using System.Collections.Generic;
using System.IO;
using Edgar.Legacy.Core.ChainDecompositions;
using Edgar.Legacy.Core.LayoutGenerators.DungeonGenerator;
using Edgar.Legacy.Utils;
using Edgar.Legacy.Utils.MetaOptimization.Evolution;
using Edgar.Legacy.Utils.MetaOptimization.Evolution.DungeonGeneratorEvolution;
using Edgar.Legacy.Utils.MetaOptimization.Evolution.SimpleDungeonGeneratorEvolution;
using Edgar.Legacy.Utils.MetaOptimization.Mutations;
using SandboxEvolutionRunner.Evolution;
using SandboxEvolutionRunner.Utils;

namespace SandboxEvolutionRunner.Scenarios
{
    public class SimpleEvolutionScenario : EvolutionScenario
    {
        protected override Result RunEvolution(Input input, Options options,
            List<IPerformanceAnalyzer<DungeonGeneratorConfiguration<int>, Individual<int>>> analyzers)
        {
            var evolution = new SimpleDungeonGeneratorEvolution<int>(input.MapDescription, analyzers,
                new EvolutionOptions()
                {
                    MaxPopulationSize = 1,
                    MaxMutationsPerIndividual = 20,
                    EvaluationIterations = options.EvolutionIterations,
                    WithConsoleOutput = false,
                    AllowWorseThanInitial = true,
                    AllowRepeatingConfigurations = false,
                    AllowNotPerfectSuccessRate = true,
                    FitnessType = options.FitnessType,
                    AddPreviousGenerationWhenComputingNext = true,
                }, Path.Combine(DirectoryFullPath, FileNamesHelper.PrefixWithTimestamp(input.Name)));

            var result = evolution.Evolve(input.Configuration);

            return new Result()
            {
                Input = input,
                NewConfiguration = result.BestConfiguration,
                Individuals = result.AllIndividuals,
            };
        }

        protected override DungeonGeneratorConfiguration<int> GetBasicConfiguration(
            NamedMapDescription namedMapDescription)
        {
            var configuration = base.GetBasicConfiguration(namedMapDescription);
            var chainDecompositionOld = new BreadthFirstChainDecompositionOld<int>();
            var chainDecomposition =
                new TwoStageChainDecomposition<int>(namedMapDescription.MapDescription, chainDecompositionOld);
            var chains = chainDecomposition.GetChains(namedMapDescription.MapDescription.GetGraph());
            configuration.Chains = chains;

            return configuration;
        }
    }
}