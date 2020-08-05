using System.Linq;
using MapGeneration.Core.ChainDecompositions;
using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Utils.Interfaces;

namespace Edgar.GraphBasedGenerator
{
    public class GraphBasedGeneratorConfiguration<TNode> : DungeonGeneratorConfiguration<TNode>, ISmartCloneable<GraphBasedGeneratorConfiguration<TNode>>
    {
        public bool OptimizeCorridorConstraints { get; set; }

        public new GraphBasedGeneratorConfiguration<TNode> SmartClone()
        {
            return new GraphBasedGeneratorConfiguration<TNode>()
            {
                RoomsCanTouch = RoomsCanTouch,
                EarlyStopIfIterationsExceeded = EarlyStopIfIterationsExceeded,
                EarlyStopIfTimeExceeded = EarlyStopIfTimeExceeded,
                RepeatModeOverride = RepeatMode.NoImmediate,
                ThrowIfRepeatModeNotSatisfied = ThrowIfRepeatModeNotSatisfied,
                ChainDecompositionConfiguration = ChainDecompositionConfiguration.SmartClone(),
                Chains = Chains?.Select(x => new Chain<TNode>(x.Nodes.ToList(), x.Number)).ToList(),
                SimulatedAnnealingConfiguration = SimulatedAnnealingConfiguration.SmartClone(),
                SimulatedAnnealingMaxBranching = SimulatedAnnealingMaxBranching,

                OptimizeCorridorConstraints = OptimizeCorridorConstraints,
            };
        }
    }
}