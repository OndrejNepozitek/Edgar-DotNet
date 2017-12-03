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
	using Interfaces;
	using Utils;

	public class LayoutGenerator<TNode> : AbstractLayoutGenerator<Layout<TNode>, GridPolygon, TNode> where TNode : IComparable<TNode>
	{
		protected readonly float ShapePerturbChance = 0.4f;
		protected IConfigurationSpaces<GridPolygon, Configuration, IntVector2> ConfigurationSpaces;
		protected IGraphDecomposer<TNode> GraphDecomposer = new DummyGraphDecomposer<TNode>();
		protected GridPolygonOverlap PolygonOverlap = new GridPolygonOverlap();
		private const float sigma = 300f; // TODO: Change

		public LayoutGenerator(IConfigurationSpaces<GridPolygon, Configuration, IntVector2> configurationSpaces)
		{
			ConfigurationSpaces = configurationSpaces;
			ConfigurationSpaces.InjectRandomGenerator(Random);
		}

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
			var newEnergy = GetEnergy(newLayout, node);
			energyDelta = newEnergy - energy;

			return newLayout;
		}

		protected override Layout<TNode> AddChainToLayout(Layout<TNode> layout, List<TNode> chain)
		{
			var newLayout = layout.Clone();

			foreach (var node in chain)
			{
				AddNode(newLayout, node);
			}

			return newLayout;
		}

		protected override Layout<TNode> GetInitialLayout(List<TNode> chain)
		{
			var layout = new Layout<TNode>();
			return layout;

			return AddChainToLayout(layout, chain);
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
					if (PolygonOverlap.DoOverlap(c1.Polygon, c1.Position, c2.Polygon, c2.Position)/* || !PolygonOverlap.DoTouch(c1.Polygon, c1.Position, c2.Polygon, c2.Position)*/)
					{
						return false;
					}

					// TODO: slow
					if (Graph.HasEdge(node1, node2) && !PolygonOverlap.DoTouch(c1.Polygon, c1.Position, c2.Polygon, c2.Position))
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
			layout.GetConfiguration(node, out var configuration);
			GridPolygon polygon;

			do
			{
				polygon = ConfigurationSpaces.GetRandomShape();
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

			var newPosition = ConfigurationSpaces.GetRandomIntersection(configurations, mainConfiguration);
			var newConfiguration = new Configuration(mainConfiguration.Polygon, newPosition);
			var newLayout = layout.Clone();
			newLayout.SetConfiguration(node, newConfiguration);

			return newLayout;
		}

		protected void AddNode(Layout<TNode> layout, TNode node)
		{
			var configurations = new List<Configuration>();

			foreach (var node1 in Graph.Neighbours(node))
			{
				if (layout.GetConfiguration(node1, out var configuration))
				{
					configurations.Add(configuration);
				}
			}

			if (configurations.Count == 0)
			{
				layout.SetConfiguration(node, new Configuration(ConfigurationSpaces.GetRandomShape(), new IntVector2()));
				return;
			}

			var bestEnergy = float.MaxValue;
			GridPolygon bestShape = null;
			var bestPosition = new IntVector2();
		
			foreach (var shape in ConfigurationSpaces.GetAllShapes())
			{
				var intersection = ConfigurationSpaces.GetMaximumIntersection(configurations, new Configuration(shape, new IntVector2()));

				foreach (var position in intersection)
				{
					var energy = GetEnergy(layout, node, new Configuration(shape, position));

					if (energy < bestEnergy)
					{
						bestEnergy = energy;
						bestShape = shape;
						bestPosition = position;
					}
				}
			}

			var newConfiguration = new Configuration(bestShape, bestPosition);
			layout.SetConfiguration(node, newConfiguration);
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
