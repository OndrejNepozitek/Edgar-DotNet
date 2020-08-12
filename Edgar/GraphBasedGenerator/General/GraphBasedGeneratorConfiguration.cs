using System.Linq;
using Edgar.Legacy.Core.ChainDecompositions;
using Edgar.Legacy.Core.LayoutGenerators.DungeonGenerator;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.Utils.Interfaces;

namespace Edgar.GraphBasedGenerator.General
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