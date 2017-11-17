namespace MapGeneration.Grid
{
	using System;
	using System.Collections.Generic;
	using ConfigurationSpaces;
	using DataStructures.Graphs;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Layouts;

	public class GridLayout<TNode> : ILayout<GridPolygon> where TNode : IComparable<TNode>
	{
		// TODO: would it be faster to use a list/array instead ?
		private Dictionary<TNode, GridConfiguration> nodes;
		private IGraph<TNode> graph;

		public GridLayout()
		{
			nodes = new Dictionary<TNode, GridConfiguration>();
		}

		private GridLayout(GridLayout<TNode> layout)
		{
			this.nodes = new Dictionary<TNode, GridConfiguration>(layout.nodes);
			graph = layout.graph;
		}

		public GridLayout(IGraph<TNode> graph)
		{
			this.graph = graph;
		}

		public bool IsValid()
		{
			throw new System.NotImplementedException();
		}

		public float GetEnergy()
		{
			throw new System.NotImplementedException();
		}

		public float GetDifference(ILayout<GridPolygon> other)
		{
			throw new System.NotImplementedException();
		}

		public bool GetConfiguration(TNode node, out GridConfiguration configuration)
		{
			throw new System.NotImplementedException();
		}

		public void SetConfiguration(TNode node, GridConfiguration configuration)
		{
			throw new System.NotImplementedException();
		}

		public GridLayout<TNode> Clone()
		{
			return new GridLayout<TNode>(this);
		}
	}
}