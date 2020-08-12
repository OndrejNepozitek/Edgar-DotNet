using Edgar.Legacy.Core.ChainDecompositions;
using Edgar.Legacy.Core.ChainDecompositions.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Graphs;

namespace MapGeneration.Tests.Core.GraphDecomposition
{
	using System.Linq;
    using NUnit.Framework;

	public abstract class ChainDecomposersTests
	{
		private IChainDecomposition<int> chainDecomposition;

		[SetUp]
		public void SetUp()
		{
			CreateConcrete();
		}

		protected abstract void CreateConcrete();

		[Test]
		public void GetChains_BasicCounts()
		{
			{
				// Two C_3s connected by a common vertex
				var graph = new UndirectedAdjacencyListGraph<int>();

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

				var chains = chainDecomposition.GetChains(graph);

				Assert.IsTrue(graph.Vertices.SequenceEqualWithoutOrder(chains.SelectMany(x => x.Nodes).Distinct()));
			}

			{
				// Two intersecting paths
				var graph = new UndirectedAdjacencyListGraph<int>();
				Enumerable.Range(0, 7).ToList().ForEach(x => graph.AddVertex(x));

				graph.AddEdge(0, 1);
				graph.AddEdge(1, 2);
				graph.AddEdge(2, 3);
				graph.AddEdge(4, 1);
				graph.AddEdge(1, 5);
				graph.AddEdge(5, 6);

				var chains = chainDecomposition.GetChains(graph);

				Assert.IsTrue(graph.Vertices.SequenceEqualWithoutOrder(chains.SelectMany(x => x.Nodes).Distinct()));
			}
		}

		[TestFixture]
		public class BreadthFirstChainDecompositionTests : ChainDecomposersTests
		{
			protected override void CreateConcrete()
			{
				chainDecomposition = new BreadthFirstChainDecompositionOld<int>();
			}
		}
	}
}