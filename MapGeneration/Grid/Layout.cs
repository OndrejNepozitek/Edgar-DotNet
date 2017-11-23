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
		private readonly Dictionary<TNode, Configuration> nodes;
		private readonly IGraph<TNode> graph;

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
			return float.MaxValue; // TODO: change
		}

		public bool GetConfiguration(TNode node, out Configuration configuration)
		{
			return nodes.TryGetValue(node, out configuration);
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

		public bool IsValid()
		{
			return nodes.Sum(x => x.Value.WrongNeighbours) == 0;
		}

		public Layout<TNode> Clone()
		{
			return new Layout<TNode>(this);
		}
	}
}