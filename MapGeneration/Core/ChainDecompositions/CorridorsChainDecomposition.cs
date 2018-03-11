namespace MapGeneration.Core.ChainDecompositions
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.DataStructures.Graphs;
	using Interfaces.Core.ChainDecompositions;
	using Interfaces.Core.MapDescription;

	/// <inheritdoc />
	/// <summary>
	/// Chain decomposition for map descriptions with corridors.
	/// </summary>
	/// <remarks>
	/// It uses a chain decomposition for normal map description with a graph without corridors.
	/// It then adds all the corridors to corresponding chains. The benefit of this approach is
	/// that graphs with corridors have very specific structure and classic chain decompositions
	/// may not give optimal results. By removing corridors, we can use classic chain decompositions.
	/// </remarks>
	/// <typeparam name="TNode"></typeparam>
	public class CorridorsChainDecomposition<TNode> : IChainDecomposition<TNode>
	{
		private readonly ICorridorMapDescription<TNode> mapDescription;
		private readonly IChainDecomposition<TNode> decomposition;

		public CorridorsChainDecomposition(ICorridorMapDescription<TNode> mapDescription, IChainDecomposition<TNode> decomposition)
		{
			this.mapDescription = mapDescription;
			this.decomposition = decomposition;
		}

		/// <inheritdoc />
		public List<List<TNode>> GetChains(IGraph<TNode> graph)
		{
			if (!mapDescription.IsWithCorridors)
				throw new InvalidOperationException("Map description must be with corridors to use this decomposition.");

			var graphWithoutCorridors = mapDescription.GetGraphWithoutCorrridors();
			var faces = decomposition.GetChains(graphWithoutCorridors);

			var usedVertices = new HashSet<TNode>();
			var corridors = graph.Vertices.Where(x => mapDescription.IsCorridorRoom(x)).ToList();

			foreach (var face in faces)
			{
				face.ForEach(x => usedVertices.Add(x));

				var corridorsToRemove = new List<TNode>();
				foreach (var corridor in corridors)
				{
					var neighbours = graph.GetNeighbours(corridor).ToList();

					if (neighbours.Count != 2)
						throw new ArgumentException("Every corridor must have exactly two neighbours");

					if (usedVertices.Contains(neighbours[0]) && usedVertices.Contains(neighbours[1]))
					{
						corridorsToRemove.Add(corridor);
						face.Add(corridor);
					}
				}
				corridorsToRemove.ForEach(x => corridors.Remove(x));
			}

			if (corridors.Count != 0)
				throw new ArgumentException();

			return faces;
		}
	}
}