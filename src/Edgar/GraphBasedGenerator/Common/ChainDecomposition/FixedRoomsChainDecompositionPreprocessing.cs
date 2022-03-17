using System.Collections.Generic;
using System.Linq;
using Edgar.Graphs;

namespace Edgar.GraphBasedGenerator.Common.ChainDecomposition
{
    public class FixedRoomsChainDecompositionPreprocessing<TNode> : IChainDecomposition<TNode>
    {
        private readonly List<TNode> fixedRooms;
        private readonly IChainDecomposition<TNode> chainDecomposition;

        public FixedRoomsChainDecompositionPreprocessing(List<TNode> fixedRooms,
            IChainDecomposition<TNode> chainDecomposition)
        {
            this.fixedRooms = fixedRooms;
            this.chainDecomposition = chainDecomposition;
        }

        public List<Chain<TNode>> GetChains(IGraph<TNode> graph)
        {
            if (fixedRooms.Count == 0)
            {
                return chainDecomposition.GetChains(graph);
            }

            // Invert the logic: isFixed -> isWalkable
            var isWalkable = new Dictionary<TNode, bool>();

            foreach (var node in graph.Vertices)
            {
                isWalkable[node] = true;
            }

            foreach (var node in fixedRooms)
            {
                isWalkable[node] = false;
            }

            // Make fixed nodes that border with non-fixed nodes walkable
            isWalkable = ChainDecompositionGraphUtils.MakeBorderNodesWalkable(graph, isWalkable);

            var components = ChainDecompositionGraphUtils.GetWalkableComponents(graph, isWalkable);
            var listsOfChains = new List<List<Chain<TNode>>>();

            foreach (var component in components)
            {
                var nodes = new HashSet<TNode>();
                component.ForEach(x => nodes.Add(x));

                var subgraph =
                    GraphAlgorithms.GetInducedSubgraph(graph, nodes, new UndirectedAdjacencyListGraph<TNode>());
                var chains = chainDecomposition.GetChains(subgraph);
                listsOfChains.Add(chains);
            }

            return listsOfChains.SelectMany(x => x).ToList();
        }
    }
}