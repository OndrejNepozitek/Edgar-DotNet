using System.Collections.Generic;
using System.Linq;
using MapGeneration.Core.ChainDecompositions;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Interfaces.Core.ChainDecompositions;
using MapGeneration.Interfaces.Utils;
using MapGeneration.MetaOptimization.Configurations;

namespace MapGeneration.MetaOptimization.Evolution.DungeonGeneratorEvolution
{
    // TODO: maybe make properties private setter so that all have to be provided
    public class GeneratorConfiguration : IChainDecompositionConfiguration<int>, ISimulatedAnnealingConfiguration, ISmartCloneable<GeneratorConfiguration>
    {
        public List<IChain<int>> Chains { get; set; }

        public SimulatedAnnealingConfigurationProvider SimulatedAnnealingConfiguration { get; set; }

        public GeneratorConfiguration SmartClone()
        {
            return new GeneratorConfiguration()
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

        protected bool Equals(GeneratorConfiguration other)
        {
            return Chains.SequenceEqual(other.Chains) && SimulatedAnnealingConfiguration.Equals(other.SimulatedAnnealingConfiguration);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GeneratorConfiguration)obj);
        }

        public override int GetHashCode()
        {
            return (Chains != null ? Chains.GetHashCode() : 0);
        }

        public static bool operator ==(GeneratorConfiguration left, GeneratorConfiguration right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GeneratorConfiguration left, GeneratorConfiguration right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}