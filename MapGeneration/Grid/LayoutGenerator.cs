namespace MapGeneration.Grid
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Common;
	using DataStructures.Graphs;
	using GeneralAlgorithms.Algorithms.Graphs.GraphDecomposition;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Utils;

	public class LayoutGenerator<TNode> : LayoutGenerator<Layout<TNode>, GridPolygon, TNode> where TNode : IComparable<TNode>
	{
		protected readonly float ShapePerturbChance = 0.2f;
		protected ConfigurationSpaces ConfigurationSpaces;
		protected IGraphDecomposer<TNode> GraphDecomposer = new DummyGraphDecomposer<TNode>();
		protected GridPolygonOverlap PolygonOverlap = new GridPolygonOverlap();
		private const float sigma = 20f; // TODO: Change

		/// <summary>
		/// Decide whether shape or position should be perturbed and call corresponding methods.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="chain"></param>
		/// <param name="energyDelta"></param>
		/// <returns></returns>
		protected override Layout<TNode> PerturbLayout(Layout<TNode> layout, List<TNode> chain, out float energyDelta)
		{
			var node = chain.GetRandom(Random);
			var energy = GetEnergy(layout, node);
			var newLayout = Random.NextDouble() <= ShapePerturbChance ? PerturbShape(layout, node) : PerturbPosition(layout, node);
			energyDelta = GetEnergy(newLayout, node) - energy;

			return newLayout;
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
			// return layout.IsValid();

			// TODO: this currently does not work as it does not check if all neighbouring vertices touch
			// TODO: not all nodes should be checked - it should be enough to check just the perturbed node with respect to all other nodes
			var nodes = Graph.Vertices.ToList();

			foreach (var node1 in nodes)
			{
				foreach (var node2 in nodes)
				{
					if (node1.Equals(node2)) continue;

					if (!layout.GetConfiguration(node1, out var c1) || !layout.GetConfiguration(node2, out var c2)) continue;

					// TODO: slow, should be one function
					if (PolygonOverlap.DoOverlap(c1.Polygon, c1.Position, c2.Polygon, c2.Position) || !PolygonOverlap.DoTouch(c1.Polygon, c1.Position, c2.Polygon, c2.Position))
					{
						return false;
					}
				}
			}

			return true;
		}

		protected override List<List<TNode>> GetChains(IGraph<TNode> graph)
		{
			return GraphDecomposer.GetChains(graph);
		}

		/// <summary>
		/// Choose randomly a new shape for given node.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		protected Layout<TNode> PerturbShape(Layout<TNode> layout, TNode node)
		{
			var polygons = ConfigurationSpaces.GetPolygons();
			layout.GetConfiguration(node, out var configuration);
			GridPolygon polygon;

			do
			{
				polygon = polygons.GetRandom(Random);
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

			foreach (var node1 in Graph.Neighbours(node))
			{
				if (layout.GetConfiguration(node1, out var configuration))
				{
					configurations.Add(configuration);
				}
			}

			layout.GetConfiguration(node, out var mainConfiguration);
			var intersection = ConfigurationSpaces.GetMaximumIntersection(configurations, mainConfiguration);

			var position = intersection.GetRandom(Random);
			var newConfiguration = new Configuration(mainConfiguration.Polygon, position);
			var newLayout = layout.Clone();
			newLayout.SetConfiguration(node, newConfiguration);

			return newLayout;
		}

		protected float GetEnergy(Layout<TNode> layout, TNode node)
		{
			layout.GetConfiguration(node, out var configuration);
			return GetEnergy(layout, node, configuration);
		}

		protected float GetEnergy(Layout<TNode> layout, TNode node, Configuration configuration)
		{
			var intersection = 0;
			var distance = 0;

			foreach (var n in Graph.Neighbours(node))
			{
				if (!layout.GetConfiguration(n, out var c))
					continue;

				var area = PolygonOverlap.OverlapArea(configuration.Polygon, configuration.Position, c.Polygon, c.Position);

				if (area != 0)
				{
					intersection += area;
				}
				else
				{
					if (!PolygonOverlap.DoTouch(configuration.Polygon, configuration.Position, c.Polygon, c.Position))
					{
						distance += IntVector2.ManhattanDistance(configuration.Polygon.BoundingRectangle.Center + configuration.Position,
							c.Polygon.BoundingRectangle.Center + c.Position);
					}
				}
			}

			return (float)(Math.Pow(Math.E, intersection / sigma) * Math.Pow(Math.E, distance / sigma) - 1);
		}
	}
}
