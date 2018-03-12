namespace MapGeneration.Core.Constraints
{
	using System;
	using System.Linq;
	using ConfigurationSpaces;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Graphs;
	using Interfaces.Core.Configuration;
	using Interfaces.Core.Configuration.EnergyData;
	using Interfaces.Core.ConfigurationSpaces;
	using Interfaces.Core.Constraints;
	using Interfaces.Core.Layouts;
	using Interfaces.Core.MapDescription;

	/// <summary>
	/// Constraint made for map descriptions with corridors.
	/// </summary>
	/// <remarks>
	/// It ensures that neighbours in the original graph without corridors are exactly a specified
	/// offset away from each other. It is then really easy to use a hungry algorithm to connect
	/// rooms by corridors. Valid positions are check with modified configurations spaces.
	/// </remarks>
	public class CorridorConstraints<TLayout, TNode, TConfiguration, TEnergyData, TShapeContainer> : INodeConstraint<TLayout, TNode, TConfiguration, TEnergyData>
		where TLayout : ILayout<TNode, TConfiguration>
		where TConfiguration : IEnergyConfiguration<TShapeContainer, TEnergyData>
		where TEnergyData : ICorridorsData, new()
	{
		private readonly ICorridorMapDescription<TNode> mapDescription;
		private readonly float energySigma;
		private readonly IConfigurationSpaces<TNode, TShapeContainer, TConfiguration, ConfigurationSpace> configurationSpaces;
		private readonly IGraph<TNode> graphWithoutCorridors;

		public CorridorConstraints(ICorridorMapDescription<TNode> mapDescription, float averageSize, IConfigurationSpaces<TNode, TShapeContainer, TConfiguration, ConfigurationSpace> configurationSpaces)
		{
			this.mapDescription = mapDescription;
			this.energySigma = 10 * averageSize; // TODO: should it be like this?
			this.configurationSpaces = configurationSpaces;
			graphWithoutCorridors = this.mapDescription.GetGraphWithoutCorrridors();
		}

		/// <inheritdoc />
		public bool ComputeEnergyData(TLayout layout, TNode node, TConfiguration configuration, ref TEnergyData energyData)
		{
			if (mapDescription.IsCorridorRoom(node))
				return true;

			var distance = 0;
			var neighbours = graphWithoutCorridors.GetNeighbours(node).ToList();

			foreach (var vertex in neighbours)
			{
				if (vertex.Equals(node))
					continue;

				if (!layout.GetConfiguration(vertex, out var c))
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
			if (mapDescription.IsCorridorRoom(node))
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
			if (mapDescription.IsCorridorRoom(node))
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
			return graphWithoutCorridors.HasEdge(node1, node2);
		}
	}
}