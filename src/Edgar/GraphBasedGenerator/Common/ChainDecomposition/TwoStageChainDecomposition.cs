using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Graphs;
using Edgar.Legacy.Core.ChainDecompositions;
using Edgar.Utils;

namespace Edgar.GraphBasedGenerator.Common.ChainDecomposition
{
    /// <inheritdoc />
	/// <summary>
	/// Chain decomposition for layout generators with two-stage generation.
	/// </summary>
    /// <typeparam name="TNode"></typeparam>
	public class TwoStageChainDecomposition<TNode> : IChainDecomposition<TNode>
	{
		private readonly ILevelDescription<TNode> levelDescription;
		private readonly IChainDecomposition<TNode> decomposition;

		public TwoStageChainDecomposition(ILevelDescription<TNode> levelDescription, IChainDecomposition<TNode> decomposition)
		{
			this.levelDescription = levelDescription;
			this.decomposition = decomposition;
		}

		/// <inheritdoc />
		public List<Chain<TNode>> GetChains(IGraph<TNode> graph)
		{
			// Get all the faces from the stage one graph
			var stageOneGraph = GraphAlgorithms.GetInducedSubgraph(
                levelDescription.GetGraphWithoutCorridors(), 
                graph.Vertices.ToHashSet(),
                new UndirectedAdjacencyListGraph<TNode>()
            );
			var faces = decomposition.GetChains(stageOneGraph);

			var usedVertices = new HashSet<TNode>();
			var notUsedStageTwoRooms = graph.Vertices.Where(x => levelDescription.GetRoomDescription(x).IsCorridor).ToList();

            // Iterate through all the faces, marking all the seen vertices
            // As soon as all the neighbors of a stage two room are used, add the stage two room to the current face
			foreach (var face in faces)
			{
				// TODO: weird ForEach
				face.Nodes.ToList().ForEach(x => usedVertices.Add(x));

				foreach (var stageTwoRoom in notUsedStageTwoRooms.ToList())
				{
					var neighbors = graph.GetNeighbors(stageTwoRoom).ToList();

                    if (neighbors.TrueForAll(x => usedVertices.Contains(x)))
                    {
                        notUsedStageTwoRooms.Remove(stageTwoRoom);
                        face.Nodes.Add(stageTwoRoom);
                    }
                }
            }

            // It must not happen that a stage two room is not in the decomposition
			if (notUsedStageTwoRooms.Count != 0)
				throw new ArgumentException();

			return faces;
		}
	}
}