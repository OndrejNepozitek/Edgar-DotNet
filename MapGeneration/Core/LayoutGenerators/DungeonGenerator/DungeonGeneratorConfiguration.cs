using System;
using System.Collections.Generic;
using System.Linq;
using MapGeneration.Core.ChainDecompositions;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Interfaces.Core.ChainDecompositions;
using MapGeneration.Interfaces.Core.MapDescriptions;
using MapGeneration.Interfaces.Utils;
using MapGeneration.MetaOptimization.Configurations;
using MapGeneration.MetaOptimization.Evolution.SAConfigurationEvolution;

namespace MapGeneration.Core.LayoutGenerators.DungeonGenerator
{
    public class DungeonGeneratorConfiguration<TNode> : IChainDecompositionConfiguration<TNode>, ISimulatedAnnealingConfiguration, ISmartCloneable<DungeonGeneratorConfiguration<TNode>> where TNode : IEquatable<TNode>
    {
        public List<IChain<TNode>> Chains { get; set; }

        public SimulatedAnnealingConfigurationProvider SimulatedAnnealingConfiguration { get; set; }

        public static DungeonGeneratorConfiguration<TNode> GetDefaultConfiguration(IMapDescription<TNode> mapDescription)
        {
            var chainDecomposition = new TwoStageChainDecomposition<TNode>(mapDescription, new BreadthFirstChainDecomposition<TNode>());
            var chains = chainDecomposition.GetChains(mapDescription.GetGraph());

            var simulatedAnnealingConfigurations = new List<SimulatedAnnealingConfiguration>();

            for (int i = 0; i < chains.Count; i++)
            {
                simulatedAnnealingConfigurations.Add(LayoutEvolvers.SimulatedAnnealing.SimulatedAnnealingConfiguration.GetDefaultConfiguration());
            }

            return new DungeonGeneratorConfiguration<TNode>()
            {
                Chains = chains.ToList(),
                SimulatedAnnealingConfiguration = new SimulatedAnnealingConfigurationProvider(simulatedAnnealingConfigurations),
            };
        }

        public DungeonGeneratorConfiguration<TNode> SmartClone()
        {
            return new DungeonGeneratorConfiguration<TNode>()
            {
                // TODO: ugly
                Chains = Chains.Select(x => new Chain<TNode>(x.Nodes.ToList(), x.Number)).Cast<IChain<TNode>>().ToList(),
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

        protected bool Equals(DungeonGeneratorConfiguration<TNode> other)
        {
            return Chains.SequenceEqual(other.Chains) && SimulatedAnnealingConfiguration.Equals(other.SimulatedAnnealingConfiguration);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DungeonGeneratorConfiguration<TNode>)obj);
        }

        public override int GetHashCode()
        {
            return (Chains != null ? Chains.GetHashCode() : 0);
        }

        public static bool operator ==(DungeonGeneratorConfiguration<TNode> left, DungeonGeneratorConfiguration<TNode> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DungeonGeneratorConfiguration<TNode> left, DungeonGeneratorConfiguration<TNode> right)
        {
            return !Equals(left, right);
        }

        #endregion
    }

    // TODO: remove later
    public class DungeonGeneratorConfiguration : DungeonGeneratorConfiguration<int>, ISmartCloneable<DungeonGeneratorConfiguration>
    {
        public static DungeonGeneratorConfiguration GetDefaultConfiguration<TNode>(MapDescriptionOld<TNode> mapDescriptionOld)
        {
            var chainDecomposition = new TwoStageChainDecomposition<int>(mapDescriptionOld, new BreadthFirstChainDecomposition<int>());
            var chains = chainDecomposition.GetChains(mapDescriptionOld.GetGraph());

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

        public new DungeonGeneratorConfiguration SmartClone()
        {
            return new DungeonGeneratorConfiguration()
            {
                // TODO: ugly
                Chains = Chains.Select(x => new Chain<int>(x.Nodes.ToList(), x.Number)).Cast<IChain<int>>().ToList(),
                SimulatedAnnealingConfiguration = SimulatedAnnealingConfiguration.SmartClone(),
            };
        }
    }
}