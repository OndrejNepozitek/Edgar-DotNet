using System.Collections.Generic;
using System.Linq;
using Edgar.Graphs;

namespace Edgar.GraphBasedGenerator.Common.ChainDecomposition
{
    public class PartialDecomposition<TNode>
    {
        private readonly Dictionary<TNode, int> coveredVertices;
        private readonly List<List<TNode>> remainingFaces;
        private readonly List<Chain<TNode>> chains;

        public IGraph<TNode> Graph { get; }

        public int NumberOfChains => coveredVertices.Count == 0 ? 0 : coveredVertices.Values.Max() + 1;

        public PartialDecomposition(List<List<TNode>> faces, IGraph<TNode> graph)
        {
            this.remainingFaces = faces;
            coveredVertices = new Dictionary<TNode, int>();
            chains = new List<Chain<TNode>>();
            Graph = graph;
        }

        private PartialDecomposition(PartialDecomposition<TNode> oldDecomposition, List<TNode> chain, bool isFromFace)
        {
            Graph = oldDecomposition.Graph;
            coveredVertices = new Dictionary<TNode, int>(oldDecomposition.coveredVertices);

            // Cover chain
            var numberOfChains = oldDecomposition.NumberOfChains;
            foreach (var node in chain)
            {
                coveredVertices[node] = numberOfChains;
            }

            // Remove covered faces
            remainingFaces = oldDecomposition
                .remainingFaces
                .Where(face => face.Any(node => !coveredVertices.ContainsKey(node)))
                .ToList();

            chains = oldDecomposition.chains.Select(x => new Chain<TNode>(x.Nodes.ToList(), x.Number, x.IsFromFace))
                .ToList();
            chains.Add(new Chain<TNode>(chain, chains.Count, isFromFace));
        }

        public PartialDecomposition<TNode> AddChain(List<TNode> chain, bool isFromFace)
        {
            return new PartialDecomposition<TNode>(this, chain, isFromFace);
        }

        public PartialDecomposition<TNode> AddChain(ChainCandidate<TNode> chain)
        {
            return new PartialDecomposition<TNode>(this, chain.Nodes, chain.IsFromFace);
        }

        public List<TNode> GetAllCoveredVertices()
        {
            return coveredVertices.Keys.ToList();
        }

        public bool IsCovered(TNode node)
        {
            return coveredVertices.ContainsKey(node);
        }

        public int GetChainNumber(TNode node)
        {
            if (coveredVertices.ContainsKey(node))
            {
                return coveredVertices[node];
            }

            return -1;
        }

        public List<List<TNode>> GetRemainingFaces()
        {
            return remainingFaces.Select(x => x.ToList()).ToList();
        }

        public List<Chain<TNode>> GetFinalDecomposition()
        {
            return chains;
        }
    }
}