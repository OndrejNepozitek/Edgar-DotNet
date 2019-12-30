using System;
using System.Collections.Generic;
using System.Linq;
using MapGeneration.Core.ChainDecompositions;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Interfaces.Core.ChainDecompositions;
using MapGeneration.Interfaces.Core.MapDescriptions;
using MapGeneration.Interfaces.Utils;
using MapGeneration.MetaOptimization.Configurations;

namespace MapGeneration.Core.LayoutGenerators.DungeonGenerator
{
    public class DungeonGeneratorConfiguration<TNode> : IChainDecompositionConfiguration<TNode>, ISimulatedAnnealingConfiguration, ISmartCloneable<DungeonGeneratorConfiguration<TNode>> where TNode : IEquatable<TNode>
    {
        /// <summary>
        /// Whether non-neighboring rooms may touch (share walls) or not.
        /// The setting is applied only to non-neighboring rooms because all neighbors
        /// share walls as they must be connected by doors.
        /// It is recommended to allow touching rooms if the dungeon is without corridors.
        /// </summary>
        public bool RoomsCanTouch { get; set; } = true;

        /// <summary>
        /// Decomposition of the input graph into disjunct subgraphs.
        /// </summary>
        public List<IChain<TNode>> Chains { get; set; }

        /// <summary>
        /// Simulated annealing configuration.
        /// </summary>
        public SimulatedAnnealingConfigurationProvider SimulatedAnnealingConfiguration { get; set; }

        public DungeonGeneratorConfiguration(IMapDescription<TNode> mapDescription)
        {
            var chainDecomposition = new TwoStageChainDecomposition<TNode>(mapDescription, new BreadthFirstChainDecomposition<TNode>());
            Chains = chainDecomposition.GetChains(mapDescription.GetGraph()).ToList();

            var simulatedAnnealingConfigurations = new List<SimulatedAnnealingConfiguration>();
            for (int i = 0; i < Chains.Count; i++)
            {
                simulatedAnnealingConfigurations.Add(LayoutEvolvers.SimulatedAnnealing.SimulatedAnnealingConfiguration.GetDefaultConfiguration());
            }
            SimulatedAnnealingConfiguration = new SimulatedAnnealingConfigurationProvider(simulatedAnnealingConfigurations);
        }

        protected DungeonGeneratorConfiguration()
        {
            /* empty */
        }

        public DungeonGeneratorConfiguration<TNode> SmartClone()
        {
            return new DungeonGeneratorConfiguration<TNode>()
            {
                // TODO: ugly
                Chains = Chains.Select(x => new Chain<TNode>(x.Nodes.ToList(), x.Number)).Cast<IChain<TNode>>().ToList(),
                SimulatedAnnealingConfiguration = SimulatedAnnealingConfiguration.SmartClone(),
                RoomsCanTouch = RoomsCanTouch,
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
        public DungeonGeneratorConfiguration(IMapDescription<int> mapDescription) : base(mapDescription)
        {
        }

        public DungeonGeneratorConfiguration()
        {
        }

        public new DungeonGeneratorConfiguration SmartClone()
        {
            return new DungeonGeneratorConfiguration()
            {
                // TODO: ugly
                Chains = Chains.Select(x => new Chain<int>(x.Nodes.ToList(), x.Number)).Cast<IChain<int>>().ToList(),
                SimulatedAnnealingConfiguration = SimulatedAnnealingConfiguration.SmartClone(),
                RoomsCanTouch = RoomsCanTouch,
            };
        }
    }
}