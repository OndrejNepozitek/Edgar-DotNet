namespace MapGeneration.Interfaces
{
	using System;
	using System.Collections.Generic;
	using DataStructures.Graphs;

	public interface ILayoutGenerator<TLayout, TPolygon, TNode> where TNode : IComparable<TNode> where TLayout : ILayout<TPolygon>
	{
		IList<TLayout> GetLayouts(IGraph<TNode> graph, Action<TLayout> action, int minimumLayouts = 10);
	}
}
