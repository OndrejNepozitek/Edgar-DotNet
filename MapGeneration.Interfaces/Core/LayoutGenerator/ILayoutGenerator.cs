namespace MapGeneration.Interfaces.Core.LayoutGenerator
{
	using System.Collections.Generic;

	public interface ILayoutGenerator<TMapDescription, TNode>
	{
		IList<IMapLayout<TNode>> GetLayouts(TMapDescription mapDescription, int numberOfLayouts = 10);
	}
}