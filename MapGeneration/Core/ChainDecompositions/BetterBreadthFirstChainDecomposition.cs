using MapGeneration.Interfaces.Core.ChainDecompositions;
using MapGeneration.Utils.Logging;

namespace MapGeneration.Core.ChainDecompositions
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.Algorithms.Common;
	using GeneralAlgorithms.DataStructures.Graphs;

	/// <summary>
	/// The algorithm starts with the smallest chains. It then looks for connected faces that
	/// are as small as possible. If no such face exists, paths close to already covered nodes
	/// are considered. When a face can be processed, it has greater priority than paths.
	/// </summary>
	/// <typeparam name="TNode"></typeparam>
	public class BetterBreadthFirstChainDecomposition<TNode> : ChainDecompositionBase<TNode>
		where TNode : IEquatable<TNode>
    {
        private readonly int maxTreeSize;
        private readonly bool mergeSmallChains;
        private readonly bool startTreeWithMultipleVertices;
        private readonly Logger logger;

		public BetterBreadthFirstChainDecomposition(int maxTreeSize, bool mergeSmallChains, bool startTreeWithMultipleVertices, Logger logger = null)
        {
            this.maxTreeSize = maxTreeSize;
            this.mergeSmallChains = mergeSmallChains;
            this.startTreeWithMultipleVertices = startTreeWithMultipleVertices;
            this.logger = logger ?? new Logger();
        }

		/// <inheritdoc />
		public override IList<IChain<TNode>> GetChains(IGraph<TNode> graph)
		{
			Initialize(graph);

			if (Faces.Count != 0)
			{
				// Get faces and remove the largest one
				Faces.RemoveAt(Faces.MaxBy(x => x.Count));
			}

			var decomposition = new PartialDecomposition(Faces);
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

        private PartialDecomposition GetFirstComponent(PartialDecomposition decomposition)
        {
            var faces = decomposition.GetRemainingFaces();
            if (Faces.Count != 0)
            {
                var smallestFaceIndex = faces.MinBy(x => x.Count);
                var smallestFace = faces[smallestFaceIndex];
                var cycleComponent =  new GraphComponent()
                {
                    Nodes = smallestFace,
                    IsFromFace = true,
                    MinimumNeighborChainNumber = 0,
                };

                logger.WriteLine("Starting with cycle");
                logger.WriteLine(cycleComponent);

                return decomposition.AddChain(cycleComponent.Nodes);
            }

            var startingNode = Graph.Vertices.First(x => Graph.GetNeighbours(x).Count() == 1);
            var treeComponent = GetTreeComponent(decomposition, startingNode);

            logger.WriteLine("Starting with tree");
            logger.WriteLine(treeComponent);

            return decomposition.AddChain(treeComponent.Nodes);
        }

        private PartialDecomposition ExtendDecomposition(PartialDecomposition decomposition)
        {
            var remainingFace = decomposition.GetRemainingFaces();
            var blacklist = new List<TNode>();
            var components = new List<GraphComponent>();

            foreach (var node in decomposition.GetAllCoveredVertices())
            {
                var neighbors = Graph.GetNeighbours(node);
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

                        var cycleComponent = GetCycleComponent(decomposition, face);
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
                    if (startTreeWithMultipleVertices)
                    {
                        var treeComponent = GetTreeComponent(decomposition, treeStartingNodes);
                        components.Add(treeComponent);
                        blacklist.AddRange(treeComponent.Nodes);
                    }
                    else
                    {
                        foreach (var startingNode in treeStartingNodes)
                        {
                            var treeComponent = GetTreeComponent(decomposition, startingNode);
                            components.Add(treeComponent);
                            blacklist.AddRange(treeComponent.Nodes);
                        }
                    }
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
                var smallestCycleIndex = cycleComponents.MinBy(x => x.Nodes.Count);
                var smallestCycle = cycleComponents[smallestCycleIndex];

                logger.WriteLine("Adding smallest cycle component");
                logger.WriteLine(smallestCycle);

                return decomposition.AddChain(smallestCycle.Nodes);
            }

            var treeComponents = components
                .Where(x => !x.IsFromFace)
                .OrderBy(x => x.MinimumNeighborChainNumber)
                .ThenByDescending(x => x.Nodes.Count)
                .ToList();

            var biggestTree = treeComponents[0];

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

            logger.WriteLine("Adding biggest oldest tree component");
            logger.WriteLine(biggestTree);

            return decomposition.AddChain(biggestTree.Nodes);
        }

        private GraphComponent GetTreeComponent(PartialDecomposition decomposition, List<TNode> startingNodes)
        {
            return GetDfsTreeComponent(decomposition, startingNodes);
        }

        private GraphComponent GetTreeComponent(PartialDecomposition decomposition, TNode startingNode)
        {
            return GetDfsTreeComponent(decomposition, new List<TNode>() { startingNode });
        }

        private GraphComponent GetDfsTreeComponent(PartialDecomposition decomposition, List<TNode> startingNodes)
        {
            var nodes = new List<TNode>();
            var queue = new Queue<TNode>();

            nodes.AddRange(startingNodes);
            foreach (var startingNode in startingNodes)
            {
                queue.Enqueue(startingNode);
            }

            while (queue.Count != 0 && nodes.Count < maxTreeSize)
            {
                var node = queue.Dequeue();

                if (decomposition.GetRemainingFaces().Any(x => x.Contains(node)))
                {
                    continue;
                }

                var neighbors = Graph.GetNeighbours(node);

                foreach (var neighbor in neighbors)
                {
                    if (!nodes.Contains(neighbor) && !decomposition.IsCovered(neighbor))
                    {
                        nodes.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return new GraphComponent()
            {
                Nodes = nodes,
                IsFromFace = false,
                MinimumNeighborChainNumber = GetMinimumNeighborChainNumber(decomposition, nodes),
            };
        }

        private int GetMinimumNeighborChainNumber(PartialDecomposition decomposition, List<TNode> nodes)
        {
            var coveredNeighbors = nodes
                .SelectMany(Graph.GetNeighbours)
                .Where(decomposition.IsCovered)
                .ToList();

            if (coveredNeighbors.Count != 0)
            {
                return coveredNeighbors.Min(decomposition.GetChainNumber);
            }

            return -1;
        }

        private GraphComponent GetCycleComponent(PartialDecomposition decomposition, List<TNode> face)
        {
            var nodes = new List<TNode>();
            var notCoveredNodes = face.Where(x => !decomposition.IsCovered(x)).ToList();
            var nodeOrder = new Dictionary<TNode, int>();

            while (notCoveredNodes.Count != 0)
            {
                var nodeIndex = notCoveredNodes
                    .MinBy(
                        x => Graph
                            .GetNeighbours(x)
                            .Min(y =>
                                decomposition.IsCovered(y) 
                                    ? -1 
                                    : nodeOrder.ContainsKey(y) ? nodeOrder[y] : int.MaxValue));

                nodeOrder[notCoveredNodes[nodeIndex]] = nodeOrder.Count;
                nodes.Add(notCoveredNodes[nodeIndex]);
                notCoveredNodes.RemoveAt(nodeIndex);
            }

            return new GraphComponent()
            {
                Nodes = nodes,
                IsFromFace = true,
                MinimumNeighborChainNumber = GetMinimumNeighborChainNumber(decomposition, nodes),
            };
        }

        private class GraphComponent
        {
            public List<TNode> Nodes { get; set; }

            public bool IsFromFace { get; set; }

            public int MinimumNeighborChainNumber { get; set; }

            public override string ToString()
            {
                return $"{(IsFromFace ? "cycle" : "tree")} {MinimumNeighborChainNumber} [{string.Join(",", Nodes)}]";
            }
        }

        private class PartialDecomposition
        {
            private readonly Dictionary<TNode, int> coveredVertices;
            private readonly List<List<TNode>> remainingFaces;
            private readonly List<List<TNode>> chains;

            public int NumberOfChains => coveredVertices.Count == 0 ? 0 : coveredVertices.Values.Max() + 1;

            public PartialDecomposition(List<List<TNode>> faces)
            {
                this.remainingFaces = faces;
                coveredVertices = new Dictionary<TNode, int>();
                chains = new List<List<TNode>>();
            }

            private PartialDecomposition(PartialDecomposition oldDecomposition, List<TNode> chain)
            {
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

                chains = oldDecomposition.chains.Select(x => x.ToList()).ToList();
                chains.Add(chain);
            }

            public PartialDecomposition AddChain(List<TNode> chain)
            {
				return new PartialDecomposition(this, chain);
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

            public List<IChain<TNode>> GetFinalDecomposition()
            {
                var chains = new List<IChain<TNode>>();

                for (var i = 0; i < this.chains.Count; i++)
                {
                    var chain = this.chains[i];
                    chains.Add(new Chain<TNode>(chain, i));
                }

                return chains;
            }
        }
    }
}