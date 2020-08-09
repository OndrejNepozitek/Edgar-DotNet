using System;
using System.Linq;
using Edgar.GraphBasedGenerator.Configurations;
using Edgar.GraphBasedGenerator.ConfigurationSpaces;
using Edgar.GraphBasedGenerator.RoomShapeGeometry;
using GeneralAlgorithms.DataStructures.Graphs;
using MapGeneration.Core.Configurations.Interfaces.EnergyData;
using MapGeneration.Core.Constraints.Interfaces;
using MapGeneration.Core.Layouts.Interfaces;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.MapDescriptions.Interfaces;

namespace Edgar.GraphBasedGenerator.Constraints.CorridorConstraint
{
    public class CorridorConstraint<TNode, TConfiguration, TEnergyData> : INodeConstraint<ILayout<TNode, TConfiguration>, TNode, TConfiguration, TEnergyData>
        where TConfiguration : ISimpleEnergyConfiguration<TEnergyData>
		where TEnergyData : ICorridorConstraintData
	{
		private readonly ILevelDescription<TNode> mapDescription;
        private readonly IConfigurationSpaces<TConfiguration> configurationSpaces;
		private readonly IGraph<TNode> graphWithoutCorridors;
        private readonly IGraph<TNode> graph;
        private readonly IRoomShapeGeometry<TConfiguration> roomShapeGeometry;

		public CorridorConstraint(ILevelDescription<TNode> mapDescription, IConfigurationSpaces<TConfiguration> configurationSpaces, IRoomShapeGeometry<TConfiguration> roomShapeGeometry)
		{
			this.mapDescription = mapDescription;
            this.configurationSpaces = configurationSpaces;
            this.roomShapeGeometry = roomShapeGeometry;
            graphWithoutCorridors = mapDescription.GetGraphWithoutCorridors();
            graph = mapDescription.GetGraph();
        }

		/// <inheritdoc />
		public bool ComputeEnergyData(ILayout<TNode, TConfiguration> layout, TNode node, TConfiguration configuration, ref TEnergyData energyData)
		{
			if (mapDescription.GetRoomDescription(node).IsCorridor)
				return true;

			var distance = 0;
			var neighbours = graphWithoutCorridors.GetNeighbours(node).ToList();

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

            var constraintData = new CorridorConstraintData {CorridorDistance = distance};
            energyData.CorridorConstraintData = constraintData;

            return distance == 0;
		}

		/// <inheritdoc />
		public bool UpdateEnergyData(ILayout<TNode, TConfiguration> layout, TNode perturbedNode, TConfiguration oldConfiguration,
			TConfiguration newConfiguration, TNode node, TConfiguration configuration, ref TEnergyData energyData)
		{
            if (mapDescription.GetRoomDescription(node).IsCorridor)
				return true;

			// MoveDistance should not be recomputed as it is used only when two nodes are neighbours which is not the case here
			var distanceTotal = configuration.EnergyData.CorridorConstraintData.CorridorDistance;
			if (AreNeighboursWithoutCorridors(perturbedNode, node))
			{
				// Distance is taken into account only when there is no overlap
				var distanceOld = !configurationSpaces.HaveValidPosition(oldConfiguration, configuration) ? ComputeDistance(configuration, oldConfiguration) : 0;
				var distanceNew = !configurationSpaces.HaveValidPosition(newConfiguration, configuration) ? ComputeDistance(configuration, newConfiguration) : 0;

				distanceTotal = configuration.EnergyData.CorridorConstraintData.CorridorDistance + (distanceNew - distanceOld);
			}

            var constraintData = new CorridorConstraintData {CorridorDistance = distanceTotal};
			energyData.CorridorConstraintData = constraintData;

            return distanceTotal == 0;
		}

		/// <inheritdoc />
		public bool UpdateEnergyData(ILayout<TNode, TConfiguration> oldLayout, ILayout<TNode, TConfiguration> newLayout, TNode node, ref TEnergyData energyData)
		{
            if (mapDescription.GetRoomDescription(node).IsCorridor)
				return true;

			oldLayout.GetConfiguration(node, out var oldConfiguration);
			var newDistance = oldConfiguration.EnergyData.CorridorConstraintData.CorridorDistance;

			foreach (var vertex in oldLayout.Graph.Vertices)
			{
				if (vertex.Equals(node))
					continue;

				if (!oldLayout.GetConfiguration(vertex, out var nodeConfiguration))
					continue;

                newLayout.GetConfiguration(vertex, out var newNodeConfiguration);
				newDistance += newNodeConfiguration.EnergyData.CorridorConstraintData.CorridorDistance - nodeConfiguration.EnergyData.CorridorConstraintData.CorridorDistance;
			}

            var constraintData = new CorridorConstraintData {CorridorDistance = newDistance};
            energyData.CorridorConstraintData = constraintData;

            return newDistance == 0;
		}

		// TODO: keep dry
		private int ComputeDistance(TConfiguration configuration1, TConfiguration configuration2)
        {
            var distance = roomShapeGeometry.GetCenterDistance(configuration1, configuration2);

			if (distance < 0)
			{
				throw new InvalidOperationException();
			}

			return distance;
		}

        private bool AreNeighboursWithoutCorridors(TNode node1, TNode node2)
		{
			return graphWithoutCorridors.HasEdge(node1, node2) && !graph.HasEdge(node1, node2);
		}
	}
}