using System.Collections.Generic;
using System.Linq;
using Edgar.Graphs;

namespace Edgar.GraphBasedGenerator.Common.ChainDecomposition
{
    public class FixedRoomChainDecompositionPreprocessing<TNode> : IChainDecomposition<TNode>
    {
        private readonly HashSet<TNode> fixedRooms;
        private readonly IChainDecomposition<TNode> chainDecomposition;

        public FixedRoomChainDecompositionPreprocessing(HashSet<TNode> fixedRooms, IChainDecomposition<TNode> chainDecomposition)
        {
            this.fixedRooms = fixedRooms;
            this.chainDecomposition = chainDecomposition;
        }

        public List<Chain<TNode>> GetChains(IGraph<TNode> graph)
        {
            // Invert the logic: isFixed -> isWalkable
            var isWalkable = new Dictionary<TNode, bool>();

            foreach (var node in graph.Vertices)
            {
                isWalkable[node] = !fixedRooms.Contains(node);
            }

            // Make fixed nodes that border with non-fixed nodes walkable
            isWalkable = ChainDecompositionUtils.MakeBorderNodesWalkable(graph, isWalkable);

            var components = ChainDecompositionUtils.GetWalkableComponents(graph, isWalkable);
            var listsOfChains = new List<List<Chain<TNode>>>();

            foreach (var component in components)
            {
                var nodes = new HashSet<TNode>();
                component.ForEach(x => nodes.Add(x));

                var subgraph = GraphAlgorithms.GetInducedSubgraph(graph, nodes, new BagOfEdgesGraph<TNode>());
                var chains = chainDecomposition.GetChains(graph);
                listsOfChains.Add(chains);
            }

            return listsOfChains.SelectMany(x => x).ToList();
        }
    }
}