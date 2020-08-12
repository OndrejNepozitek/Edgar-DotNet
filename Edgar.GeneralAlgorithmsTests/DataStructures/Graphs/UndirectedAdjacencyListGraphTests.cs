using Edgar.Legacy.GeneralAlgorithms.DataStructures.Graphs;

namespace GeneralAlgorithms.Tests.DataStructures.Graphs
{
    public class UndirectedAdjacencyListGraphTests : GraphTests
	{
		protected override void CreateConcrete()
		{
			graph = new UndirectedAdjacencyListGraph<int>();
		}
	}
}