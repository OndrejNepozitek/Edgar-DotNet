namespace GeneralAlgorithms.Tests.DataStructures.Graphs
{
	using GeneralAlgorithms.DataStructures.Graphs;

	public class UndirectedAdjacencyListGraphTests : GraphTests
	{
		protected override void CreateConcrete()
		{
			graph = new UndirectedAdjacencyListGraph<int>();
		}
	}
}