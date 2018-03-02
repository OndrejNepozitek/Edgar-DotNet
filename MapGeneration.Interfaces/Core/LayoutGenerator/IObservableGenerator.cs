namespace MapGeneration.Interfaces.Core.LayoutGenerator
{
	using System;

	public interface IObservableGenerator<TMapDescription, TNode> : ILayoutGenerator<TMapDescription, TNode>
	{
		event Action<IMapLayout<TNode>> OnPerturbed;

		event Action<IMapLayout<TNode>> OnPartialValid;

		event Action<IMapLayout<TNode>> OnValid;
	}
}