namespace MapGeneration.Core.GraphDecomposition
{
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.Algorithms.Graphs.GraphDecomposition;
	using GeneralAlgorithms.DataStructures.Graphs;
	using Interfaces.Core;

	public abstract class ChainDecompositionBase<TNode> : IChainDecomposition<TNode>
	{
		protected IGraph<TNode> Graph;
		protected IGraphDecomposer<TNode> GraphDecomposer;
		protected Dictionary<TNode, int> CoveredVertices;
		protected int ChainsCounter;
		protected List<List<TNode>> Faces;

		protected ChainDecompositionBase(IGraphDecomposer<TNode> graphDecomposer)
		{
			GraphDecomposer = graphDecomposer;
		}

		public abstract List<List<TNode>> GetChains(IGraph<TNode> graph);

		/// <summary>
		/// Setups graph and gets faces.
		/// </summary>
		/// <param name="graph"></param>
		protected void Initialize(IGraph<TNode> graph)
		{
			Graph = graph;
			Faces = GraphDecomposer.GetFaces(Graph);
			CoveredVertices = new Dictionary<TNode, int>();

			// Initialize all vertices to the -1 depth
			graph.Vertices.ToList().ForEach(x => SetDepth(x, -1));
		}

		protected int SmallestUsedNeighbourDepth(TNode node)
		{
			var neighbours = Graph.GetNeighbours(node);
			var smallestDepth = int.MaxValue;

			foreach (var neighbour in neighbours)
			{
				var depth = GetDepth(neighbour);

				if (depth != -1 && depth < smallestDepth)
				{
					smallestDepth = depth;
				}
			}

			return smallestDepth;
		}

		protected int GetDepth(TNode node)
		{
			return CoveredVertices[node];
		}

		protected void SetDepth(TNode node, int depth)
		{
			CoveredVertices[node] = depth;
		}

		protected bool IsCovered(TNode node)
		{
			return GetDepth(node) != -1;
		}

		protected int UncoveredNeighboursCount(TNode node)
		{
			var neighbours = Graph.GetNeighbours(node);

			return neighbours.Count(x => !IsCovered(x));
		}
	}
}