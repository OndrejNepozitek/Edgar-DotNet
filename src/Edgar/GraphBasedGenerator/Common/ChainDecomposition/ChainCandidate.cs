using System.Collections.Generic;

namespace Edgar.GraphBasedGenerator.Common.ChainDecomposition
{
    public class ChainCandidate<TNode>
    {
        public List<TNode> Nodes { get; set; }

        public bool IsFromFace { get; set; }

        public int MinimumNeighborChainNumber { get; set; }

        public override string ToString()
        {
            return $"{(IsFromFace ? "cycle" : "tree")} {MinimumNeighborChainNumber} [{string.Join(",", Nodes)}]";
        }
    }
}