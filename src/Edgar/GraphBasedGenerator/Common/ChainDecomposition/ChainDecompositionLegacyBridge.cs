using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.GraphBasedGenerator.Common.ChainDecomposition.Legacy;
using Edgar.Graphs;
using Edgar.Legacy.Core.ChainDecompositions;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.Utils.Logging;

namespace Edgar.GraphBasedGenerator.Common.ChainDecomposition
{
    public class ChainDecompositionLegacyBridge<TNode> : ChainDecompositionBase<TNode>
    {
        private readonly List<TNode> fixedRooms;
        private readonly int maxTreeSize;
        private readonly bool mergeSmallChains;
        private readonly bool startTreeWithMultipleVertices;
        private readonly TreeComponentStrategy treeComponentStrategy;
        private readonly Logger logger;

        public ChainDecompositionLegacyBridge(ChainDecompositionConfiguration configuration, Logger logger = null,
            List<TNode> fixedRooms = null)
        {
            this.fixedRooms = fixedRooms;
            this.maxTreeSize = configuration.MaxTreeSize;
            this.mergeSmallChains = configuration.MergeSmallChains;
            this.startTreeWithMultipleVertices = configuration.StartTreeWithMultipleVertices;
            this.treeComponentStrategy = configuration.TreeComponentStrategy;
            this.logger = logger ?? new Logger();
        }

        /// <inheritdoc />
        public override List<Chain<TNode>> GetChains(IGraph<TNode> graph)
        {
            Initialize(graph);

            if (Faces.Count != 0)
            {
                // Get faces and remove the largest one
                Faces.RemoveAt(Faces.MaxBy(x => x.Count));
            }

            var decomposition = new PartialDecomposition<TNode>(Faces, Graph);
            decomposition = GetFirstComponent(decomposition);

            while (decomposition.GetAllCoveredVertices().Count != Graph.VerticesCount)
            {
                decomposition = ExtendDecomposition(decomposition);
            }

            var chains = decomposition.GetFinalDecomposition();
            logger.WriteLine("Final decomposition:");
            foreach (var chain in chains)
            {
                logger.WriteLine($"[{string.Join(",", chain.Nodes)}]");
            }

            return chains;
        }

        private PartialDecomposition<TNode> GetFirstComponent(PartialDecomposition<TNode> decomposition)
        {
            var graph = decomposition.Graph;
            var initialFixedChain = ChainDecompositionUtils.GetInitialFixedComponent(graph, fixedRooms);

            if (initialFixedChain != null)
            {
                return decomposition.AddChain(initialFixedChain, true);
            }

            var faces = decomposition.GetRemainingFaces();
            if (faces.Count != 0)
            {
                var firstFace = faces.MinElement(x => x.Count);

                var cycleComponent = new ChainCandidate<TNode>()
                {
                    Nodes = firstFace,
                    IsFromFace = true,
                    MinimumNeighborChainNumber = 0,
                };

                return decomposition.AddChain(cycleComponent.Nodes, true);
            }

            var startingNode = graph
                .Vertices.First(x => graph
                    .GetNeighbors(x).Count() == 1);
            var treeComponent = GetTreeComponent(decomposition, new List<TNode>() {startingNode});

            return decomposition.AddChain(treeComponent.Nodes, false);
        }

        private PartialDecomposition<TNode> ExtendDecomposition(PartialDecomposition<TNode> decomposition)
        {
            var remainingFace = decomposition.GetRemainingFaces();
            var blacklist = new List<TNode>();
            var components = new List<ChainCandidate<TNode>>();

            foreach (var node in decomposition.GetAllCoveredVertices())
            {
                var neighbors = Graph.GetNeighbors(node);
                var notCoveredNeighbors = neighbors.Where(x => !decomposition.IsCovered(x));
                var treeStartingNodes = new List<TNode>();

                foreach (var neighbor in notCoveredNeighbors)
                {
                    if (blacklist.Contains(neighbor))
                    {
                        continue;
                    }

                    var foundFace = false;
                    foreach (var face in remainingFace)
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
                    var treeComponent = GetTreeComponent(decomposition, treeStartingNodes);
                    components.Add(treeComponent);
                    blacklist.AddRange(treeComponent.Nodes);
                }
            }

            logger.WriteLine();
            logger.WriteLine("Extending decomposition");
            logger.WriteLine("Candidates:");

            foreach (var component in components)
            {
                logger.WriteLine(component);
            }

            logger.WriteLine();

            var cycleComponents = components.Where(x => x.IsFromFace).ToList();
            if (cycleComponents.Count != 0)
            {
                var nextCycleIndex = cycleComponents.MinBy(x => x.Nodes.Count);
                var nextCycle = cycleComponents[nextCycleIndex];

                logger.WriteLine("Adding smallest cycle component");
                logger.WriteLine(nextCycle);

                return decomposition.AddChain(nextCycle.Nodes, true);
            }

            var treeComponents = components
                .Where(x => !x.IsFromFace)
                .OrderBy(x => x.MinimumNeighborChainNumber)
                .ThenByDescending(x => x.Nodes.Count)
                .ToList();

            var biggestTree = treeComponents[0];

            if (mergeSmallChains)
            {
                if (biggestTree.Nodes.Count < maxTreeSize)
                {
                    for (var i = 1; i < treeComponents.Count; i++)
                    {
                        var component = treeComponents[i];

                        if (component.Nodes.Count + biggestTree.Nodes.Count <= maxTreeSize)
                        {
                            biggestTree.Nodes.AddRange(component.Nodes);
                        }
                    }
                }
            }

            logger.WriteLine("Adding biggest oldest tree component");
            logger.WriteLine(biggestTree);

            return decomposition.AddChain(biggestTree.Nodes, false);
        }

        private ChainCandidate<TNode> GetTreeComponent(PartialDecomposition<TNode> decomposition,
            List<TNode> startingNodes)
        {
            return ChainDecompositionUtils.GetBfsTreeCandidate(decomposition, startingNodes, maxTreeSize);
        }
    }
}