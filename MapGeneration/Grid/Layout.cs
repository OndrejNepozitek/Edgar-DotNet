namespace MapGeneration.Grid
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using DataStructures.Graphs;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces;

	public class Layout<TNode> : ILayout<GridPolygon> where TNode : IComparable<TNode>
	{
		// TODO: would it be faster to use a list/array instead ?
		private Dictionary<TNode, Configuration> nodes;
		private IGraph<TNode> graph;

		public Layout()
		{
			nodes = new Dictionary<TNode, Configuration>();
		}

		private Layout(Layout<TNode> layout)
		{
			nodes = new Dictionary<TNode, Configuration>(layout.nodes);
			graph = layout.graph;
		}

		public Layout(IGraph<TNode> graph)
		{
			this.graph = graph;
		}

		public float GetEnergy()
		{
			throw new NotImplementedException();
		}

		public float GetDifference(ILayout<GridPolygon> other)
		{
			throw new NotImplementedException();
		}

		public bool GetConfiguration(TNode node, out Configuration configuration)
		{
			throw new NotImplementedException();
		}

		public void SetConfiguration(TNode node, Configuration configuration)
		{
			// TODO: revise
			nodes[node] = configuration;
		}

		public List<Configuration> GetAllConfigurations()
		{
			return nodes.Values.ToList();
		}

		public Layout<TNode> Clone()
		{
			return new Layout<TNode>(this);
		}
	}
}