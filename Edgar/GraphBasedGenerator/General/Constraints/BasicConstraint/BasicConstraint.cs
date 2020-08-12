using System;
using System.Linq;
using Edgar.GraphBasedGenerator.General.Configurations;
using Edgar.GraphBasedGenerator.General.ConfigurationSpaces;
using Edgar.GraphBasedGenerator.General.RoomShapeGeometry;
using Edgar.Legacy.Core.Constraints.Interfaces;
using Edgar.Legacy.Core.Layouts.Interfaces;

namespace Edgar.GraphBasedGenerator.General.Constraints.BasicConstraint
{
    public class BasicConstraint<TNode, TConfiguration, TEnergyData> : INodeConstraint<ILayout<TNode, TConfiguration>, TNode, TConfiguration, TEnergyData>
        where TConfiguration : IEnergyConfiguration<TEnergyData>
		where TEnergyData : IBasicConstraintData
    {
        private readonly IRoomShapeGeometry<TConfiguration> roomShapeGeometry;
        private readonly IConfigurationSpaces<TConfiguration> configurationSpaces;
        private readonly ILevelDescription<TNode> mapDescription;
        private readonly bool optimizeCorridors;

		public BasicConstraint(IRoomShapeGeometry<TConfiguration> roomShapeGeometry, IConfigurationSpaces<TConfiguration> configurationSpaces, ILevelDescription<TNode> mapDescription, bool optimizeCorridors)
		{
            this.configurationSpaces = configurationSpaces;
            this.mapDescription = mapDescription;
            this.optimizeCorridors = optimizeCorridors;
            this.roomShapeGeometry = roomShapeGeometry;
        }

		public bool ComputeEnergyData(ILayout<TNode, TConfiguration> layout, TNode node, TConfiguration configuration, ref TEnergyData energyData)
        {
            var isCorridor = optimizeCorridors && mapDescription.GetRoomDescription(node).IsCorridor;

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
				else if (!isCorridor && neighbors.Contains(vertex))
				{
					if (!configurationSpaces.HaveValidPosition(configuration, c))
					{
						// TODO: this is not really accurate when there are more sophisticated door positions (as smaller distance is not always better)
						distance += ComputeDistance(configuration, c);
					}
				}
			}

            var constraintData = new BasicConstraintData {Overlap = overlap, MoveDistance = distance};
            energyData.BasicConstraintData = constraintData;

            return overlap == 0 && distance == 0;
		}

		public bool UpdateEnergyData(ILayout<TNode, TConfiguration> layout, TNode perturbedNode, TConfiguration oldConfiguration,
			TConfiguration newConfiguration, TNode node, TConfiguration configuration, ref TEnergyData energyData)
		{
            var isCorridor = optimizeCorridors && (mapDescription.GetRoomDescription(node).IsCorridor || mapDescription.GetRoomDescription(perturbedNode).IsCorridor);

            var overlapOld = ComputeOverlap(configuration, oldConfiguration);
			var overlapNew = ComputeOverlap(configuration, newConfiguration);
			var overlapTotal = configuration.EnergyData.BasicConstraintData.Overlap + (overlapNew - overlapOld);

			// MoveDistance should not be recomputed as it is used only when two nodes are neighbours which is not the case here
			var distanceTotal = configuration.EnergyData.BasicConstraintData.MoveDistance;
			if (!isCorridor && AreNeighbours(layout, perturbedNode, node))
			{
				// Distance is taken into account only when there is no overlap
				var distanceOld = overlapOld == 0 && !configurationSpaces.HaveValidPosition(oldConfiguration, configuration) ? ComputeDistance(configuration, oldConfiguration) : 0;
				var distanceNew = overlapNew == 0 && !configurationSpaces.HaveValidPosition(newConfiguration, configuration) ? ComputeDistance(configuration, newConfiguration) : 0;

				distanceTotal = configuration.EnergyData.BasicConstraintData.MoveDistance + (distanceNew - distanceOld);
			}

            var constraintData = energyData.BasicConstraintData;
            constraintData.MoveDistance = distanceTotal;
            constraintData.Overlap = overlapTotal;
            energyData.BasicConstraintData = constraintData;

            return overlapTotal == 0 && distanceTotal == 0;
		}

		public bool UpdateEnergyData(ILayout<TNode, TConfiguration> oldLayout, ILayout<TNode, TConfiguration> newLayout, TNode node, ref TEnergyData energyData)
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

            var constraintData = energyData.BasicConstraintData;
            constraintData.MoveDistance = newDistance;
            constraintData.Overlap = newOverlap;
            energyData.BasicConstraintData = constraintData;

            return newOverlap == 0 && newDistance == 0;
		}

		private int ComputeOverlap(TConfiguration configuration1, TConfiguration configuration2)
		{
			return roomShapeGeometry.GetOverlapArea(configuration1, configuration2);
		}

		private int ComputeDistance(TConfiguration configuration1, TConfiguration configuration2)
        {
            var distance = roomShapeGeometry.GetCenterDistance(configuration1, configuration2);

			if (distance < 0)
			{
				throw new InvalidOperationException();
			}

			return distance;
		}

        private bool AreNeighbours(ILayout<TNode, TConfiguration> layout, TNode node1, TNode node2)
		{
			return layout.Graph.HasEdge(node1, node2);
		}
	}
}