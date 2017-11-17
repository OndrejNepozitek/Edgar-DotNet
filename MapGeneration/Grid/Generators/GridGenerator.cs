namespace MapGeneration.ConfigurationSpaces.Generators
{
	using System;
	using System.Collections.Generic;
	using DataStructures.Graphs;
	using GeneralAlgorithms.DataStructures.Polygons;

	public class GridGenerator
	{
		public IDictionary<Tuple<GridPolygon, GridPolygon>, ConfigurationSpace> Generate<TNode>(IGraph<TNode> graph)
			where TNode : IComparable<TNode>
		{
			
		}
	}
}
