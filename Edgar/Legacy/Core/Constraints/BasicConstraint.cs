using System;
using System.Linq;
using Edgar.Legacy.Core.Configurations.Interfaces;
using Edgar.Legacy.Core.Configurations.Interfaces.EnergyData;
using Edgar.Legacy.Core.ConfigurationSpaces;
using Edgar.Legacy.Core.ConfigurationSpaces.Interfaces;
using Edgar.Legacy.Core.Constraints.Interfaces;
using Edgar.Legacy.Core.Layouts.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace Edgar.Legacy.Core.Constraints
{
    /// <summary>
	/// Basic constraint that checks if no polygons overlap and if neighboring
	/// nodes are connected by doors.
	/// </summary>
	/// <typeparam name="TLayout"></typeparam>
	/// <typeparam name="TNode"></typeparam>
	/// <typeparam name="TConfiguration"></typeparam>
	/// <typeparam name="TEnergyData"></typeparam>
	/// <typeparam name="TShapeContainer"></typeparam>
    public class BasicConstraint<TLayout, TNode, TConfiguration, TEnergyData, TShapeContainer> : INodeConstraint<TLayout, TNode, TConfiguration, TEnergyData>
		where TLayout : ILayout<TNode, TConfiguration>
		where TConfiguration : IEnergyConfiguration<TShapeContainer, TNode, TEnergyData>
		where TEnergyData : INodeEnergyData, new()
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

		/// <inheritdoc />
		/// <returns>True if there is no overlap and all neighbours are connected by doors.</returns>
		public bool ComputeEnergyData(TLayout layout, TNode node, TConfiguration configuration, ref TEnergyData energyData)
		{
			var overlap = 0;
			var distance = 0;
			var neighbours = layout.Graph.GetNeighbours(node).ToList();

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
				else if (neighbours.Contains(vertex))
				{
					if (!configurationSpaces.HaveValidPosition(configuration, c))
					{
						// TODO: this is not really accurate when there are more sophisticated door positions (as smaller distance is not always better)
						distance += ComputeDistance(configuration, c);
					}
				}
			}

			var energy = ComputeEnergy(overlap, distance);

			energyData.Overlap = overlap;
			energyData.MoveDistance = distance;
			energyData.Energy += energy;

			return overlap == 0 && distance == 0;
		}

		/// <inheritdoc />
		/// <remarks>
		/// Recomputes only the relation to the perturbed node.
		/// </remarks>
		/// <returns>True if there is no overlap and all neighbours are connected by doors.</returns>
		public bool UpdateEnergyData(TLayout layout, TNode perturbedNode, TConfiguration oldConfiguration,
			TConfiguration newConfiguration, TNode node, TConfiguration configuration, ref TEnergyData energyData)
		{
			var overlapOld = ComputeOverlap(configuration, oldConfiguration);
			var overlapNew = ComputeOverlap(configuration, newConfiguration);
			var overlapTotal = configuration.EnergyData.Overlap + (overlapNew - overlapOld);

			// MoveDistance should not be recomputed as it is used only when two nodes are neighbours which is not the case here
			var distanceTotal = configuration.EnergyData.MoveDistance;
			if (AreNeighbours(layout, perturbedNode, node))
			{
				// Distance is taken into account only when there is no overlap
				var distanceOld = overlapOld == 0 && !configurationSpaces.HaveValidPosition(oldConfiguration, configuration) ? ComputeDistance(configuration, oldConfiguration) : 0;
				var distanceNew = overlapNew == 0 && !configurationSpaces.HaveValidPosition(newConfiguration, configuration) ? ComputeDistance(configuration, newConfiguration) : 0;

				distanceTotal = configuration.EnergyData.MoveDistance + (distanceNew - distanceOld);
			}

			var newEnergy = ComputeEnergy(overlapTotal, distanceTotal);

			energyData.MoveDistance = distanceTotal;
			energyData.Overlap = overlapTotal;
			energyData.Energy += newEnergy;

			return overlapTotal == 0 && distanceTotal == 0;
		}

		/// <inheritdoc />
		/// <summary>
		/// Updates energy data by collecting differences from all the other nodes.
		/// </summary>
		public bool UpdateEnergyData(TLayout oldLayout, TLayout newLayout, TNode node, ref TEnergyData energyData)
		{
			oldLayout.GetConfiguration(node, out var oldConfiguration);
			var newOverlap = oldConfiguration.EnergyData.Overlap;
			var newDistance = oldConfiguration.EnergyData.MoveDistance;

			foreach (var vertex in oldLayout.Graph.Vertices)
			{
				if (vertex.Equals(node))
					continue;

				if (!oldLayout.GetConfiguration(vertex, out var nodeConfiguration))
					continue;

				newLayout.GetConfiguration(vertex, out var newNodeConfiguration);

				newOverlap += newNodeConfiguration.EnergyData.Overlap - nodeConfiguration.EnergyData.Overlap;
				newDistance += newNodeConfiguration.EnergyData.MoveDistance - nodeConfiguration.EnergyData.MoveDistance;
			}


			var newEnergy = ComputeEnergy(newOverlap, newDistance);

			energyData.MoveDistance = newDistance;
			energyData.Overlap = newOverlap;
			energyData.Energy += newEnergy;

			return newOverlap == 0 && newDistance == 0;
		}

		private int ComputeOverlap(TConfiguration configuration1, TConfiguration configuration2)
		{
			return polygonOverlap.OverlapArea(configuration1.ShapeContainer, configuration1.Position, configuration2.ShapeContainer, configuration2.Position);
		}

		private int ComputeDistance(TConfiguration configuration1, TConfiguration configuration2)
		{
			var distance = Vector2Int.ManhattanDistance(configuration1.Shape.BoundingRectangle.Center + configuration1.Position,
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