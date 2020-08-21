using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.GraphBasedGenerator.Common;
using Edgar.Legacy.Core.ChainDecompositions;
using Edgar.Legacy.Core.LayoutEvolvers.SimulatedAnnealing;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.Utils.Interfaces;
using Edgar.Legacy.Utils.MetaOptimization.Configurations;

namespace Edgar.Legacy.Core.LayoutGenerators.DungeonGenerator
{
    /// <summary>
    /// Configuration of the dungeon generator.
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    public class DungeonGeneratorConfiguration<TNode> : IChainDecompositionConfiguration<TNode>,
        ISimulatedAnnealingConfiguration, ISmartCloneable<DungeonGeneratorConfiguration<TNode>>
    {
        /// <summary>
        /// Whether non-neighboring rooms may touch (share walls) or not.
        /// The setting is applied only to non-neighboring rooms because all neighbors
        /// share walls as they must be connected by doors.
        /// It is recommended to allow touching rooms if the dungeon is without corridors.
        /// </summary>
        public bool RoomsCanTouch { get; set; } = true;

        /// <summary>
        /// The number of iterations after which the algorithm is stopped even if it
        /// was not able to generate a layout.
        /// Defaults to null - algorithm is not stopped early.
        /// </summary>
        public int? EarlyStopIfIterationsExceeded { get; set; }

        /// <summary>
        /// The time interval after which the algorithm is stopped even if it
        /// was not able to generate a layout.
        /// Defaults to null - algorithm is not stopped early.
        /// </summary>
        public TimeSpan? EarlyStopIfTimeExceeded { get; set; }

        /// <summary>
        /// Whether to override repeat mode of individual room templates.
        /// If this property is set, the chosen repeat mode will be used for all room templates
        /// no matter what their own repeat mode was.
        /// </summary>
        public RoomTemplateRepeatMode? RepeatModeOverride { get; set; }

        /// <summary>
        /// Whether to throw an exception when the algorithm is not able to satisfy all the repeat
        /// mode requirements. If set to false, the algorithm will try to lower the requirements,
        /// e.g. instead of no rooms being repeated, it at least tries to not repeat neighbors.
        /// </summary>
        public bool ThrowIfRepeatModeNotSatisfied { get; set; }

        /// <summary>
        /// Chain decomposition configuration that will be used to compute the chains from the input graph.
        /// </summary>
        /// <remarks>
        /// Should not be changed manually.
        /// </remarks>
        public ChainDecompositionConfiguration ChainDecompositionConfiguration { get; set; }

        /// <summary>
        /// Decomposition of the input graph into disjunct subgraphs (chains).
        /// In most cases, this property should not be set and the algorithm will use the ChainDecompositionConfiguration
        /// to compute the chains. 
        /// </summary>
        /// <remarks>
        /// Should not be changed manually.
        /// </remarks>
        public List<Chain<TNode>> Chains { get; set; }

        /// <summary>
        /// Simulated annealing configuration.
        /// </summary>
        /// <remarks>
        /// Should not be changed manually.
        /// </remarks>
        public SimulatedAnnealingConfigurationProvider SimulatedAnnealingConfiguration { get; set; }

        /// <summary>
        /// Maximum branching factor of simulated annealing.
        /// </summary>
        /// <remarks>
        /// Should not be changed manually.
        /// </remarks>
        public int SimulatedAnnealingMaxBranching { get; set; } = 5;

        public DungeonGeneratorConfiguration()
        {
            ChainDecompositionConfiguration = new ChainDecompositionConfiguration();
            SimulatedAnnealingConfiguration = new SimulatedAnnealingConfigurationProvider(LayoutEvolvers.SimulatedAnnealing.SimulatedAnnealingConfiguration.GetDefaultConfiguration());
        }
        
        public DungeonGeneratorConfiguration<TNode> SmartClone()
        {
            return new DungeonGeneratorConfiguration<TNode>()
            {
                RoomsCanTouch = RoomsCanTouch,
                EarlyStopIfIterationsExceeded = EarlyStopIfIterationsExceeded,
                EarlyStopIfTimeExceeded = EarlyStopIfTimeExceeded,
                RepeatModeOverride = RepeatModeOverride,
                ThrowIfRepeatModeNotSatisfied = ThrowIfRepeatModeNotSatisfied,
                ChainDecompositionConfiguration = ChainDecompositionConfiguration.SmartClone(),
                Chains = Chains?.Select(x => new Chain<TNode>(x.Nodes.ToList(), x.Number)).ToList(),
                SimulatedAnnealingConfiguration = SimulatedAnnealingConfiguration.SmartClone(),
                SimulatedAnnealingMaxBranching = SimulatedAnnealingMaxBranching,
            };
        }

        public override string ToString()
        {
            var result = "";

            result += $"\n  Max branching {SimulatedAnnealingMaxBranching}";

            if (Chains != null)
            {
                for (int i = 0; i < Chains.Count; i++)
                {
                    var chain = Chains[i];
                    var configuration = SimulatedAnnealingConfiguration.GetConfiguration(i);
                    result +=
                        $"\n  Chain {i} [{string.Join(",", chain.Nodes)}] c {configuration.Cycles} tpc {configuration.TrialsPerCycle} miws {configuration.MaxIterationsWithoutSuccess} ms2f {configuration.MaxStageTwoFailures}";
                }
            } else if (SimulatedAnnealingConfiguration != null)
            {
                var list = SimulatedAnnealingConfiguration.GetAllConfigurations();

                for (var i = 0; i < list.Count; i++)
                {
                    var configuration = list[i];
                    result +=
                        $"\n  Chain {i} [] c {configuration.Cycles} tpc {configuration.TrialsPerCycle} miws {configuration.MaxIterationsWithoutSuccess} ms2f {configuration.MaxStageTwoFailures}";
                }
            }


            return result;
        }

        #region Equals

        protected bool Equals(DungeonGeneratorConfiguration<TNode> other)
        {
            return RoomsCanTouch == other.RoomsCanTouch 
                   && EarlyStopIfIterationsExceeded == other.EarlyStopIfIterationsExceeded
                   && Nullable.Equals(EarlyStopIfTimeExceeded, other.EarlyStopIfTimeExceeded)
                   && RepeatModeOverride == other.RepeatModeOverride 
                   && ThrowIfRepeatModeNotSatisfied == other.ThrowIfRepeatModeNotSatisfied
                   && Equals(ChainDecompositionConfiguration, other.ChainDecompositionConfiguration) 
                   && Chains.SequenceEqual(other.Chains)
                   && Equals(SimulatedAnnealingConfiguration, other.SimulatedAnnealingConfiguration)
                   && SimulatedAnnealingMaxBranching == other.SimulatedAnnealingMaxBranching;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DungeonGeneratorConfiguration<TNode>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = RoomsCanTouch.GetHashCode();
                hashCode = (hashCode * 397) ^ EarlyStopIfIterationsExceeded.GetHashCode();
                hashCode = (hashCode * 397) ^ EarlyStopIfTimeExceeded.GetHashCode();
                hashCode = (hashCode * 397) ^ RepeatModeOverride.GetHashCode();
                hashCode = (hashCode * 397) ^ ThrowIfRepeatModeNotSatisfied.GetHashCode();
                hashCode = (hashCode * 397) ^ (ChainDecompositionConfiguration != null ? ChainDecompositionConfiguration.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (SimulatedAnnealingConfiguration != null ? SimulatedAnnealingConfiguration.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ SimulatedAnnealingMaxBranching;
                return hashCode;
            }
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
}