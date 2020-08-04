using Edgar.GraphBasedGenerator.Constraints.CorridorConstraint;
using GeneralAlgorithms.Algorithms.Polygons;
using GeneralAlgorithms.DataStructures.Graphs;
using MapGeneration.Core.Configurations.Interfaces;
using MapGeneration.Core.Configurations.Interfaces.EnergyData;
using MapGeneration.Core.Constraints.Interfaces;
using MapGeneration.Core.Layouts.Interfaces;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.MapDescriptions.Interfaces;

namespace Edgar.GraphBasedGenerator.Constraints.MinimumDistanceConstraint
{
    public class MinimumDistanceConstraint
    <TLayout, TNode, TConfiguration, TEnergyData, TShapeContainer> : INodeConstraint<TLayout, TNode, TConfiguration, TEnergyData>
		where TLayout : ILayout<TNode, TConfiguration>
		where TConfiguration : IEnergyConfiguration<TShapeContainer, TNode, TEnergyData>
		where TEnergyData : IMinimumDistanceConstraintData, IEnergyData
	{
        private readonly IMapDescription<TNode> mapDescription;
		private readonly IPolygonOverlap<TShapeContainer> polygonOverlap;
        private readonly IGraph<TNode> stageOneGraph;
        private readonly IGraph<TNode> graph;
        private readonly int minimumDistance;

		public MinimumDistanceConstraint(IMapDescription<TNode> mapDescription, IPolygonOverlap<TShapeContainer> polygonOverlap, int minimumDistance)
		{
			this.mapDescription = mapDescription;
			this.polygonOverlap = polygonOverlap;
            this.minimumDistance = minimumDistance;
            stageOneGraph = mapDescription.GetStageOneGraph();
            graph = mapDescription.GetGraph();
		}

		/// <inheritdoc />
		public bool ComputeEnergyData(TLayout layout, TNode node, TConfiguration configuration, ref TEnergyData energyData)
		{
			// TODO: why this?
			if (mapDescription.GetRoomDescription(node).GetType() == typeof(CorridorRoomDescription))
				return true;

			var wrongDistanceCount = 0;

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
					wrongDistanceCount++;
				}
			}

            var constraintData = new MinimumDistanceConstraintData() {WrongDistanceCount = wrongDistanceCount};
            energyData.MinimumDistanceConstraintData = constraintData;
            energyData.Energy += wrongDistanceCount;

			return wrongDistanceCount == 0;
		}

		/// <inheritdoc />
		public bool UpdateEnergyData(TLayout layout, TNode perturbedNode, TConfiguration oldConfiguration,
			TConfiguration newConfiguration, TNode node, TConfiguration configuration, ref TEnergyData energyData)
		{
			if (mapDescription.GetRoomDescription(node).GetType() == typeof(CorridorRoomDescription) || mapDescription.GetRoomDescription(perturbedNode).GetType() == typeof(CorridorRoomDescription))
				return true;

			var wrongDistanceOld = 0;
			var wrongDistanceNew = 0;

			if (!AreNeighbours(perturbedNode, node))
			{
				wrongDistanceOld += DoTouch(configuration, oldConfiguration) ? 1 : 0;
				wrongDistanceNew += DoTouch(configuration, newConfiguration) ? 1 : 0;
			}

			var wrongDistanceTotal = configuration.EnergyData.MinimumDistanceConstraintData.WrongDistanceCount + (wrongDistanceNew - wrongDistanceOld);

            var constraintData = new MinimumDistanceConstraintData() {WrongDistanceCount = wrongDistanceTotal};
            energyData.MinimumDistanceConstraintData = constraintData;
			energyData.Energy += wrongDistanceTotal;

			return wrongDistanceTotal == 0;
		}

		/// <inheritdoc />
		public bool UpdateEnergyData(TLayout oldLayout, TLayout newLayout, TNode node, ref TEnergyData energyData)
		{
			if (mapDescription.GetRoomDescription(node).GetType() == typeof(CorridorRoomDescription))
				return true;

			oldLayout.GetConfiguration(node, out var oldConfiguration);
			var wrongDistanceNew = oldConfiguration.EnergyData.MinimumDistanceConstraintData.WrongDistanceCount;

			foreach (var vertex in oldLayout.Graph.Vertices)
			{
				if (vertex.Equals(node))
					continue;

				if (!oldLayout.GetConfiguration(vertex, out var nodeConfiguration))
					continue;

				newLayout.GetConfiguration(vertex, out var newNodeConfiguration);

				wrongDistanceNew += newNodeConfiguration.EnergyData.MinimumDistanceConstraintData.WrongDistanceCount - nodeConfiguration.EnergyData.MinimumDistanceConstraintData.WrongDistanceCount;
			}

            var constraintData = new MinimumDistanceConstraintData() {WrongDistanceCount = wrongDistanceNew};
            energyData.MinimumDistanceConstraintData = constraintData;
            energyData.Energy += wrongDistanceNew;

			return wrongDistanceNew == 0;
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