namespace MapGeneration.Interfaces.Core.LayoutGenerator
{
	using System.Collections.Generic;
	using MapDescription;

	public interface ILayoutGenerator<TNode>
	{
		IList<IMapLayout<TNode>> GetLayouts(IMapDescription<TNode> mapDescription, int numberOfLayouts = 10);
	}
}