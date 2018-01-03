namespace MapGeneration.Core
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using Interfaces;
	using Utils;

	public class LayoutOperations<TNode, TLayout, TConfiguration, TShapeContainer> : IRandomInjectable
		where TLayout : ILayout<TNode, TConfiguration>
		where TConfiguration : IConfiguration<TConfiguration, TShapeContainer>
	{
		private readonly IConfigurationSpaces<TNode, TShapeContainer, TConfiguration> configurationSpaces;
		private readonly PolygonOverlap polygonOverlap = new PolygonOverlap();
		private Random random = new Random();

		private const float EnergySigma = 300f; // TODO: change

		public LayoutOperations(IConfigurationSpaces<TNode, TShapeContainer, TConfiguration> configurationSpaces)
		{
			this.configurationSpaces = configurationSpaces;
		}

		public TLayout PerturbShape(TLayout layout, TNode node, bool updateEnergies = true)
		{
			/*layout.GetConfiguration(node, out var configuration);

			// Return the current layout if given node cannot be shape-perturbed
			if (!configurationSpaces.CanPerturbShape(node))
				return layout;

			GridPolygon shape;
			do
			{
				shape = configurationSpaces.GetRandomShape(node);
			}
			while (ReferenceEquals(shape, configuration.Shape));

			var newConfiguration = new Grid.Configuration(configuration, polygon);

			return UpdateLayoutAfterPerturabtion(layout, node, newConfiguration);*/

			throw new NotImplementedException();
		}

		public TLayout PerturbShape(TLayout layout, IList<TNode> nodeOptions, bool updateEnergies = true)
		{
			var canBePerturbed = nodeOptions.Where(x => configurationSpaces.CanPerturbShape(x)).ToList();

			if (canBePerturbed.Count == 0)
				return layout;

			return PerturbShape(layout, canBePerturbed.GetRandom(random));
		}

		public TLayout PerturbPosition(TLayout layout, bool updateEnergies = true)
		{
			throw new NotImplementedException();
		}

		public void RecomputeValidityVectors(TLayout layout)
		{
			foreach (var vertex in layout.Graph.Vertices)
			{
				if (!layout.GetConfiguration(vertex, out var configuration))
					continue;

				// TODO: could it be faster?
				var neighbours = layout.Graph.GetNeighbours(vertex).ToList();

				var validityVector = SimpleBitVector32.StartWithOnes(neighbours.Count);

				for (var i = 0; i < neighbours.Count; i++)
				{
					var neighbour = neighbours[i];

					// Non-existent neighbour is the same thing as a valid neighbour
					if (!layout.GetConfiguration(neighbour, out var nc))
					{
						validityVector[i] = false;
						continue;
					}

					var isValid = configurationSpaces.HaveValidPosition(configuration, nc);
					validityVector[i] = !isValid;
				}

				layout.SetConfiguration(vertex, configuration.SetValidityVector(validityVector));
			}
		}

		public void RecomputeEnergy(TLayout layout)
		{
			
		}

		public float GetEnergy(TLayout layout, TNode node, Configuration configuration)
		{
			var intersection = 0;
			var distance = 0;

			foreach (var n in layout.Graph.GetNeighbours(node))
			{
				if (!layout.GetConfiguration(n, out var c))
					continue;

				var area = polygonOverlap.OverlapArea(configuration.Shape, configuration.Position, c.Shape, c.Position);

				if (area != 0)
				{
					intersection += area;
				}
				else
				{
					if (!polygonOverlap.DoTouch(configuration.Shape, configuration.Position, c.Shape, c.Position))
					{
						distance += IntVector2.ManhattanDistance(configuration.Shape.BoundingRectangle.Center + configuration.Position,
							c.Shape.BoundingRectangle.Center + c.Position);
					}
				}
			}

			return (float)(Math.Pow(Math.E, intersection / EnergySigma) * Math.Pow(Math.E, distance / EnergySigma) - 1);
		}

		public void InjectRandomGenerator(Random random)
		{
			this.random = random;
			configurationSpaces.InjectRandomGenerator(random);
		}
	}
}