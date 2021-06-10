using System.Collections.Generic;
using System.Linq;
using Edgar.Legacy.Core.ChainDecompositions;
using Edgar.Legacy.Utils.Logging;

namespace Edgar.GraphBasedGenerator.Common.ChainDecomposition
{
    public class FixedRoomsChainDecomposition<TNode> : ChainDecomposition<TNode>
    {
        private readonly List<TNode> fixedRooms;

        public FixedRoomsChainDecomposition(ChainDecompositionConfiguration configuration, Logger logger = null, List<TNode> fixedRooms = null) : base(configuration, logger, fixedRooms)
        {
            this.fixedRooms = fixedRooms ?? new List<TNode>();
        }

        protected override ChainCandidate<TNode> GetInitialChain(PartialDecomposition<TNode> decomposition)
        {
            // Some of the fixed rooms might not be included in the graph that is being decomposed
            var relevantFixedRooms = decomposition.Graph.Vertices.Where(x => fixedRooms.Contains(x)).ToList();

            // If there are no fixed rooms, return the default initial chain
            if (relevantFixedRooms.Count == 0)
            {
                return base.GetInitialChain(decomposition);
            }

            var faces = decomposition.GetRemainingFaces();
            
            // If there are no cycles and only a single fixed room, start a tree chain from that room
            if (faces.Count == 0 && relevantFixedRooms.Count == 1)
            {
                return ChainDecompositionUtils.GetBfsTreeCandidate(
                    decomposition,
                    new List<TNode>() { relevantFixedRooms[0] },
                    MaxTreeSize
                );
            }

            return base.GetInitialChain(decomposition);
        }
    }
}