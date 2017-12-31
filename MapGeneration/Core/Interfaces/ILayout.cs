namespace MapGeneration.Core.Interfaces
{
	using GeneralAlgorithms.DataStructures.Graphs;

	public interface ILayout<TNode, TConfiguration>
	{
		IGraph<TNode> Graph { get; }

		bool GetConfiguration(TNode node, out TConfiguration configuration);

		void SetConfiguration(TNode node, TConfiguration configuration);

		ILayout<TNode, TConfiguration> Clone();
	}
}