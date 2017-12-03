namespace MapGeneration.Interfaces
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Graphs;

	public interface ILayoutGenerator<TNode, TPolygon, TPosition>
	{
		IList<ILayout<TNode, TPolygon, TPosition>> GetLayouts(Graph<int> mapDescription, int minimumLayouts = 10);
	}
}
