namespace MapGeneration.Grid
{
	using System;
	using System.Collections.Generic;
	using DataStructures.Graphs;
	using GeneralAlgorithms.DataStructures.Polygons;
	using LayoutGenerators;
	using Utils;

	public class LayoutGenerator<TNode> : LayoutGenerator<Layout<TNode>, GridPolygon, TNode> where TNode : IComparable<TNode>
	{
		protected readonly float ShapePerturbChance = 0.2f;
		protected ConfigurationSpaces configurationSpaces;
		protected IGraph<TNode> graph;

		/// <summary>
		/// Decide whether shape or position should be perturbed and call corresponding methods.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="chain"></param>
		/// <returns></returns>
		protected override Layout<TNode> PerturbLayout(Layout<TNode> layout, List<TNode> chain)
		{
			var node = chain.GetRandom(random);

			if (random.NextDouble() <= ShapePerturbChance)
			{
				return PerturbShape(layout, node);
			}

			return PerturbPosition(layout, node);
		}

		protected override Layout<TNode> AddChainToLayout(Layout<TNode> layout, List<TNode> chain)
		{
			throw new NotImplementedException();
		}

		protected override Layout<TNode> GetInitialLayout(List<TNode> chain)
		{
			throw new NotImplementedException();
		}

		protected override bool IsLayoutValid(Layout<TNode> layout)
		{
			// Check for intersections
			throw new NotImplementedException();
		}

		/// <summary>
		/// Choose randomly a new shape for given node.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		protected Layout<TNode> PerturbShape(Layout<TNode> layout, TNode node)
		{
			var polygons = configurationSpaces.GetPolygons();
			layout.GetConfiguration(node, out var configuration);
			GridPolygon polygon;

			do
			{
				polygon = polygons.GetRandom(random);
			}
			while (ReferenceEquals(polygon, configuration.Polygon));

			var newConfiguration = new Configuration(polygon, configuration.Position);
			var newLayout = layout.Clone();
			newLayout.SetConfiguration(node, newConfiguration);

			return newLayout;
		}

		/// <summary>
		/// Find maximum configuration spaces intersection with neigbours and choose one random point to change position to.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		protected Layout<TNode> PerturbPosition(Layout<TNode> layout, TNode node)
		{
			var configurations = new List<Configuration>();

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
			var newConfiguration = new Configuration(mainConfiguration.Polygon, position);
			var newLayout = layout.Clone();
			newLayout.SetConfiguration(node, newConfiguration);

			return newLayout;
		}
	}
}
