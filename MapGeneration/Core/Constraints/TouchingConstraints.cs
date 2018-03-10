namespace MapGeneration.Core.Constraints
{
	using ConfigurationSpaces;
	using GeneralAlgorithms.Algorithms.Polygons;
	using Interfaces.Core;
	using Interfaces.Core.Configuration;
	using Interfaces.Core.Configuration.EnergyData;
	using Interfaces.Core.ConfigurationSpaces;
	using Interfaces.Core.Constraints;
	using Interfaces.Core.Layouts;
	using Interfaces.Core.MapDescription;

	public class TouchingConstraints<TLayout, TNode, TConfiguration, TEnergyData, TShapeContainer> : INodeConstraint<TLayout, TNode, TConfiguration, TEnergyData>
		where TLayout : ILayout<TNode, TConfiguration>
		where TConfiguration : IEnergyConfiguration<TShapeContainer, TEnergyData>
		where TEnergyData : ICorridorsData, new()
	{
		private readonly ICorridorMapDescription<TNode> mapDescription;
		private readonly IPolygonOverlap<TShapeContainer> polygonOverlap;

		public TouchingConstraints(ICorridorMapDescription<TNode> mapDescription, IPolygonOverlap<TShapeContainer> polygonOverlap)
		{
			this.mapDescription = mapDescription;
			this.polygonOverlap = polygonOverlap;
		}

		public bool ComputeEnergyData(TLayout layout, TNode node, TConfiguration configuration, ref TEnergyData energyData)
		{
			if (mapDescription.IsCorridorRoom(node))
				return true;

			var numberOfTouching = 0;

			foreach (var vertex in layout.Graph.Vertices)
			{
				if (vertex.Equals(node))
					continue;

				if (!layout.GetConfiguration(vertex, out var c))
					continue;

				if (mapDescription.IsCorridorRoom(vertex))
					continue;

				if (DoTouch(configuration, c))
				{
					numberOfTouching++;
				}
			}

			energyData.NumberOfTouching = numberOfTouching;
			energyData.Energy += numberOfTouching;

			return numberOfTouching == 0;
		}

		public bool UpdateEnergyData(TLayout layout, TNode perturbedNode, TConfiguration oldConfiguration,
			TConfiguration newConfiguration, TNode node, TConfiguration configuration, ref TEnergyData energyData)
		{
			if (mapDescription.IsCorridorRoom(perturbedNode) || mapDescription.IsCorridorRoom(node))
				return true;

			var isTouchingOld = 0;
			var isTouchingNew = 0;

			if (!AreNeighbours(layout, perturbedNode, node))
			{
				isTouchingOld = DoTouch(configuration, oldConfiguration) ? 1 : 0;
				isTouchingNew = DoTouch(configuration, newConfiguration) ? 1 : 0;
			}

			//var overlapOld = ComputeOverlap(configuration, oldConfiguration);
			//var overlapNew = ComputeOverlap(configuration, newConfiguration);

			var numberOfTouchingTotal = configuration.EnergyData.NumberOfTouching + (isTouchingNew - isTouchingOld);

			energyData.NumberOfTouching = numberOfTouchingTotal;
			energyData.Energy += numberOfTouchingTotal;

			return numberOfTouchingTotal == 0;
		}

		public bool UpdateEnergyData(TLayout oldLayout, TLayout newLayout, TNode node, ref TEnergyData energyData)
		{
			if (mapDescription.IsCorridorRoom(node))
				return true;

			oldLayout.GetConfiguration(node, out var oldConfiguration);
			var newNumberOfTouching = oldConfiguration.EnergyData.NumberOfTouching;

			foreach (var vertex in oldLayout.Graph.Vertices)
			{
				if (vertex.Equals(node))
					continue;

				if (!oldLayout.GetConfiguration(vertex, out var nodeConfiguration))
					continue;

				newLayout.GetConfiguration(vertex, out var newNodeConfiguration);

				newNumberOfTouching += newNodeConfiguration.EnergyData.NumberOfTouching - nodeConfiguration.EnergyData.NumberOfTouching;
			}

			energyData.NumberOfTouching = newNumberOfTouching;
			energyData.Energy += newNumberOfTouching;

			return newNumberOfTouching == 0;
		}

		private bool DoTouch(TConfiguration configuration1, TConfiguration configuration2)
		{
			return polygonOverlap.DoTouch(configuration1.ShapeContainer, configuration1.Position, configuration2.ShapeContainer, configuration2.Position, 1);
		}

		private int ComputeOverlap(TConfiguration configuration1, TConfiguration configuration2)
		{
			return polygonOverlap.OverlapArea(configuration1.ShapeContainer, configuration1.Position, configuration2.ShapeContainer, configuration2.Position);
		}

		private bool AreNeighbours(TLayout layout, TNode node1, TNode node2)
		{
			return layout.Graph.HasEdge(node1, node2);
		}
	}
}