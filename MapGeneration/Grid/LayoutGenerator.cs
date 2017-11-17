namespace MapGeneration.Grid
{
	using System;
	using System.Collections.Generic;
	using DataStructures.Graphs;
	using GeneralAlgorithms.DataStructures.Polygons;
	using LayoutGenerators;
	using Utils;

	public class LayoutGenerator<TNode> : LayoutGenerator<GridLayout<TNode>, GridPolygon, TNode> where TNode : IComparable<TNode>
	{
		protected readonly float ShapePerturbChance = 0.2f;
		protected GridConfigurationSpaces configurationSpaces;
		protected IGraph<TNode> graph;

		protected override GridLayout<TNode> PerturbLayout(GridLayout<TNode> layout, List<TNode> chain)
		{
			var node = chain.GetRandom(random);

			if (random.NextDouble() <= ShapePerturbChance)
			{
				return PerturbShape(layout, node);
			}

			return PerturbPosition(layout, node);
		}

		protected override GridLayout<TNode> AddChainToLayout(GridLayout<TNode> layout, List<TNode> chain)
		{
			throw new NotImplementedException();
		}

		protected override GridLayout<TNode> GetInitialLayout(List<TNode> chain)
		{
			throw new NotImplementedException();
		}

		protected GridLayout<TNode> PerturbShape(GridLayout<TNode> layout, TNode node)
		{
			var polygons = configurationSpaces.GetPolygons();
			layout.GetConfiguration(node, out var configuration);
			GridPolygon polygon;

			do
			{
				polygon = polygons.GetRandom(random);
			}
			while (ReferenceEquals(polygon, configuration.Polygon));

			var newConfiguration = new GridConfiguration(polygon, configuration.Position);
			var newLayout = layout.Clone();
			newLayout.SetConfiguration(node, newConfiguration);

			return newLayout;
		}

		protected GridLayout<TNode> PerturbPosition(GridLayout<TNode> layout, TNode node)
		{
			var configurations = new List<GridConfiguration>();

			foreach (var node1 in graph.Neighbours(node))
			{
				if (layout.GetConfiguration(node1, out var configuration))
				{
					configurations.Add(configuration);
				}
			}

			layout.GetConfiguration(node, out var mainConfiguration);
			var intersection = configurationSpaces.GetMaximumIntersection(configurations, mainConfiguration);

			var position = intersection.GetRandom(random);
			var newConfiguration = new GridConfiguration(mainConfiguration.Polygon, position);
			var newLayout = layout.Clone();
			newLayout.SetConfiguration(node, newConfiguration);

			return newLayout;
		}
	}
}
