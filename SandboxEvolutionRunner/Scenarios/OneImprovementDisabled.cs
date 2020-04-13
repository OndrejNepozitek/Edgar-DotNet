using MapGeneration.Core.ChainDecompositions;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using SandboxEvolutionRunner.Utils;

namespace SandboxEvolutionRunner.Scenarios
{
    public class OneImprovementDisabled : Scenario
    {
        private DungeonGeneratorConfiguration<int> GetWithoutGreedyTreesConfiguration(NamedMapDescription namedMapDescription)
        {
            var configuration = GetNewConfiguration(namedMapDescription);
            configuration.SimulatedAnnealingConfiguration.GetConfiguration(0).HandleTreesGreedily = false;

            return configuration;
        }

        private DungeonGeneratorConfiguration<int> GetWithoutMaxIterationsConfiguration(NamedMapDescription namedMapDescription)
        {
            var configuration = GetNewConfiguration(namedMapDescription);
            configuration.SimulatedAnnealingConfiguration = new SimulatedAnnealingConfigurationProvider(new SimulatedAnnealingConfiguration()
            {
                MaxIterationsWithoutSuccess = 10000,
            });

            return configuration;
        }

        private DungeonGeneratorConfiguration<int> GetWithoutChainDecompositionConfiguration(NamedMapDescription namedMapDescription)
        {
            var chainDecompositionOld = new BreadthFirstChainDecompositionOld<int>();
            var chainDecomposition = new TwoStageChainDecomposition<int>(namedMapDescription.MapDescription, chainDecompositionOld);

            var configuration = GetNewConfiguration(namedMapDescription);
            configuration.Chains = chainDecomposition.GetChains(namedMapDescription.MapDescription.GetGraph());

            return configuration;
        }

        private DungeonGeneratorConfiguration<int> GetNewConfiguration(NamedMapDescription namedMapDescription)
        {
            var configuration = GetBasicConfiguration(namedMapDescription);
            configuration.SimulatedAnnealingConfiguration = new SimulatedAnnealingConfigurationProvider(new SimulatedAnnealingConfiguration()
            {
                MaxIterationsWithoutSuccess = 100,
                HandleTreesGreedily = true,
            });

            return configuration;
        }

        private DungeonGeneratorConfiguration<int> GetOldConfiguration(NamedMapDescription namedMapDescription)
        {
            var chainDecompositionOld = new BreadthFirstChainDecompositionOld<int>();
            var chainDecomposition = new TwoStageChainDecomposition<int>(namedMapDescription.MapDescription, chainDecompositionOld);

            var configuration = GetBasicConfiguration(namedMapDescription);
            configuration.Chains = chainDecomposition.GetChains(namedMapDescription.MapDescription.GetGraph());
            configuration.SimulatedAnnealingConfiguration = new SimulatedAnnealingConfigurationProvider(new SimulatedAnnealingConfiguration()
            {
                MaxIterationsWithoutSuccess = 10000,
            });

            return configuration;
        }

        protected override void Run()
        {
            var mapDescriptions = GetMapDescriptions();

            RunBenchmark(mapDescriptions, GetNewConfiguration, Options.FinalEvaluationIterations, "New");
            RunBenchmark(mapDescriptions, GetOldConfiguration, Options.FinalEvaluationIterations, "Old");
            RunBenchmark(mapDescriptions, GetWithoutGreedyTreesConfiguration, Options.FinalEvaluationIterations, "WithoutGreedyTrees");
            RunBenchmark(mapDescriptions, GetWithoutMaxIterationsConfiguration, Options.FinalEvaluationIterations, "WithoutMaxIterations");
            RunBenchmark(mapDescriptions, GetWithoutChainDecompositionConfiguration, Options.FinalEvaluationIterations, "WithoutChainDecomposition");
        }
    }
}