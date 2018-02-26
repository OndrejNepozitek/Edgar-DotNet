namespace MapGeneration.Core.LayoutOperations
{
	using System.Collections.Generic;
	using System.Linq;
	using Constraints;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using Interfaces;
	using Interfaces.Configuration;
	using Interfaces.Configuration.EnergyData;

	public class LayoutOperationsWithConstraints<TLayout, TNode, TConfiguration, TShapeContainer, TEnergyData> : AbstractLayoutOperations<TLayout, TNode, TConfiguration, TShapeContainer>
		where TLayout : IEnergyLayout<TNode, TConfiguration>, ISmartCloneable<TLayout>
		where TConfiguration : IEnergyConfiguration<TShapeContainer, TEnergyData>, ISmartCloneable<TConfiguration>, new()
		where TEnergyData : IEnergyData, new()
	{
		private readonly List<IConstraint<TLayout, TNode, TConfiguration, TEnergyData>> constraints = new List<IConstraint<TLayout, TNode, TConfiguration, TEnergyData>>();

		public LayoutOperationsWithConstraints(IConfigurationSpaces<TNode, TShapeContainer, TConfiguration> configurationSpaces, IPolygonOverlap polygonOverlap, float energySigma) : base(configurationSpaces)
		{
			constraints.Add(new BasicContraint<TLayout, TNode, TConfiguration, TEnergyData, TShapeContainer>(polygonOverlap, energySigma, configurationSpaces));
		}

		public override bool IsLayoutValid(TLayout layout)
		{
			if (!layout.IsValid)
				return false;

			if (layout.GetAllConfigurations().Any(x => !x.EnergyData.IsValid))
				return false;

			return true;
		}

		public override float GetEnergy(TLayout layout)
		{
			return layout.GetAllConfigurations().Sum(x => x.EnergyData.Energy) + layout.Energy;
		}

		public override void UpdateLayout(TLayout layout)
		{
			layout.IsValid = true;

			foreach (var node in layout.Graph.Vertices)
			{
				if (!layout.GetConfiguration(node, out var configuration))
					continue;

				var newEnergyData = RunAllCompute(layout, node, configuration);
				configuration.EnergyData = newEnergyData;
				layout.SetConfiguration(node, configuration);
			}
		}

		public override void AddNodeGreedily(TLayout layout, TNode node)
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

		private TConfiguration CreateConfiguration(TShapeContainer shapeContainer, IntVector2 position)
		{
			var configuration = new TConfiguration
			{
				ShapeContainer = shapeContainer,
				Position = position
			};

			return configuration;
		}

		protected override void UpdateLayout(TLayout layout, TNode node, TConfiguration configuration)
		{
			// Prepare new layout with temporary configuration to compute energies
			var graph = layout.Graph;
			var oldLayout = layout.SmartClone(); // TODO: is the clone needed?
			oldLayout.GetConfiguration(node, out var oldConfiguration);

			// Update validity vectors and energies of vertices
			foreach (var vertex in graph.Vertices)
			{
				if (vertex.Equals(node))
					continue;

				if (!layout.GetConfiguration(vertex, out var nodeConfiguration))
					continue;

				var vertexEnergyData = RunAllUpdate(layout, node, oldConfiguration, configuration, vertex, nodeConfiguration);

				nodeConfiguration.EnergyData = vertexEnergyData;
				layout.SetConfiguration(vertex, nodeConfiguration);
			}

			// Update energy and validity vector of the perturbed node
			var newEnergyData = RunAllUpdate(node, oldLayout, layout);
			configuration.EnergyData = newEnergyData;
			layout.SetConfiguration(node, configuration);
		}

		private TEnergyData RunAllCompute(TLayout layout, TNode node, TConfiguration configuration)
		{
			var energyData = new TEnergyData { IsValid = true };

			foreach (var constraint in constraints)
			{
				constraint.ComputeEnergyData(layout, node, configuration, ref energyData);
			}

			return energyData;
		}

		private TEnergyData RunAllUpdate(TLayout layout, TNode perturbedNode, TConfiguration oldConfiguration, TConfiguration newConfiguration, TNode node, TConfiguration configuration)
		{
			var energyData = new TEnergyData { IsValid = true };

			foreach (var constraint in constraints)
			{
				constraint.UpdateEnergyData(layout, perturbedNode, oldConfiguration, newConfiguration, node, configuration, ref energyData);
			}

			return energyData;
		}

		private TEnergyData RunAllUpdate(TNode node, TLayout oldLayout, TLayout newLayout)
		{
			var energyData = new TEnergyData { IsValid = true };

			foreach (var constraint in constraints)
			{
				constraint.UpdateEnergyData(oldLayout, newLayout, node, ref energyData);
			}

			return energyData;
		}
	}
}