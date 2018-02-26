namespace MapGeneration.Core.Constraints
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using ConfigurationSpaces;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using Interfaces.Core;
	using Interfaces.Core.Configuration;
	using Interfaces.Core.Configuration.EnergyData;
	using Interfaces.Core.ConfigurationSpaces;

	public class BasicContraint<TLayout, TNode, TConfiguration, TEnergyData, TShapeContainer> : IConstraint<TLayout, TNode, TConfiguration, TEnergyData>
		where TLayout : ILayout<TNode, TConfiguration>
		where TConfiguration : IEnergyConfiguration<TShapeContainer, TEnergyData>
		where TEnergyData : IEnergyData, new()
	{
		private readonly IPolygonOverlap polygonOverlap;
		private readonly float energySigma;
		private readonly IConfigurationSpaces<TNode, TShapeContainer, TConfiguration, ConfigurationSpace> configurationSpaces;

		public BasicContraint(IPolygonOverlap polygonOverlap, float energySigma, IConfigurationSpaces<TNode, TShapeContainer, TConfiguration, ConfigurationSpace> configurationSpaces)
		{
			this.polygonOverlap = polygonOverlap;
			this.energySigma = energySigma;
			this.configurationSpaces = configurationSpaces;
		}

		public void ComputeEnergyData(TLayout layout, TNode node, TConfiguration configuration, ref TEnergyData energyData)
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

			if (overlap != 0 || distance != 0)
			{
				energyData.IsValid = false;
			}
		}

		public void UpdateEnergyData(TLayout layout, TNode perturbedNode, TConfiguration oldConfiguration, TConfiguration newConfiguration, TNode node, TConfiguration configuration, ref TEnergyData energyData)
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

			if (overlapTotal != 0 || distanceTotal != 0)
			{
				energyData.IsValid = false;
			}
		}

		public void UpdateEnergyData(TLayout oldLayout, TLayout newLayout, TNode node, ref TEnergyData energyData)
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

			if (newOverlap != 0 || newDistance != 0)
			{
				energyData.IsValid = false;
			}
		}

		private int ComputeOverlap(TConfiguration configuration1, TConfiguration configuration2)
		{
			return polygonOverlap.OverlapArea(configuration1.Shape, configuration1.Position, configuration2.Shape, configuration2.Position);
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