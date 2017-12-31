namespace MapGeneration.Core
{
	using System;
	using GeneralAlgorithms.DataStructures.Graphs;
	using Interfaces;

	public class Layout : ILayout<int, Configuration>
	{
		private readonly Configuration?[] vertices;

		public IGraph<int> Graph { get; }

		public Layout(IGraph<int> graph)
		{
			Graph = graph;
			vertices = new Configuration?[Graph.VerticesCount];
		}

		private Layout(IGraph<int> graph, Configuration?[] vertices)
		{
			Graph = graph;
			this.vertices = (Configuration?[]) vertices.Clone();
		}

		public bool GetConfiguration(int node, out Configuration configuration)
		{
			if (vertices[node].HasValue)
			{
				configuration = vertices[node].Value;
				return true;
			}

			configuration = default(Configuration);
			return false;
		}

		public void SetConfiguration(int node, Configuration configuration)
		{
			vertices[node] = configuration;
		}

		public float GetEnergy()
		{
			// return vertices.Where(x => x.HasValue).Sum(x => x.Value.Energy);

			throw new NotImplementedException();
		}

		public Layout Clone()
		{
			return new Layout(Graph, vertices);
		}

		ILayout<int, Configuration> ILayout<int, Configuration>.Clone()
		{
			return Clone();
		}
	}
}