using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Graphs;

namespace Edgar.Legacy.Core.ChainDecompositions
{
    /// <summary>
	/// The algorithm starts with the smallest chains. It then looks for connected faces that
	/// are as small as possible. If no such face exists, paths close to already covered nodes
	/// are considered. When a face can be processed, it has greater priority than paths.
	/// </summary>
	/// <typeparam name="TNode"></typeparam>
	public class BreadthFirstChainDecompositionOld<TNode> : ChainDecompositionBase<TNode>
    {
		private readonly bool groupSoloVertices;

		public BreadthFirstChainDecompositionOld(bool groupSoloVertices = true)
		{
			this.groupSoloVertices = groupSoloVertices;
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

			var chains = new List<Chain<TNode>>();

			if (Faces.Count != 0)
			{
				chains.Add(new Chain<TNode>(GetFirstCycle(Faces), chains.Count));
			}

			// Process cycles
			while (Faces.Count != 0)
			{
				var chain = GetNextCycle();
                var isFromCycle = chain != null;

				if (chain == null)
				{
					chain = GetNextPath();
				}

				// Must not happen. There must always be a cycle or a path while we have at least one face available.
				if (chain == null)
					throw new InvalidOperationException();

				chains.Add(new Chain<TNode>(chain, chains.Count)
                {
					IsFromFace = isFromCycle,
                });
			}

			// Add remaining nodes
			while (Graph.Vertices.Any(x => !IsCovered(x)))
			{
				var chain = GetNextPath();

				// Must not happen. There must always be a path while there are vertices that are not covered. (The graph must be connected)
				if (chain == null)
					throw new InvalidOperationException();

				chains.Add(new Chain<TNode>(chain, chains.Count)
                {
					IsFromFace = false,
                });
			}

			return chains;
		}

		/// <summary>
		/// Tries to find a face that neighbours with covered vertices in the smallest depth.
		/// Chooses the smallest one if there are multiple in the same depth.
		/// </summary>
		/// <returns></returns>
		private List<TNode> GetNextCycle()
		{
			var bestFaceIndex = -1;
			var bestFaceSize = int.MaxValue;
			var bestFaceDepth = int.MaxValue;

			for (var i = 0; i < Faces.Count; i++)
			{
				var face = Faces[i];

				// Check nodes in the face and also all neighbouring nodes
				var nodesToCheck = face.Concat(face.SelectMany(x => Graph.GetNeighbours(x))).Where(IsCovered).ToList();

				// If nodesToCheck is empty, it does not neighbour with any covered node and therefore we skip it
				if (nodesToCheck.Count == 0)
					continue;

				var minDepth = nodesToCheck.Min(GetDepth);
				var size = face.Count(x => !IsCovered(x));

				if (minDepth < bestFaceDepth || (minDepth == bestFaceDepth && size < bestFaceSize))
				{
					bestFaceIndex = i;
					bestFaceSize = size;
					bestFaceDepth = minDepth;
				}
			}

			// Return null if no face was found. That means that we must now consider paths and then come back to cycles.
			if (bestFaceIndex == -1)
				return null;

			var nextFace = Faces[bestFaceIndex].Where(x => !IsCovered(x)).ToList();
			Faces.RemoveAt(bestFaceIndex);

			// This must not happen as all faces with all nodes covered must be already removed
			if (nextFace.Count == 0)
				throw new InvalidOperationException();

			var chain = new List<TNode>();
			var counter = ChainsCounter;

			// Find a vertex that neighbours with a covered node
			var firstVertexIndex = -1;
			for (var i = 0; i < nextFace.Count; i++)
			{
				var vertex = nextFace[i];

				if (Graph.GetNeighbours(vertex).Any(IsCovered))
				{
					firstVertexIndex = i;
					break;
				}
			}

			// This must not happen as we considered only faces that neighbour with already covered vertices
			if (firstVertexIndex == -1)
				throw new InvalidOperationException();

			// Use the first vertex
			SetDepth(nextFace[firstVertexIndex], counter++);
			chain.Add(nextFace[firstVertexIndex]);
			nextFace.RemoveAt(firstVertexIndex);

			// Add vertices starting with the ones that neighbour with nodes on the highest depths
			while (nextFace.Count != 0)
			{
				var nextVertexIndex = nextFace.MinBy(SmallestCoveredNeighbourDepth);
				var nextVertex = nextFace[nextVertexIndex];

				SetDepth(nextVertex, counter++);
				chain.Add(nextVertex);
				nextFace.Remove(nextVertex);
			}

			// Fix depths
			foreach (var node in chain)
			{
				SetDepth(node, ChainsCounter);
			}

			ChainsCounter++;
			RemoveCoveredNodes(Faces);

			return chain;
		}

		/// <summary>
		/// Gets a path that is conencted to already discovered vertices.
		/// </summary>
		/// <remarks>
		/// It tries to minimize the number of chains with only a single node.
		/// </remarks>
		/// <returns></returns>
		private List<TNode> GetNextPath()
		{
			var firstVertex = default(TNode);
			var firstDepth = int.MaxValue;
			var foundFirst = false;

			// Save a node that is the covered neighbour of the firstVertex
			var hasOrigin = false;
			var origin = default(TNode);

			// Check if there is at least one covered node
			if (Graph.Vertices.Any(IsCovered))
			{
				foreach (var node in Graph.Vertices.Where(x => !IsCovered(x)))
				{
					var coveredNeighbours = Graph.GetNeighbours(node).Where(IsCovered).ToList();

					if (coveredNeighbours.Count == 0)
						continue;

					var minDepthIndex = coveredNeighbours.MinBy(GetDepth);
					var minDepth = GetDepth(coveredNeighbours[minDepthIndex]);

					if (minDepth < firstDepth)
					{
						firstVertex = node;
						firstDepth = minDepthIndex;
						foundFirst = true;

						hasOrigin = true;
						origin = coveredNeighbours[minDepthIndex];
					}
				}
			}
			else
			{
				// If there are no covered nodes, find a one that is a leaf
				firstVertex = Graph.Vertices.First(x => Graph.GetNeighbours(x).Count() == 1);
				foundFirst = true;
			}

			// Must not happen
			if (!foundFirst)
				throw new InvalidOperationException();

			var chain = new List<TNode>();
			chain.Add(firstVertex);
			SetDepth(firstVertex, ChainsCounter);

			var nodeInFaces = false;

			// Check if we have an origin vertex and if the first vertex has any uncovered neighbour.
			// If it does not have any uncovered neighbour, it will normally form a chain of the lenght 1.
			// We want to prevent it and check if the origin node has any neighbour with the same characteristics.
			if (groupSoloVertices && hasOrigin && UncoveredNeighboursCount(firstVertex) == 0)
			{
				var soloNeighbours = GetSoloNeighbours(origin, out nodeInFaces);

				foreach (var neighbour in soloNeighbours)
				{
					chain.Add(neighbour);
					SetDepth(neighbour, ChainsCounter);
				}
			}

			while (true)
			{
				var lastNode = chain[chain.Count - 1];
				var neighbours = Graph.GetNeighbours(lastNode).Where(x => !IsCovered(x)).ToList();

				// Break if there are not neigbours
				if (neighbours.Count == 0)
					break;

				if (groupSoloVertices)
				{
					var soloNeighbours = GetSoloNeighbours(lastNode, out var nodeInFacesLocal);

					if (nodeInFacesLocal)
					{
						nodeInFaces = true;
					}

					// Add solo neighbours as above.
					// Break if we found at least one.
					if (soloNeighbours.Count != 0)
					{
						foreach (var neighbour in soloNeighbours)
						{
							chain.Add(neighbour);
							SetDepth(neighbour, ChainsCounter);
						}

						break;
					}
				}

				var nextNode = neighbours[0];
				if (Faces.Any(x => x.Contains(nextNode)))
				{
					nodeInFaces = true;
				}

				chain.Add(nextNode);
				SetDepth(nextNode, ChainsCounter);

				// Break if we found a node that is contained in a face.
				// We do not want this path to continue with that face.
				if (nodeInFaces)
					break;
			}

			ChainsCounter++;
			return chain;
		}

		/// <summary>
		/// Gets neighbours that do not have any uncovered neighbours and are not contained in any face.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="nodeInFaces">Whether we found a node that is contained in a face.</param>
		/// <returns></returns>
		private List<TNode> GetSoloNeighbours(TNode node, out bool nodeInFaces)
		{
			var soloNeighbours = new List<TNode>();
			nodeInFaces = false;

			foreach (var neighbour in Graph.GetNeighbours(node).Where(x => !IsCovered(x)))
			{
				if (UncoveredNeighboursCount(neighbour) == 0)
				{
					if (Faces.Any(x => x.Contains(neighbour)))
					{
						nodeInFaces = true;
						continue;
					}

					soloNeighbours.Add(neighbour);
				}
			}

			return soloNeighbours;
		}

		/// <summary>
		/// Gets the first cycle.
		/// </summary>
		/// <param name="faces"></param>
		/// <returns></returns>
		private List<TNode> GetFirstCycle(List<List<TNode>> faces)
		{
			if (faces.Count == 0)
				throw new ArgumentException();

			var smallestIndex = faces.MinBy(x => x.Count);
			var smallestFace = faces[smallestIndex];

			foreach (var node in smallestFace)
			{
				SetDepth(node, ChainsCounter);
			}

			ChainsCounter++;
			faces.RemoveAt(smallestIndex);
			RemoveCoveredNodes(faces);

			return smallestFace;
		}

		/// <summary>
		/// Removes all faces that have all nodes already covered.
		/// </summary>
		/// <param name="faces"></param>
		private void RemoveCoveredNodes(List<List<TNode>> faces)
		{
			// Loops are reversed because removing elements changes index
			for (var i = faces.Count - 1; i >= 0; i--)
			{
				var face = faces[i];

				if (face.TrueForAll(IsCovered))
				{
					faces.RemoveAt(i);
				}
			}
		}
	}
}