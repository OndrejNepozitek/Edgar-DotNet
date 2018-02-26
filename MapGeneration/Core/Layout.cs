namespace MapGeneration.Core
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Graphs;
	using Interfaces.Core;

	public class Layout<TConfiguration> : IEnergyLayout<int, TConfiguration>, ISmartCloneable<Layout<TConfiguration>>
		where TConfiguration : ISmartCloneable<TConfiguration>
	{
		private readonly TConfiguration[] vertices;
		private readonly bool[] hasValue;

		public float Energy { get; set; } = 0; // TODO: change

		public bool IsValid { get; set; } = true; // TODO: change

		public IGraph<int> Graph { get; }

		public Layout(IGraph<int> graph)
		{
			Graph = graph;
			vertices = new TConfiguration[Graph.VerticesCount];
			hasValue = new bool[Graph.VerticesCount];
		}

		public bool GetConfiguration(int node, out TConfiguration configuration)
		{
			if (hasValue[node])
			{
				configuration = vertices[node];
				return true;
			}

			configuration = default(TConfiguration);
			return false;
		}

		public void SetConfiguration(int node, TConfiguration configuration)
		{
			vertices[node] = configuration;
			hasValue[node] = true;
		}

		public IEnumerable<TConfiguration> GetAllConfigurations()
		{
			for (var i = 0; i < vertices.Length; i++)
			{
				if (hasValue[i])
				{
					yield return vertices[i];
				}
			}
		}

		public Layout<TConfiguration> SmartClone()
		{
			var layout = new Layout<TConfiguration>(Graph);

			for (var i = 0; i < vertices.Length; i++)
			{
				var configuration = vertices[i];

				if (hasValue[i])
				{
					layout.vertices[i] = configuration.SmartClone();
					layout.hasValue[i] = true;
				}
			}

			return layout;
		}
	}
}