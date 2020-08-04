using System;
using System.Linq;
using GeneralAlgorithms.Algorithms.Polygons;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.Configurations.Interfaces;
using MapGeneration.Core.Configurations.Interfaces.EnergyData;
using MapGeneration.Core.ConfigurationSpaces;
using MapGeneration.Core.ConfigurationSpaces.Interfaces;
using MapGeneration.Core.Constraints.Interfaces;
using MapGeneration.Core.Layouts.Interfaces;

namespace Edgar.GraphBasedGenerator.Constraints.BasicConstraint
{
    public class BasicConstraint<TLayout, TNode, TConfiguration, TEnergyData, TShapeContainer> : INodeConstraint<TLayout, TNode, TConfiguration, TEnergyData>
		where TLayout : ILayout<TNode, TConfiguration>
		where TConfiguration : IEnergyConfiguration<TShapeContainer, TNode, TEnergyData>
		where TEnergyData : IBasicConstraintData, IEnergyData
	{
		private readonly IPolygonOverlap<TShapeContainer> polygonOverlap;
        private readonly float energySigma;
		private readonly IConfigurationSpaces<TNode, TShapeContainer, TConfiguration, ConfigurationSpace> configurationSpaces;

		public BasicConstraint(IPolygonOverlap<TShapeContainer> polygonOverlap, float averageSize, IConfigurationSpaces<TNode, TShapeContainer, TConfiguration, ConfigurationSpace> configurationSpaces)
		{
			this.polygonOverlap = polygonOverlap;
			energySigma = 10 * averageSize;
			this.configurationSpaces = configurationSpaces;
		}

		public bool ComputeEnergyData(TLayout layout, TNode node, TConfiguration configuration, ref TEnergyData energyData)
		{
            var overlap = 0;
			var distance = 0;
			var neighbors = layout.Graph.GetNeighbours(node).ToList();

			foreach (var vertex in layout.Graph.Vertices)
			{
				if (vertex.Equals(node))
					continue;

				if (!layout.GetConfiguration(vertex, out var c))
					continue;

				var area = ComputeOverlap(configuration, c);

				if (area != 0)
				{
					overlap += area;
				}
				else if (neighbors.Contains(vertex))
				{
					if (!configurationSpaces.HaveValidPosition(configuration, c))
					{
						// TODO: this is not really accurate when there are more sophisticated door positions (as smaller distance is not always better)
						distance += ComputeDistance(configuration, c);
					}
				}
			}

			var energy = ComputeEnergy(overlap, distance);

            var constraintData = new BasicConstraintData();
            constraintData.Overlap = overlap;
            constraintData.MoveDistance = distance;
            energyData.BasicConstraintData = constraintData;
            energyData.Energy += energy;

			return overlap == 0 && distance == 0;
		}

		public bool UpdateEnergyData(TLayout layout, TNode perturbedNode, TConfiguration oldConfiguration,
			TConfiguration newConfiguration, TNode node, TConfiguration configuration, ref TEnergyData energyData)
		{
            var overlapOld = ComputeOverlap(configuration, oldConfiguration);
			var overlapNew = ComputeOverlap(configuration, newConfiguration);
			var overlapTotal = configuration.EnergyData.BasicConstraintData.Overlap + (overlapNew - overlapOld);

			// MoveDistance should not be recomputed as it is used only when two nodes are neighbours which is not the case here
			var distanceTotal = configuration.EnergyData.BasicConstraintData.MoveDistance;
			if (AreNeighbours(layout, perturbedNode, node))
			{
				// Distance is taken into account only when there is no overlap
				var distanceOld = overlapOld == 0 && !configurationSpaces.HaveValidPosition(oldConfiguration, configuration) ? ComputeDistance(configuration, oldConfiguration) : 0;
				var distanceNew = overlapNew == 0 && !configurationSpaces.HaveValidPosition(newConfiguration, configuration) ? ComputeDistance(configuration, newConfiguration) : 0;

				distanceTotal = configuration.EnergyData.BasicConstraintData.MoveDistance + (distanceNew - distanceOld);
			}

			var newEnergy = ComputeEnergy(overlapTotal, distanceTotal);

            var constraintData = energyData.BasicConstraintData;
            constraintData.MoveDistance = distanceTotal;
            constraintData.Overlap = overlapTotal;
            energyData.BasicConstraintData = constraintData;
			energyData.Energy += newEnergy;

			return overlapTotal == 0 && distanceTotal == 0;
		}

		public bool UpdateEnergyData(TLayout oldLayout, TLayout newLayout, TNode node, ref TEnergyData energyData)
		{
			oldLayout.GetConfiguration(node, out var oldConfiguration);
			var newOverlap = oldConfiguration.EnergyData.BasicConstraintData.Overlap;
			var newDistance = oldConfiguration.EnergyData.BasicConstraintData.MoveDistance;

			foreach (var vertex in oldLayout.Graph.Vertices)
			{
				if (vertex.Equals(node))
					continue;

				if (!oldLayout.GetConfiguration(vertex, out var nodeConfiguration))
					continue;

				newLayout.GetConfiguration(vertex, out var newNodeConfiguration);

				newOverlap += newNodeConfiguration.EnergyData.BasicConstraintData.Overlap - nodeConfiguration.EnergyData.BasicConstraintData.Overlap;
				newDistance += newNodeConfiguration.EnergyData.BasicConstraintData.MoveDistance - nodeConfiguration.EnergyData.BasicConstraintData.MoveDistance;
			}


			var newEnergy = ComputeEnergy(newOverlap, newDistance);

            var constraintData = energyData.BasicConstraintData;
            constraintData.MoveDistance = newDistance;
            constraintData.Overlap = newOverlap;
            energyData.BasicConstraintData = constraintData;
			energyData.Energy += newEnergy;

			return newOverlap == 0 && newDistance == 0;
		}

		private int ComputeOverlap(TConfiguration configuration1, TConfiguration configuration2)
		{
			return polygonOverlap.OverlapArea(configuration1.ShapeContainer, configuration1.Position, configuration2.ShapeContainer, configuration2.Position);
		}

		private int ComputeDistance(TConfiguration configuration1, TConfiguration configuration2)
		{
			var distance = IntVector2.ManhattanDistance(configuration1.Shape.BoundingRectangle.Center + configuration1.Position,
				configuration2.Shape.BoundingRectangle.Center + configuration2.Position);

			if (distance < 0)
			{
				throw new InvalidOperationException();
			}

			return distance;
		}

		private float ComputeEnergy(int overlap, float distance)
		{
			return (float)(Math.Pow(Math.E, overlap / (energySigma * 625)) * Math.Pow(Math.E, distance / (energySigma * 50)) - 1);
		}

		private bool AreNeighbours(TLayout layout, TNode node1, TNode node2)
		{
			return layout.Graph.HasEdge(node1, node2);
		}
	}
}