namespace MapGeneration.Core.Experimental
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Constraints;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using Interfaces;
	using Utils;

	public class LayoutOperations<TNode, TLayout, TConfiguration, TShapeContainer, TEnergyData> : ILayoutOperations<TLayout, TNode>
		where TLayout : IEnergyLayout<TNode, TConfiguration>
		where TConfiguration : IEnergyConfiguration<TConfiguration, TShapeContainer, TEnergyData>, new()
		where TEnergyData : IEnergyData<TEnergyData>, new()
		where TNode : IEquatable<TNode>
	{
		protected readonly IConfigurationSpaces<TNode, TShapeContainer, TConfiguration> ConfigurationSpaces;
		protected readonly IPolygonOverlap PolygonOverlap;
		protected Random Random = new Random();
		private readonly List<IConstraint<TLayout, TNode, TConfiguration, TEnergyData>> constraints = new List<IConstraint<TLayout, TNode, TConfiguration, TEnergyData>>();

		public LayoutOperations(IConfigurationSpaces<TNode, TShapeContainer, TConfiguration> configurationSpaces, IPolygonOverlap polygonOverlap, float energySigma)
		{
			ConfigurationSpaces = configurationSpaces;
			PolygonOverlap = polygonOverlap;
			constraints.Add(new BasicContraint<TLayout, TNode, TConfiguration, TEnergyData, TShapeContainer>(polygonOverlap, energySigma, configurationSpaces));
		}

		public TLayout UpdateLayout(TLayout layout, TNode node, TConfiguration configuration)
		{
			// Prepare new layout with temporary configuration to compute energies
			layout.GetConfiguration(node, out var oldConfiguration);
			var graph = layout.Graph;
			var newLayout = (TLayout)layout.Clone(); // TODO: should it be without the cast?

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
				var newVertexConfiguration = nodeConfiguration;

				var vertexEnergyData = RunAllUpdate(layout, oldConfiguration, configuration, nodeConfiguration, isNeighbour);

				newVertexConfiguration = newVertexConfiguration.SetEnergyData(vertexEnergyData);
				newLayout.SetConfiguration(vertex, newVertexConfiguration);
			}

			// Update energy and validity vector of the perturbed node
			var newEnergyData = RunAllUpdate(node, layout, newLayout);
			configuration = configuration.SetEnergyData(newEnergyData);
			newLayout.SetConfiguration(node, configuration);

			return newLayout;
		}

		public void UpdateLayout(TLayout layout)
		{
			layout.IsValid = true;

			foreach (var node in layout.Graph.Vertices)
			{
				if (!layout.GetConfiguration(node, out var configuration))
					continue;

				var newEnergyData = RunAllCompute(layout, node, configuration);
				var newConfiguration = configuration.SetEnergyData(newEnergyData);
				layout.SetConfiguration(node, newConfiguration);
			}
		}

		private TEnergyData RunAllCompute(TLayout layout, TNode node, TConfiguration configuration)
		{
			var energyData = new TEnergyData().SetIsValid(true);

			foreach (var constraint in constraints)
			{
				energyData = constraint.ComputeEnergyData(layout, node, configuration, energyData);
			}

			return energyData;
		}

		private TEnergyData RunAllUpdate(TLayout layout, TConfiguration oldConfiguration, TConfiguration newConfiguration, TConfiguration configuration, bool areNeighbours)
		{
			var energyData = new TEnergyData().SetIsValid(true);

			foreach (var constraint in constraints)
			{
				energyData = constraint.UpdateEnergyData(layout, oldConfiguration, newConfiguration, configuration, areNeighbours, energyData);
			}

			return energyData;
		}

		private TEnergyData RunAllUpdate(TNode node, TLayout oldLayout, TLayout newLayout)
		{
			var energyData = new TEnergyData().SetIsValid(true);

			foreach (var constraint in constraints)
			{
				energyData = constraint.UpdateEnergyData(oldLayout, newLayout, node, energyData);
			}

			return energyData;
		}

		public bool IsLayoutValid(TLayout layout)
		{
			if (!layout.IsValid)
				return false;

			if (layout.GetAllConfigurations().Any(x => !x.EnergyData.IsValid))
				return false;

			return true;
		}

		public float GetEnergy(TLayout layout)
		{
			return layout.GetAllConfigurations().Sum(x => x.EnergyData.Energy) + layout.Energy;
		}

		/// <summary>
		/// Returns a layout where a given node is position perturbed.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="node">The node that should be perturbed.</param>
		/// <param name="updateLayout">Whether energies and validity vectors should be updated after the change.</param>
		/// <returns></returns>
		public virtual TLayout PerturbPosition(TLayout layout, TNode node, bool updateLayout)
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

			var newPosition = ConfigurationSpaces.GetRandomIntersectionPoint(mainConfiguration, configurations);
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
		public virtual TLayout PerturbPosition(TLayout layout, IList<TNode> nodeOptions, bool updateLayout)
		{
			// TODO: check what would happen if only invalid nodes could be perturbed
			var canBePerturbed = nodeOptions.ToList();

			if (canBePerturbed.Count == 0)
				return (TLayout)layout.Clone(); // TODO: should it be without the cast?

			return PerturbPosition(layout, canBePerturbed.GetRandom(Random), updateLayout);
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
		public virtual TLayout PerturbShape(TLayout layout, TNode node, bool updateLayout)
		{
			layout.GetConfiguration(node, out var configuration);

			// Return the current layout if a given node cannot be shape-perturbed
			if (!ConfigurationSpaces.CanPerturbShape(node))
				return (TLayout)layout.Clone(); // TODO: should it be without the cast?

			TShapeContainer shape;
			do
			{
				shape = ConfigurationSpaces.GetRandomShape(node);
			}
			while (ReferenceEquals(shape, configuration.Shape));

			var newConfiguration = configuration.SetShape(shape);

			if (updateLayout)
			{
				var updated = UpdateLayout(layout, node, newConfiguration);
				return updated;
			}

			var newLayout = (TLayout)layout.Clone(); // TODO: should it be without the cast?
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
		public virtual TLayout PerturbShape(TLayout layout, IList<TNode> nodeOptions, bool updateLayout)
		{
			var canBePerturbed = nodeOptions.Where(x => ConfigurationSpaces.CanPerturbShape(x)).ToList();

			if (canBePerturbed.Count == 0)
				return (TLayout)layout.Clone(); // TODO: should it be without the cast?

			return PerturbShape(layout, canBePerturbed.GetRandom(Random), updateLayout);
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
				layout.SetConfiguration(node, CreateConfiguration(ConfigurationSpaces.GetRandomShape(node), new IntVector2()));
				return;
			}

			var bestEnergy = float.MaxValue;
			var bestShape = default(TShapeContainer);
			var bestPosition = new IntVector2();

			foreach (var shape in ConfigurationSpaces.GetShapesForNode(node))
			{
				var intersection = ConfigurationSpaces.GetMaximumIntersection(CreateConfiguration(shape, new IntVector2()), configurations);

				foreach (var intersectionLine in intersection)
				{
					var tryAll = true;
					var mod = 1;
					const int maxPoints = 20;

					if (intersectionLine.Length > maxPoints)
					{
						tryAll = false;
						mod = intersectionLine.Length / maxPoints;
					}

					var i = 0;

					foreach (var position in intersectionLine.GetPoints())
					{
						if (!tryAll && i % mod != 0 && i != intersectionLine.Length)
							continue;

						var energy = RunAllCompute(layout, node, CreateConfiguration(shape, position)).Energy;

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

						i++;
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


		public void InjectRandomGenerator(Random random)
		{
			this.Random = random;
			ConfigurationSpaces.InjectRandomGenerator(random);
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

		private TEnergyData CreateEnergyData(float energy, int overlap, int distance)
		{
			var energyData = new TEnergyData();
			energyData = energyData.SetEnergy(energy);
			energyData = energyData.SetMoveDistance(distance);
			energyData = energyData.SetOverlap(overlap);

			return energyData;
		}
	}
}