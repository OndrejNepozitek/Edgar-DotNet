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
		where TConfiguration : IEnergyConfiguration<TConfiguration, TShapeContainer>, new()
		where TNode : IEquatable<TNode>
	{
		private readonly IConfigurationSpaces<TNode, TShapeContainer, TConfiguration> configurationSpaces;
		private readonly IPolygonOverlap polygonOverlap;
		private Random random = new Random();

		private readonly float energySigma = 15800f; // TODO: change

		public LayoutOperations(IConfigurationSpaces<TNode, TShapeContainer, TConfiguration> configurationSpaces, IPolygonOverlap polygonOverlap, float energySigma = 0)
		{
			this.configurationSpaces = configurationSpaces;
			this.polygonOverlap = polygonOverlap;
			this.energySigma = energySigma;
		}

		/// <summary>
		/// Returns a layout where a given node is shape perturbed.
		/// If a given node cannot be perturbed, return a copy of the same layout.
		/// </summary>
		/// <remarks>
		/// Energies and validity vectors are unchanged if a given node cannot be
		/// shape perturbed.
		/// </remarks>
		/// <param name="layout"></param>
		/// <param name="node">The node that should be perturbed.</param>
		/// <param name="updateLayout">Whether energies and validity vectors should be updated after the change.</param>
		/// <returns></returns>
		public TLayout PerturbShape(TLayout layout, TNode node, bool updateLayout, bool perturbPosition)
		{
			if (!updateLayout && perturbPosition)
				throw new InvalidOperationException();

			layout.GetConfiguration(node, out var configuration);

			// Return the current layout if a given node cannot be shape-perturbed
			if (!configurationSpaces.CanPerturbShape(node))
				return (TLayout) layout.Clone(); // TODO: should it be without the cast?

			TShapeContainer shape;
			do
			{
				shape = configurationSpaces.GetRandomShape(node);
			}
			while (ReferenceEquals(shape, configuration.Shape));

			var newConfiguration = configuration.SetShape(shape);

			if (updateLayout)
			{
				var updated = UpdateLayout(layout, node, newConfiguration);

				if (perturbPosition)
				{
					updated = PerturbPosition(updated, node, updateLayout);
				}

				return updated;
			}

			var newLayout = (TLayout) layout.Clone(); // TODO: should it be without the cast?
			newLayout.SetConfiguration(node, newConfiguration);

			return newLayout;
		}

		/// <summary>
		/// Returns a layout where a random node from given options is shape perturbed.
		/// If none of the options can be perturbed, return a copy of the same layout.
		/// </summary>
		/// <remarks>
		/// Energies and validity vectors are unchanged if none of given node can be
		/// shape perturbed.
		/// </remarks>
		/// <param name="layout"></param>
		/// <param name="nodeOptions"></param>
		/// <param name="updateLayout">Whether energies and validity vectors should be updated after the change.</param>
		/// <returns></returns>
		public TLayout PerturbShape(TLayout layout, IList<TNode> nodeOptions, bool updateLayout, bool perturbPosition)
		{
			var canBePerturbed = nodeOptions.Where(x => configurationSpaces.CanPerturbShape(x)).ToList();

			if (canBePerturbed.Count == 0)
				return (TLayout) layout.Clone(); // TODO: should it be without the cast?

			return PerturbShape(layout, canBePerturbed.GetRandom(random), updateLayout, perturbPosition);
		}

		/// <summary>
		/// Returns a layout where a given node is position perturbed.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="node">The node that should be perturbed.</param>
		/// <param name="updateLayout">Whether energies and validity vectors should be updated after the change.</param>
		/// <returns></returns>
		public TLayout PerturbPosition(TLayout layout, TNode node, bool updateLayout)
		{
			var configurations = new List<TConfiguration>();

			foreach (var neighbour in layout.Graph.GetNeighbours(node))
			{
				if (layout.GetConfiguration(neighbour, out var configuration))
				{
					configurations.Add(configuration);
				}
			}

			if (!layout.GetConfiguration(node, out var mainConfiguration))
				throw new InvalidOperationException();

			var newPosition = configurationSpaces.GetRandomIntersectionPoint(mainConfiguration, configurations);
			var newConfiguration = mainConfiguration.SetPosition(newPosition);

			if (updateLayout)
			{
				return UpdateLayout(layout, node, newConfiguration);
			}

			var newLayout = (TLayout)layout.Clone(); // TODO: should it be without the cast?
			newLayout.SetConfiguration(node, newConfiguration);

			return newLayout;
		}

		/// <summary>
		/// Returns a layout where a random node from given options is position perturbed.
		/// If none of the options can be perturbed, return a copy of the same layout.
		/// </summary>
		/// <remarks>
		/// Energies and validity vectors are unchanged if there is no node to be shape perturbed.
		/// </remarks>
		/// <param name="layout"></param>
		/// <param name="nodeOptions"></param>
		/// <param name="updateLayout">Whether energies and validity vectors should be updated after the change.</param>
		/// <returns></returns>
		public TLayout PerturbPosition(TLayout layout, IList<TNode> nodeOptions, bool updateLayout)
		{
			// TODO: check what would happen if only invalid nodes could be perturbed
			var canBePerturbed = nodeOptions.ToList();

			if (canBePerturbed.Count == 0)
				return (TLayout) layout.Clone(); // TODO: should it be without the cast?

			return PerturbPosition(layout, canBePerturbed.GetRandom(random), updateLayout);
		}

		/// <summary>
		/// The method returns a layout where a given node has a configuration given as an argument
		/// and all validity vectors and energies are updated.
		/// </summary>
		/// <remarks>
		/// All validity vectors and energies of a given layout must be up-to-date for this method to work.
		/// A given layout is not changed if its implementation follows the clone semantics.
		/// </remarks>
		/// <param name="layout">The layout that should be the base for changing a given node.</param>
		/// <param name="node">The node to be changed.</param>
		/// <param name="configuration">The new configuration of the node that is changed.</param>
		/// <returns></returns>
		public TLayout UpdateLayout(TLayout layout, TNode node, TConfiguration configuration)
		{
			// Prepare new layout with temporary configuration to compute energies
			layout.GetConfiguration(node, out var oldConfiguration);
			var graph = layout.Graph;
			var newLayout = (TLayout) layout.Clone(); // TODO: should it be without the cast?

			// Having these variables saves us one call to GetEnergy()
			var newOverlap = oldConfiguration.EnergyData.Area;
			var newDistance = oldConfiguration.EnergyData.MoveDistance;

			// Recalculate validities
			var validityVector = configuration.ValidityVector;
			var neighbours = graph.GetNeighbours(node).ToList(); // TODO: can we avoid ToList() ?

			// Update validity vectors and energies of vertices
			foreach (var vertex in graph.Vertices)
			{
				if (vertex.Equals(node))
					continue;

				if (!layout.GetConfiguration(vertex, out var nodeConfiguration))
					continue;

				var neighbourIndex = neighbours.IndexOf(vertex);
				var isNeighbour = neighbourIndex != -1;
				var updateEnergies = true;
				var newVertexConfiguration = nodeConfiguration;
				var isNeighbourValid = false; // This variable has a meaningful value only if the current vertex is a neighbour

				// If the vertex is a neighbour of the perturbed node, we must check if its validity vector changed
				if (isNeighbour)
				{
					var neighbourValidityVector = nodeConfiguration.ValidityVector;
					isNeighbourValid = configurationSpaces.HaveValidPosition(configuration, nodeConfiguration);
					var reverseNeighbourIndex = graph.GetNeighbourIndex(vertex, node);

					// We must check changes
					// Invalid neighbours must be checked even without changes because their energy could change
					if (neighbourValidityVector[reverseNeighbourIndex] != !isNeighbourValid || neighbourValidityVector[reverseNeighbourIndex])
					{
						neighbourValidityVector[reverseNeighbourIndex] = !isNeighbourValid;
						validityVector[neighbourIndex] = !isNeighbourValid;

						newVertexConfiguration = newVertexConfiguration.SetValidityVector(neighbourValidityVector);
					}
					else
					{
						// We got here if the two nodes were valid before the change and are valid also after it.
						// That means that both the energy and validity vector did not changed a we don't have to check it.
						updateEnergies = false;
					}
				}

				if (!updateEnergies)
					continue;

				var vertexEnergyData = RecomputeEnergyData(oldConfiguration, configuration, nodeConfiguration, isNeighbour, isNeighbourValid);
				newVertexConfiguration = newVertexConfiguration.SetEnergyData(vertexEnergyData);
				newLayout.SetConfiguration(vertex, newVertexConfiguration);

				newOverlap += vertexEnergyData.Area - nodeConfiguration.EnergyData.Area;
				newDistance += vertexEnergyData.MoveDistance - nodeConfiguration.EnergyData.MoveDistance;
			}

			// Update energy and validity vector of the perturbed node
			var newEnergy = ComputeEnergy(newOverlap, newDistance);
			configuration = configuration.SetEnergyData(new EnergyData(newEnergy, newOverlap, newDistance));
			configuration = configuration.SetValidityVector(validityVector);

			newLayout.SetConfiguration(node, configuration);

			return newLayout;
		}

		/// <summary>
		/// Get a new EnergyData for a given node with respect to the change from oldConfiguration to newConfiguration.
		/// </summary>
		/// <param name="oldConfiguration">Old configuration of the node that changed its position/shape.</param>
		/// <param name="newConfiguration">New configuration of the node that changed its position/shape.</param>
		/// <param name="configuration">The configuration for which we want to get a new EnergyData.</param>
		/// <param name="areNeighbours">Whether the two nodes are neighbours.</param>
		/// <param name="validNew">Whether neighbouring nodes have a valid position in the new layout.</param>
		/// <returns></returns>
		protected EnergyData RecomputeEnergyData(TConfiguration oldConfiguration, TConfiguration newConfiguration,
			TConfiguration configuration, bool areNeighbours, bool validNew)
		{
			var overlapOld = ComputeOverlap(configuration, oldConfiguration);
			var overlapNew = ComputeOverlap(configuration, newConfiguration);
			var overlapTotal = configuration.EnergyData.Area + (overlapNew - overlapOld);

			// MoveDistance should not be recomputed as it is used only when two nodes are neighbours which is not the case here
			var distanceTotal = configuration.EnergyData.MoveDistance;
			if (areNeighbours)
			{
				// TODO: either compute it twice here or use info from validity vectors - what is better? throw away validity vectors?
				var validOld = configurationSpaces.HaveValidPosition(oldConfiguration, configuration);

				// Distance is taken into account only when there is no overlap
				var distanceOld = overlapOld == 0 && !validOld ? ComputeDistance(configuration, oldConfiguration) : 0;
				var distanceNew = overlapNew == 0 && !validNew ? ComputeDistance(configuration, newConfiguration) : 0;
				distanceTotal = configuration.EnergyData.MoveDistance + (distanceNew - distanceOld);
			}

			var newEnergy = ComputeEnergy(overlapTotal, distanceTotal);

			return new EnergyData(newEnergy, overlapTotal, distanceTotal);
		}

		/// <summary>
		/// Recompute validity vectors of all nodes.
		/// </summary>
		/// <remarks>
		/// This method is not written for speed. It is much better to change just validity vectors
		/// that need to be changed than to recompute them all.
		/// </remarks>
		/// <param name="layout"></param>
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

		/// <summary>
		/// Check if a given layout is valid.
		/// </summary>
		/// <remarks>
		/// Validity vectors and energy data must be up-to-date for this to work.
		/// </remarks>
		/// <param name="layout"></param>
		/// <returns></returns>
		public bool IsLayoutValid(TLayout layout)
		{
			if (layout.GetAllConfigurations().Any(x => !x.IsValid || x.EnergyData.Energy > 0))
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Recompute energies of all nodes.
		/// </summary>
		/// <remarks>
		/// This method is not written for speed. It is much better to change just energies
		/// that need to be changed than to recompute them all.
		/// </remarks>
		/// <param name="layout"></param>
		public void RecomputeEnergy(TLayout layout)
		{
			foreach (var vertex in layout.Graph.Vertices)
			{
				if (!layout.GetConfiguration(vertex, out var configuration))
					continue;

				var energyData = GetEnergyData(layout, vertex, configuration);
				layout.SetConfiguration(vertex, configuration.SetEnergyData(energyData));
			}
		}

		/// <summary>
		/// Compute an energy of a given node.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="node"></param>
		/// <param name="configuration"></param>
		/// <returns>Zero if the configuration is valid and a positive number otherwise.</returns>
		public float GetEnergy(TLayout layout, TNode node, TConfiguration configuration)
		{
			return GetEnergyData(layout, node, configuration).Energy;
		}

		/// <summary>
		/// Compute an energy of a given node.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="node"></param>
		/// <param name="configuration"></param>
		/// <returns>Zero if the configuration is valid and a positive number otherwise.</returns>
		public EnergyData GetEnergyData(TLayout layout, TNode node, TConfiguration configuration)
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

			return new EnergyData(energy, overlap, distance);
		}

		/// <summary>
		/// Find a position and a shape for a given node that minimizes
		/// its energy with respect to already laid out nodes.
		/// </summary>
		/// <remarks>
		/// All positions and shapes are tried to determine the best combination.
		/// Energies and validity vectors are NOT updated.
		/// </remarks>
		/// <param name="layout"></param>
		/// <param name="node"></param>
		public void AddNodeGreedily(TLayout layout, TNode node)
		{
			var configurations = new List<TConfiguration>();
			var neighbours = layout.Graph.GetNeighbours(node);

			foreach (var neighbour in neighbours)
			{
				if (layout.GetConfiguration(neighbour, out var configuration))
				{
					configurations.Add(configuration);
				}
			}

			if (configurations.Count == 0)
			{
				layout.SetConfiguration(node, CreateConfiguration(configurationSpaces.GetRandomShape(node), new IntVector2()));
				return;
			}

			var bestEnergy = float.MaxValue;
			var bestShape = default(TShapeContainer);
			var bestPosition = new IntVector2();

			foreach (var shape in configurationSpaces.GetShapesForNode(node))
			{
				var intersection = configurationSpaces.GetMaximumIntersection(CreateConfiguration(shape, new IntVector2()), configurations);

				foreach (var intersectionLine in intersection)
				{
					foreach (var position in intersectionLine.GetPoints())
					{
						var energy = GetEnergy(layout, node, CreateConfiguration(shape, position));

						if (energy < bestEnergy)
						{
							bestEnergy = energy;
							bestShape = shape;
							bestPosition = position;
						}

						if (bestEnergy <= 0)
						{
							break;
						}
					}

					// There is no point of looking for more solutions when you already reached a valid state
					// and so no position would be accepted anyway
					// TODO: What about making it somehow random? If there are more valid positions, choose one at random.
					if (bestEnergy <= 0)
					{
						break;
					}
				}
			}

			var newConfiguration = CreateConfiguration(bestShape, bestPosition);
			layout.SetConfiguration(node, newConfiguration);
		}

		/// <summary>
		/// Create a new configuration with given shape and position.
		/// </summary>
		/// <param name="shapeContainer"></param>
		/// <param name="position"></param>
		/// <returns></returns>
		protected TConfiguration CreateConfiguration(TShapeContainer shapeContainer, IntVector2 position)
		{
			var configuration = new TConfiguration();
			configuration = configuration.SetShape(shapeContainer);
			configuration = configuration.SetPosition(position);

			return configuration;
		}

		protected int ComputeOverlap(TConfiguration configuration1, TConfiguration configuration2)
		{
			return polygonOverlap.OverlapArea(configuration1.Shape, configuration1.Position, configuration2.Shape, configuration2.Position);
		}

		protected int ComputeDistance(TConfiguration configuration1, TConfiguration configuration2)
		{
			var distance = IntVector2.ManhattanDistance(configuration1.Shape.BoundingRectangle.Center + configuration1.Position,
				configuration2.Shape.BoundingRectangle.Center + configuration2.Position);

			if (distance < 0)
			{
				throw new InvalidOperationException();
			}

			return distance;
		}

		protected float ComputeEnergy(int overlap, float distance)
		{
			return (float)(Math.Pow(Math.E, overlap / (energySigma * 625)) * Math.Pow(Math.E, distance / (energySigma * 50)) - 1);
		}

		public void InjectRandomGenerator(Random random)
		{
			this.random = random;
			configurationSpaces.InjectRandomGenerator(random);
		}
	}
}
