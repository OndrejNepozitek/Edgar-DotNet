using System.Collections.Generic;
using System.Linq;
using Edgar.Graphs;
using Edgar.Legacy.Core.ChainDecompositions;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Graphs;
using Edgar.Legacy.Utils.Logging;

namespace Edgar.GraphBasedGenerator.Common.ChainDecomposition
{
    public class ChainDecomposition<TNode> : IChainDecomposition<TNode>
    {
        protected readonly GraphUtils GraphUtils = new GraphUtils();
        protected readonly int MaxTreeSize;

        public ChainDecomposition(ChainDecompositionConfiguration configuration, Logger logger = null, List<TNode> fixedRooms = null)
        {
            this.MaxTreeSize = configuration.MaxTreeSize;
        }

        public List<Chain<TNode>> GetChains(IGraph<TNode> graph)
        {
            var faces = GetPlanarFaces(graph);
            var decomposition = new PartialDecomposition<TNode>(faces, graph);

            var initialChain = GetInitialChain(decomposition);
            decomposition = decomposition.AddChain(initialChain);

            // TODO: add a helper method that checks this
            while (decomposition.GetAllCoveredVertices().Count != graph.VerticesCount)
            {
                var nextChain = GetNextChain(decomposition);
                decomposition = decomposition.AddChain(nextChain);
            }

            var chains = decomposition.GetFinalDecomposition();

            return chains;
        }

        protected virtual ChainCandidate<TNode> GetInitialChain(PartialDecomposition<TNode> decomposition)
        {
            if (decomposition.GetRemainingFaces().Count != 0)
            {
                return GetInitialCycleChain(decomposition);
            }

            return GetInitialTreeChain(decomposition);
        }

        protected virtual ChainCandidate<TNode> GetInitialCycleChain(PartialDecomposition<TNode> decomposition)
        {
            var faces = decomposition.GetRemainingFaces();
            var smallestFace = faces.MinElement(x => x.Count);

            var cycleComponent = new ChainCandidate<TNode>()
            {
                Nodes = smallestFace,
                IsFromFace = true,
                MinimumNeighborChainNumber = 0,
            };

            return cycleComponent;
        }

        protected virtual ChainCandidate<TNode> GetInitialTreeChain(PartialDecomposition<TNode> decomposition)
        {
            var graph = decomposition.Graph;
            var startingNode = graph
                .Vertices
                .First(x => graph.GetNeighbors(x).Count() == 1);
            var treeComponent = ChainDecompositionUtils.GetBfsTreeCandidate(
                decomposition,
                new List<TNode>() { startingNode },
                MaxTreeSize);

            return treeComponent;
        }

        protected virtual ChainCandidate<TNode> GetNextChain(PartialDecomposition<TNode> decomposition)
        {
            var remainingFaces = decomposition.GetRemainingFaces();
            var blacklist = new List<TNode>();
            var components = new List<ChainCandidate<TNode>>();

            foreach (var node in decomposition.GetAllCoveredVertices())
            {
                var neighbors = decomposition.Graph.GetNeighbors(node);
                var notCoveredNeighbors = neighbors.Where(x => !decomposition.IsCovered(x));
                var treeStartingNodes = new List<TNode>();

                foreach (var neighbor in notCoveredNeighbors)
                {
                    if (blacklist.Contains(neighbor))
                    {
                        continue;
                    }

                    var foundFace = false;
                    foreach (var face in remainingFaces)
                    {
                        if (!face.Contains(neighbor))
                        {
                            continue;
                        }

                        var cycleComponent = ChainDecompositionUtils.GetCycleComponent(decomposition, face);
                        components.Add(cycleComponent);
                        blacklist.AddRange(cycleComponent.Nodes);
                        foundFace = true;

                        break;
                    }

                    if (foundFace)
                    {
                        continue;
                    }

                    treeStartingNodes.Add(neighbor);
                }

                if (treeStartingNodes.Count != 0)
                {
                    var treeComponent = ChainDecompositionUtils.GetBfsTreeCandidate(decomposition, treeStartingNodes, MaxTreeSize);
                    components.Add(treeComponent);
                    blacklist.AddRange(treeComponent.Nodes);
                }
            }

            var cycleComponents = components.Where(x => x.IsFromFace).ToList();
            if (cycleComponents.Count != 0)
            {
                var nextCycleIndex = cycleComponents.MinBy(x => x.Nodes.Count);
                var nextCycle = cycleComponents[nextCycleIndex];

                return nextCycle;
            }

            var treeComponents = components
                .Where(x => !x.IsFromFace)
                .OrderBy(x => x.MinimumNeighborChainNumber)
                .ThenByDescending(x => x.Nodes.Count)
                .ToList();

            var biggestTree = treeComponents[0];

            if (true)
            {
                if (biggestTree.Nodes.Count < MaxTreeSize)
                {
                    for (var i = 1; i < treeComponents.Count; i++)
                    {
                        var component = treeComponents[i];

                        if (component.Nodes.Count + biggestTree.Nodes.Count <= MaxTreeSize)
                        {
                            biggestTree.Nodes.AddRange(component.Nodes);
                        }
                    }
                }
            }

            return biggestTree;
        }

        private List<List<TNode>> GetPlanarFaces(IGraph<TNode> graph)
        {
            var faces = GraphUtils.GetPlanarFaces(graph);

            if (faces.Count != 0)
            {
                // Get faces and remove the largest one
                faces.RemoveAt(faces.MaxBy(x => x.Count));
            }

            return faces;
        }
    }
}