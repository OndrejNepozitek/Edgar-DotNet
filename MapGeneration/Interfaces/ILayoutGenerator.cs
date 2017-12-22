namespace MapGeneration.Interfaces
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Graphs;

	public interface ILayoutGenerator<TNode, TPolygon, TPosition>
	{
		IList<ILayout<TNode, TPolygon, TPosition, IntLine>> GetLayouts(FastGraph<int> mapDescription, int minimumLayouts = 10);
	}
}
