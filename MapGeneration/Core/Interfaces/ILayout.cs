namespace MapGeneration.Core.Interfaces
{
	public interface ILayout<in TNode>
	{
		bool GetConfiguration(TNode node, out Configuration configuration);

		void SetConfiguration(TNode node, Configuration configuration);

		ILayout<TNode> Clone();
	}
}