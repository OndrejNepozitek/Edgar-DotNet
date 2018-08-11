namespace MapGeneration.Core.LayoutOperations
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using ConfigurationSpaces;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Graphs;
	using Interfaces.Core.Configuration;
	using Interfaces.Core.Configuration.EnergyData;
	using Interfaces.Core.ConfigurationSpaces;
	using Interfaces.Core.LayoutOperations;
	using Interfaces.Core.Layouts;
	using Interfaces.Core.MapDescriptions;
	using Interfaces.Utils;
	using Utils;

	/// <summary>
	/// Layout operations for evolving layouts with corridors.
	/// </summary>
	public class LayoutOperationsWithCorridors<TLayout, TNode, TConfiguration, TShapeContainer, TEnergyData, TLayoutEnergyData> : 
		LayoutOperationsWithConstraints<TLayout, TNode, TConfiguration, TShapeContainer, TEnergyData, TLayoutEnergyData>,
		ILayoutOperationsWithCorridors<TLayout, TNode>
		where TLayout : IEnergyLayout<TNode, TConfiguration, TLayoutEnergyData>, ISmartCloneable<TLayout> 
		where TConfiguration : IEnergyConfiguration<TShapeContainer, TEnergyData>, ISmartCloneable<TConfiguration>, new()
		where TEnergyData : IEnergyData, new()
		where TLayoutEnergyData : IEnergyData, new()
	{
		protected readonly IConfigurationSpaces<TNode, TShapeContainer, TConfiguration, ConfigurationSpace> CorridorConfigurationSpaces;
		protected readonly ICorridorMapDescription<TNode> MapDescription;
		protected readonly IGraph<TNode> GraphWithoutCorridors;

		public LayoutOperationsWithCorridors(
			IConfigurationSpaces<TNode, TShapeContainer, TConfiguration, ConfigurationSpace> configurationSpaces,
			ICorridorMapDescription<TNode> mapDescription,
			IConfigurationSpaces<TNode, TShapeContainer, TConfiguration, ConfigurationSpace> corridorConfigurationSpaces,
			int averageSize
			) : base(configurationSpaces, averageSize)
		{
			MapDescription = mapDescription;
			CorridorConfigurationSpaces = corridorConfigurationSpaces;
			GraphWithoutCorridors = mapDescription.GetGraphWithoutCorrridors();
		}

		/// <summary>
		/// Perturbs a non corridor position using configuration spaces generated especially for perturbing non corridors.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="node"></param>
		/// <param name="updateLayout"></param>
		protected void PerturbNonCorridorPosition(TLayout layout, TNode node, bool updateLayout)
		{
			var configurations = GetNeighboursOverCorridors(layout, node);

			if (!layout.GetConfiguration(node, out var mainConfiguration))
				throw new InvalidOperationException();

			var newPosition = CorridorConfigurationSpaces.GetRandomIntersectionPoint(mainConfiguration, configurations);
			var newConfiguration = mainConfiguration.SmartClone();
			newConfiguration.Position = newPosition;

			if (updateLayout)
			{
				UpdateLayout(layout, node, newConfiguration);
				return;
			}

			layout.SetConfiguration(node, newConfiguration);
		}

		/// <summary>
		/// Perturbs non corridor rooms until a valid layout is found.
		/// Then tries to use a greedy algorithm to lay out corridor rooms.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="chain"></param>
		/// <param name="updateLayout"></param>
		public override void PerturbLayout(TLayout layout, IList<TNode> chain, bool updateLayout)
		{
			if (!MapDescription.IsWithCorridors)
			{
				base.PerturbLayout(layout, chain, updateLayout);
				return;
			}

			var nonCorridors = chain.Where(x => !MapDescription.IsCorridorRoom(x)).ToList();
			var firstCorridor = chain.First(x => MapDescription.IsCorridorRoom(x));

			if (layout.GetConfiguration(firstCorridor, out var _))
			{
				foreach (var corridor in chain.Where(x => MapDescription.IsCorridorRoom(x)))
				{
					layout.RemoveConfiguration(corridor);
				}
			}

			if (Random.NextDouble() < 0.4f)
			{
				PerturbShape(layout, nonCorridors, updateLayout);
			}
			else
			{
				var random = nonCorridors.GetRandom(Random);
				PerturbNonCorridorPosition(layout, random, updateLayout);
			}
		}

		/// <inheritdoc />
		public bool AddCorridors(TLayout layout, IList<TNode> chain)
		{
			if (AddCorridorsInternal(layout, chain))
			{
				UpdateLayout(layout);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Greedily adds corridors from a given chain to the layout.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="chain"></param>
		/// <returns></returns>
		private bool AddCorridorsInternal(TLayout layout, IEnumerable<TNode> chain)
		{
			var clone = layout.SmartClone();
			var corridors = chain.Where(x => MapDescription.IsCorridorRoom(x)).ToList();

			foreach (var corridor in corridors)
			{
				if (!AddCorridorGreedily(clone, corridor))
					return false;
			}

			foreach (var corridor in corridors)
			{
				clone.GetConfiguration(corridor, out var configuration);
				layout.SetConfiguration(corridor, configuration);
			}

			return true;
		}

		/// <summary>
		/// Gets neighbours from the original graph without corridors.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		protected List<TConfiguration> GetNeighboursOverCorridors(TLayout layout, TNode node)
		{
			var configurations = new List<TConfiguration>();

			foreach (var neighbour in GraphWithoutCorridors.GetNeighbours(node))
			{
				if (layout.GetConfiguration(neighbour, out var configuration))
				{
					configurations.Add(configuration);
				}
			}

			return configurations;
		}

		/// <summary>
		/// Greedily adds only non corridor nodes to the layout.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="chain"></param>
		/// <param name="updateLayout"></param>
		public override void AddChain(TLayout layout, IList<TNode> chain, bool updateLayout)
		{
			if (!MapDescription.IsWithCorridors)
			{
				base.AddChain(layout, chain, updateLayout);
				return;
			}

			var rooms = chain.Where(x => !MapDescription.IsCorridorRoom(x));

			foreach (var room in rooms)
			{
				AddNonCorridorGreedily(layout, room);
			}

			if (updateLayout)
			{
				UpdateLayout(layout);
			}
		}

		/// <summary>
		/// Greedily adds non corridor node to the layout.
		/// </summary>
		/// <remarks>
		/// Uses special configuration spaces where shapes are not directly connected
		/// but are rather at a specified distance from each other.
		/// </remarks>
		/// <param name="layout"></param>
		/// <param name="node"></param>
		private void AddNonCorridorGreedily(TLayout layout, TNode node)
		{
			var configurations = GetNeighboursOverCorridors(layout, node);

			if (configurations.Count == 0)
			{
				layout.SetConfiguration(node, CreateConfiguration(ConfigurationSpaces.GetRandomShape(node), new IntVector2()));
				return;
			}

			var bestEnergy = float.MaxValue;
			var bestShape = default(TShapeContainer);
			var bestPosition = new IntVector2();

			var shapes = ConfigurationSpaces.GetShapesForNode(node).ToList();
			shapes.Shuffle(Random);

			foreach (var shape in shapes)
			{
				var intersection = CorridorConfigurationSpaces.GetMaximumIntersection(CreateConfiguration(shape, new IntVector2()), configurations);

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

						var energy = NodeComputeEnergyData(layout, node, CreateConfiguration(shape, position)).Energy;

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

		/// <summary>
		/// Adds corridor node greedily.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		public bool AddCorridorGreedily(TLayout layout, TNode node)
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
				throw new InvalidOperationException();
			}

			var foundValid = false;
			var bestShape = default(TShapeContainer);
			var bestPosition = new IntVector2();

			var shapes = ConfigurationSpaces.GetShapesForNode(node).ToList();
			shapes.Shuffle(Random);

			foreach (var shape in shapes)
			{
				var intersection = ConfigurationSpaces.GetMaximumIntersection(CreateConfiguration(shape, new IntVector2()), configurations, out var configurationsSatisfied);

				if (configurationsSatisfied != 2)
					continue;

				intersection.Shuffle(Random);

				foreach (var intersectionLine in intersection)
				{
					const int maxPoints = 20;

					if (intersectionLine.Length > maxPoints)
					{
						var mod = intersectionLine.Length / maxPoints - 1;

						for (var i = 0; i < maxPoints; i++)
						{
							var position = intersectionLine.GetNthPoint(i != maxPoints - 1 ? i * mod : intersectionLine.Length + 1);

							var energyData = NodeComputeEnergyData(layout, node, CreateConfiguration(shape, position));

							if (energyData.IsValid)
							{
								bestShape = shape;
								bestPosition = position;
								foundValid = true;
								break;
							}

							if (foundValid)
							{
								break;
							}
						}
					}
					else
					{
						var points = intersectionLine.GetPoints();
						points.Shuffle(Random);

						foreach (var position in points)
						{
							var energyData = NodeComputeEnergyData(layout, node, CreateConfiguration(shape, position));

							if (energyData.IsValid)
							{
								bestShape = shape;
								bestPosition = position;
								foundValid = true;
								break;
							}

							if (foundValid)
							{
								break;
							}
						}
					}

					if (foundValid)
					{
						break;
					}
				}
			}

			var newConfiguration = CreateConfiguration(bestShape, bestPosition);
			layout.SetConfiguration(node, newConfiguration);

			return foundValid;
		}

		/// <inheritdoc />
		public override void InjectRandomGenerator(Random random)
		{
			base.InjectRandomGenerator(random);

			if (CorridorConfigurationSpaces is IRandomInjectable injectable) injectable.InjectRandomGenerator(random);
		}
	}
}