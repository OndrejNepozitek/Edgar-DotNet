using System;
using System.Linq;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Graphs;
using MapGeneration.Core.Configurations.Interfaces;
using MapGeneration.Core.Configurations.Interfaces.EnergyData;
using MapGeneration.Core.ConfigurationSpaces;
using MapGeneration.Core.ConfigurationSpaces.Interfaces;
using MapGeneration.Core.Constraints.Interfaces;
using MapGeneration.Core.Layouts.Interfaces;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.MapDescriptions.Interfaces;

namespace MapGeneration.Core.Constraints
{
    public class CorridorConstraints
    <TLayout, TNode, TConfiguration, TEnergyData, TShapeContainer> : INodeConstraint<TLayout, TNode, TConfiguration, TEnergyData>
		where TLayout : ILayout<TNode, TConfiguration>
		where TConfiguration : IEnergyConfiguration<TShapeContainer, TNode, TEnergyData>
		where TEnergyData : ICorridorsData, new()
	{
		private readonly IMapDescription<TNode> mapDescription;
		private readonly float energySigma;
		private readonly IConfigurationSpaces<TNode, TShapeContainer, TConfiguration, ConfigurationSpace> configurationSpaces;
		private readonly IGraph<TNode> stageOneGraph;
        private readonly IGraph<TNode> graph;

		public CorridorConstraints(IMapDescription<TNode> mapDescription, float averageSize, IConfigurationSpaces<TNode, TShapeContainer, TConfiguration, ConfigurationSpace> configurationSpaces)
		{
			this.mapDescription = mapDescription;
			this.energySigma = 10 * averageSize; // TODO: should it be like this?
			this.configurationSpaces = configurationSpaces;
			stageOneGraph = mapDescription.GetStageOneGraph();
            graph = mapDescription.GetGraph();
        }

		/// <inheritdoc />
		public bool ComputeEnergyData(TLayout layout, TNode node, TConfiguration configuration, ref TEnergyData energyData)
		{
			if (mapDescription.GetRoomDescription(node).GetType() == typeof(CorridorRoomDescription))
				return true;

			var distance = 0;
			var neighbours = stageOneGraph.GetNeighbours(node).ToList();

			foreach (var vertex in neighbours)
			{
				if (vertex.Equals(node))
					continue;

				if (!layout.GetConfiguration(vertex, out var c))
					continue;

				// TODO: why wasn't this here?
				if (!AreNeighboursWithoutCorridors(vertex, node))
					continue;

				if (!configurationSpaces.HaveValidPosition(configuration, c))
				{
					distance += ComputeDistance(configuration, c);
				}
			}

			var energy = ComputeEnergy(0, distance);

			energyData.CorridorDistance = distance;
			energyData.Energy += energy;

			return distance == 0;
		}

		/// <inheritdoc />
		public bool UpdateEnergyData(TLayout layout, TNode perturbedNode, TConfiguration oldConfiguration,
			TConfiguration newConfiguration, TNode node, TConfiguration configuration, ref TEnergyData energyData)
		{
            if (mapDescription.GetRoomDescription(node).GetType() == typeof(CorridorRoomDescription))
				return true;

			// MoveDistance should not be recomputed as it is used only when two nodes are neighbours which is not the case here
			var distanceTotal = configuration.EnergyData.CorridorDistance;
			if (AreNeighboursWithoutCorridors(perturbedNode, node))
			{
				// Distance is taken into account only when there is no overlap
				var distanceOld = !configurationSpaces.HaveValidPosition(oldConfiguration, configuration) ? ComputeDistance(configuration, oldConfiguration) : 0;
				var distanceNew = !configurationSpaces.HaveValidPosition(newConfiguration, configuration) ? ComputeDistance(configuration, newConfiguration) : 0;

				distanceTotal = configuration.EnergyData.CorridorDistance + (distanceNew - distanceOld);
			}

			var newEnergy = ComputeEnergy(0, distanceTotal);

			energyData.CorridorDistance = distanceTotal;
			energyData.Energy += newEnergy;

			return distanceTotal == 0;
		}

		/// <inheritdoc />
		public bool UpdateEnergyData(TLayout oldLayout, TLayout newLayout, TNode node, ref TEnergyData energyData)
		{
            if (mapDescription.GetRoomDescription(node).GetType() == typeof(CorridorRoomDescription))
				return true;

			oldLayout.GetConfiguration(node, out var oldConfiguration);
			var newDistance = oldConfiguration.EnergyData.CorridorDistance;

			foreach (var vertex in oldLayout.Graph.Vertices)
			{
				if (vertex.Equals(node))
					continue;

				if (!oldLayout.GetConfiguration(vertex, out var nodeConfiguration))
					continue;

                newLayout.GetConfiguration(vertex, out var newNodeConfiguration);
				newDistance += newNodeConfiguration.EnergyData.CorridorDistance - nodeConfiguration.EnergyData.CorridorDistance;
			}

			var newEnergy = ComputeEnergy(0, newDistance);

			energyData.CorridorDistance = newDistance;
			energyData.Energy += newEnergy;

			return newDistance == 0;
		}

		// TODO: keep dry
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

		private bool AreNeighboursWithoutCorridors(TNode node1, TNode node2)
		{
			return stageOneGraph.HasEdge(node1, node2) && !graph.HasEdge(node1, node2);
		}
	}
}