using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Legacy.Core.ChainDecompositions;
using Edgar.Legacy.Core.ChainDecompositions.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Graphs;

namespace Edgar.GraphBasedGenerator.Common
{
    /// <inheritdoc />
	/// <summary>
	/// Chain decomposition for layout generators with two-stage generation.
	/// </summary>
    /// <typeparam name="TNode"></typeparam>
	public class TwoStageChainDecomposition<TNode> : IChainDecomposition<TNode>
	{
		private readonly ILevelDescription<TNode> mapDescription;
		private readonly IChainDecomposition<TNode> decomposition;

		public TwoStageChainDecomposition(ILevelDescription<TNode> mapDescription, IChainDecomposition<TNode> decomposition)
		{
			this.mapDescription = mapDescription;
			this.decomposition = decomposition;
		}

		/// <inheritdoc />
		public List<Chain<TNode>> GetChains(IGraph<TNode> graph)
		{
            // Get all the faces from the stage one graph
			var stageOneGraph = mapDescription.GetGraphWithoutCorridors();
			var faces = decomposition.GetChains(stageOneGraph);

			var usedVertices = new HashSet<TNode>();
			var notUsedStageTwoRooms = graph.Vertices.Where(x => mapDescription.GetRoomDescription(x).IsCorridor).ToList();

            // Iterate through all the faces, marking all the seen vertices
            // As soon as all the neighbors of a stage two room are used, add the stage two room to the current face
			foreach (var face in faces)
			{
				// TODO: weird ForEach
				face.Nodes.ToList().ForEach(x => usedVertices.Add(x));

				foreach (var stageTwoRoom in notUsedStageTwoRooms.ToList())
				{
					var neighbors = graph.GetNeighbours(stageTwoRoom).ToList();

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