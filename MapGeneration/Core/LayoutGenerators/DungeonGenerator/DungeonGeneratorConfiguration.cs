using System.Collections.Generic;
using System.Linq;
using MapGeneration.Core.ChainDecompositions;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Interfaces.Core.ChainDecompositions;
using MapGeneration.Interfaces.Utils;
using MapGeneration.MetaOptimization.Configurations;
using MapGeneration.MetaOptimization.Evolution.SAConfigurationEvolution;

namespace MapGeneration.Core.LayoutGenerators.DungeonGenerator
{
    public class DungeonGeneratorConfiguration : IChainDecompositionConfiguration<int>, ISimulatedAnnealingConfiguration, ISmartCloneable<DungeonGeneratorConfiguration>
    {
        public List<IChain<int>> Chains { get; set; }

        public SimulatedAnnealingConfigurationProvider SimulatedAnnealingConfiguration { get; set; }

        public static DungeonGeneratorConfiguration GetDefaultConfiguration<TNode>(MapDescription<TNode> mapDescription)
        {
            var chainDecomposition = new TwoStageChainDecomposition<int>(mapDescription, new BreadthFirstChainDecomposition<int>());
            var chains = chainDecomposition.GetChains(mapDescription.GetGraph());

            var simulatedAnnealingConfigurations = new List<SimulatedAnnealingConfiguration>();

            for (int i = 0; i < chains.Count; i++)
            {
                simulatedAnnealingConfigurations.Add(LayoutEvolvers.SimulatedAnnealing.SimulatedAnnealingConfiguration.GetDefaultConfiguration());
            }

            return new DungeonGeneratorConfiguration()
            {
                Chains = chains.ToList(),
                SimulatedAnnealingConfiguration = new SimulatedAnnealingConfigurationProvider(simulatedAnnealingConfigurations),
            };
        }

        public DungeonGeneratorConfiguration SmartClone()
        {
            return new DungeonGeneratorConfiguration()
            {
                // TODO: ugly
                Chains = Chains.Select(x => new Chain<int>(x.Nodes.ToList(), x.Number)).Cast<IChain<int>>().ToList(),
                SimulatedAnnealingConfiguration = SimulatedAnnealingConfiguration.SmartClone(),
            };
        }

        public override string ToString()
        {
            var result = "";

            for (int i = 0; i < Chains.Count; i++)
            {
                var chain = Chains[i];
                var configuration = SimulatedAnnealingConfiguration.GetConfiguration(i);
                result +=
                    $"\n  Chain {i} [{string.Join(",", chain.Nodes)}] c {configuration.Cycles} tpc {configuration.TrialsPerCycle} miws {configuration.MaxIterationsWithoutSuccess}";
            }

            return result;
        }

        #region Equals

        protected bool Equals(DungeonGeneratorConfiguration other)
        {
            return Chains.SequenceEqual(other.Chains) && SimulatedAnnealingConfiguration.Equals(other.SimulatedAnnealingConfiguration);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DungeonGeneratorConfiguration)obj);
        }

        public override int GetHashCode()
        {
            return (Chains != null ? Chains.GetHashCode() : 0);
        }

        public static bool operator ==(DungeonGeneratorConfiguration left, DungeonGeneratorConfiguration right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DungeonGeneratorConfiguration left, DungeonGeneratorConfiguration right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}