using Edgar.Graphs;
using Edgar.Legacy.Core.Configurations.Interfaces;
using Edgar.Legacy.Core.Configurations.Interfaces.EnergyData;
using Edgar.Legacy.Core.Constraints.Interfaces;
using Edgar.Legacy.Core.Layouts.Interfaces;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.Core.MapDescriptions.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;

namespace Edgar.Legacy.Core.Constraints
{
    public class TouchingConstraints
    <TLayout, TNode, TConfiguration, TEnergyData, TShapeContainer> : INodeConstraint<TLayout, TNode, TConfiguration, TEnergyData>
		where TLayout : ILayout<TNode, TConfiguration>
		where TConfiguration : IEnergyConfiguration<TShapeContainer, TNode, TEnergyData>
		where TEnergyData : ICorridorsData, new()
	{
		private readonly IMapDescription<TNode> mapDescription;
		private readonly IPolygonOverlap<TShapeContainer> polygonOverlap;
        private readonly IGraph<TNode> stageOneGraph;
        private readonly IGraph<TNode> graph;

		public TouchingConstraints(IMapDescription<TNode> mapDescription, IPolygonOverlap<TShapeContainer> polygonOverlap)
		{
			this.mapDescription = mapDescription;
			this.polygonOverlap = polygonOverlap;
            stageOneGraph = mapDescription.GetStageOneGraph();
            graph = mapDescription.GetGraph();
		}

		/// <inheritdoc />
		public bool ComputeEnergyData(TLayout layout, TNode node, TConfiguration configuration, ref TEnergyData energyData)
		{
			if (mapDescription.GetRoomDescription(node).GetType() == typeof(CorridorRoomDescription))
				return true;

			var numberOfTouching = 0;

			foreach (var vertex in layout.Graph.Vertices)
			{
				if (vertex.Equals(node))
					continue;

				if (!layout.GetConfiguration(vertex, out var c))
					continue;

                if (mapDescription.GetRoomDescription(vertex).GetType() == typeof(CorridorRoomDescription))
                    continue;

				if (AreNeighbours(node, vertex))
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

		/// <inheritdoc />
		public bool UpdateEnergyData(TLayout layout, TNode perturbedNode, TConfiguration oldConfiguration,
			TConfiguration newConfiguration, TNode node, TConfiguration configuration, ref TEnergyData energyData)
		{
			if (mapDescription.GetRoomDescription(node).GetType() == typeof(CorridorRoomDescription) || mapDescription.GetRoomDescription(perturbedNode).GetType() == typeof(CorridorRoomDescription))
				return true;

			var isTouchingOld = 0;
			var isTouchingNew = 0;

			if (!AreNeighbours(perturbedNode, node))
			{
				isTouchingOld += DoTouch(configuration, oldConfiguration) ? 1 : 0;
				isTouchingNew += DoTouch(configuration, newConfiguration) ? 1 : 0;
			}

			var numberOfTouchingTotal = configuration.EnergyData.NumberOfTouching + (isTouchingNew - isTouchingOld);

			energyData.NumberOfTouching = numberOfTouchingTotal;
			energyData.Energy += numberOfTouchingTotal;

			return numberOfTouchingTotal == 0;
		}

		/// <inheritdoc />
		public bool UpdateEnergyData(TLayout oldLayout, TLayout newLayout, TNode node, ref TEnergyData energyData)
		{
			if (mapDescription.GetRoomDescription(node).GetType() == typeof(CorridorRoomDescription))
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
			return polygonOverlap.DoTouch(configuration1.ShapeContainer, configuration1.Position, configuration2.ShapeContainer, configuration2.Position, 0);
		}

        private bool AreNeighbours(TNode node1, TNode node2)
        {
            return graph.HasEdge(node1, node2);
        }

        private bool AreNeighboursWithoutCorridors(TNode node1, TNode node2)
        {
            return stageOneGraph.HasEdge(node1, node2) && !graph.HasEdge(node1, node2);
        }
	}
}