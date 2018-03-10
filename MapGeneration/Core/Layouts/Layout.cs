namespace MapGeneration.Core.Layouts
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Graphs;
	using Interfaces.Core;
	using Interfaces.Core.Layouts;

	public class Layout<TConfiguration, TLayoutEnergyData> : IEnergyLayout<int, TConfiguration, TLayoutEnergyData>, ISmartCloneable<Layout<TConfiguration, TLayoutEnergyData>>
		where TConfiguration : ISmartCloneable<TConfiguration>
		where TLayoutEnergyData : ISmartCloneable<TLayoutEnergyData>
	{
		private readonly TConfiguration[] vertices;
		private readonly bool[] hasValue;

		public TLayoutEnergyData EnergyData { get; set; }

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

		public Layout<TConfiguration, TLayoutEnergyData> SmartClone()
		{
			var layout = new Layout<TConfiguration, TLayoutEnergyData>(Graph);

			for (var i = 0; i < vertices.Length; i++)
			{
				var configuration = vertices[i];

				if (hasValue[i])
				{
					layout.vertices[i] = configuration.SmartClone();
					layout.hasValue[i] = true;
				}
			}

			layout.EnergyData = EnergyData.SmartClone();

			return layout;
		}
	}
}