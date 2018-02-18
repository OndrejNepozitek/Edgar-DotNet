namespace MapGeneration.Tests.Core.GraphDecomposition
{
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.Algorithms.Common;
	using GeneralAlgorithms.Algorithms.Graphs.GraphDecomposition;
	using GeneralAlgorithms.DataStructures.Graphs;
	using MapGeneration.Core.GraphDecomposition;
	using MapGeneration.Core.Interfaces;
	using NUnit.Framework;

	[TestFixture]
	public class ChainDecomposersTests
	{
		private List<IChainDecomposition<int>> chainDecompositions;

		[SetUp]
		public void SetUp()
		{
			var graphDecomposer = new GraphDecomposer<int>();

			chainDecompositions = new List<IChainDecomposition<int>>()
			{
				new BasicChainsDecomposition<int>(graphDecomposer),
				new BreadthFirstLongerChainsDecomposition<int>(graphDecomposer),
				new LongerChainsDecomposition<int>(graphDecomposer)
			};
		}

		[Test]
		public void GetChains_BasicCounts()
		{
			foreach (var decomposition in chainDecompositions)
			{
				{
					// Two C_3s connected by a common vertex
					var graph = new FastGraph<int>(5);

					graph.AddVertex(0);
					graph.AddVertex(1);
					graph.AddVertex(2);
					graph.AddVertex(3);
					graph.AddVertex(4);

					graph.AddEdge(0, 1);
					graph.AddEdge(1, 2);
					graph.AddEdge(2, 0);
					graph.AddEdge(1, 3);
					graph.AddEdge(3, 4);
					graph.AddEdge(4, 1);

					var chains = decomposition.GetChains(graph);

					Assert.IsTrue(graph.Vertices.SequenceEqualWithoutOrder(chains.SelectMany(x => x).Distinct()));
				}

				{
					// Two intersecting paths
					var graph = new FastGraph<int>(7);
					Enumerable.Range(0, 7).ToList().ForEach(x => graph.AddVertex(x));

					graph.AddEdge(0, 1);
					graph.AddEdge(1, 2);
					graph.AddEdge(2, 3);
					graph.AddEdge(4, 1);
					graph.AddEdge(1, 5);
					graph.AddEdge(5, 6);

					var chains = decomposition.GetChains(graph);

					Assert.IsTrue(graph.Vertices.SequenceEqualWithoutOrder(chains.SelectMany(x => x).Distinct()));
				}
			}
		}
	}
}