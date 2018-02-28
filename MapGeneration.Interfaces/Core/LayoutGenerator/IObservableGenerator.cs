namespace MapGeneration.Interfaces.Core.LayoutGenerator
{
	using System;

	public interface IObservableGenerator<TNode> : ILayoutGenerator<TNode>
	{
		event Action<IMapLayout<TNode>> OnPerturbed;

		event Action<IMapLayout<TNode>> OnPartialValid;

		event Action<IMapLayout<TNode>> OnValid;
	}
}