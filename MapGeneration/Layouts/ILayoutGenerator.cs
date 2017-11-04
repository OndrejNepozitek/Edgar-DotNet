namespace MapGeneration.Layouts
{
	using System;
	using System.Collections.Generic;
	using DataStructures.Graphs;

	public interface ILayoutGenerator<TPolygon, TNode> where TNode : IComparable<TNode>
	{
		IList<ILayout<TPolygon>> GetLayouts(IGraph<TNode> graph, int minimumLayouts = 10);
	}
}
