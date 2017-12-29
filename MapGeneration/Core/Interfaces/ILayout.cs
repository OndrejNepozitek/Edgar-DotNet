namespace MapGeneration.Core.Interfaces
{
	using GeneralAlgorithms.DataStructures.Graphs;

	public interface ILayout<TNode>
	{
		IGraph<TNode> Graph { get; }

		bool GetConfiguration(TNode node, out Configuration configuration);

		void SetConfiguration(TNode node, Configuration configuration);

		ILayout<TNode> Clone();
	}
}