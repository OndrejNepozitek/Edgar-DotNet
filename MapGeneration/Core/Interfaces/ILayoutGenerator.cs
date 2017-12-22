namespace MapGeneration.Core.Interfaces
{
	using System.Collections.Generic;

	public interface ILayoutGenerator<TNode>
	{
		IList<ILayout<TNode>> GetLayouts(IMapDescription<TNode> mapDescription, int numberOfLayouts = 10);
	}
}