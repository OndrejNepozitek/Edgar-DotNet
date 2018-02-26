namespace MapGeneration.Core
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.DataStructures.Graphs;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces;

	public class Layout<TConfiguration, TEnergyData> : IEnergyLayout<int, TConfiguration>
		where TConfiguration : struct, IEnergyConfiguration<TConfiguration, IntAlias<GridPolygon>, TEnergyData>
		where TEnergyData : IEnergyData<TEnergyData>
	{
		private readonly TConfiguration?[] vertices;

		public IGraph<int> Graph { get; }

		public Layout(IGraph<int> graph)
		{
			Graph = graph;
			vertices = new TConfiguration?[Graph.VerticesCount];
		}

		private Layout(IGraph<int> graph, TConfiguration?[] vertices)
		{
			Graph = graph;
			this.vertices = (TConfiguration?[]) vertices.Clone();
		}

		public bool GetConfiguration(int node, out TConfiguration configuration)
		{
			if (vertices[node].HasValue)
			{
				configuration = vertices[node].Value;
				return true;
			}

			configuration = default(TConfiguration);
			return false;
		}

		public void SetConfiguration(int node, TConfiguration configuration)
		{
			vertices[node] = configuration;
		}

		public float GetEnergy()
		{
			return vertices.Where(x => x.HasValue).Sum(x => x.Value.EnergyData.Energy);
		}

		public Layout<TConfiguration, TEnergyData> Clone()
		{
			return new Layout<TConfiguration, TEnergyData>(Graph, vertices);
		}

		public IEnumerable<TConfiguration> GetAllConfigurations()
		{
			foreach (var configuration in vertices)
			{
				if (configuration.HasValue)
				{
					yield return configuration.Value;
				}
			}
		}

		ILayout<int, TConfiguration> ILayout<int, TConfiguration>.Clone()
		{
			return Clone();
		}

		public float Energy { get; set; } = 0; // TODO: change

		public bool IsValid { get; set; } = true; // TODO: change
	}
}