using Edgar.GraphBasedGenerator.Common.Configurations;
using Edgar.GraphBasedGenerator.Common.RoomShapeGeometry;
using Edgar.Graphs;
using Edgar.Legacy.Core.Constraints.Interfaces;
using Edgar.Legacy.Core.Layouts.Interfaces;

namespace Edgar.GraphBasedGenerator.Common.Constraints.MinimumDistanceConstraint
{
    public class MinimumDistanceConstraint<TNode, TConfiguration, TEnergyData> : INodeConstraint<
        ILayout<TNode, TConfiguration>, TNode, TConfiguration, TEnergyData>
        where TConfiguration : IEnergyConfiguration<TEnergyData>
        where TEnergyData : IMinimumDistanceConstraintData
    {
        private readonly ILevelDescription<TNode> mapDescription;
        private readonly IRoomShapeGeometry<TConfiguration> roomShapeGeometry;
        private readonly IGraph<TNode> graph;
        private readonly int minimumDistance;

        public MinimumDistanceConstraint(ILevelDescription<TNode> mapDescription,
            IRoomShapeGeometry<TConfiguration> roomShapeGeometry, int minimumDistance)
        {
            this.mapDescription = mapDescription;
            this.roomShapeGeometry = roomShapeGeometry;
            this.minimumDistance = minimumDistance;
            graph = mapDescription.GetGraph();
        }

        /// <inheritdoc />
        public bool ComputeEnergyData(ILayout<TNode, TConfiguration> layout, TNode node, TConfiguration configuration,
            ref TEnergyData energyData)
        {
            // TODO: why this?
            if (mapDescription.GetRoomDescription(node).IsCorridor)
                return true;

            var wrongDistanceCount = 0;

            foreach (var vertex in layout.Graph.Vertices)
            {
                if (vertex.Equals(node))
                    continue;

                if (!layout.GetConfiguration(vertex, out var c))
                    continue;

                if (mapDescription.GetRoomDescription(vertex).IsCorridor)
                    continue;

                if (AreNeighbours(node, vertex))
                    continue;

                if (!HaveMinimumDistance(configuration, c))
                {
                    wrongDistanceCount++;
                }
            }

            var constraintData = new MinimumDistanceConstraintData() {WrongDistanceCount = wrongDistanceCount};
            energyData.MinimumDistanceConstraintData = constraintData;

            return wrongDistanceCount == 0;
        }

        /// <inheritdoc />
        public bool UpdateEnergyData(ILayout<TNode, TConfiguration> layout, TNode perturbedNode,
            TConfiguration oldConfiguration,
            TConfiguration newConfiguration, TNode node, TConfiguration configuration, ref TEnergyData energyData)
        {
            if (mapDescription.GetRoomDescription(node).IsCorridor ||
                mapDescription.GetRoomDescription(perturbedNode).IsCorridor)
                return true;

            var wrongDistanceOld = 0;
            var wrongDistanceNew = 0;

            if (!AreNeighbours(perturbedNode, node))
            {
                wrongDistanceOld += !HaveMinimumDistance(configuration, oldConfiguration) ? 1 : 0;
                wrongDistanceNew += !HaveMinimumDistance(configuration, newConfiguration) ? 1 : 0;
            }

            var wrongDistanceTotal = configuration.EnergyData.MinimumDistanceConstraintData.WrongDistanceCount +
                                     (wrongDistanceNew - wrongDistanceOld);

            var constraintData = new MinimumDistanceConstraintData() {WrongDistanceCount = wrongDistanceTotal};
            energyData.MinimumDistanceConstraintData = constraintData;

            return wrongDistanceTotal == 0;
        }

        /// <inheritdoc />
        public bool UpdateEnergyData(ILayout<TNode, TConfiguration> oldLayout, ILayout<TNode, TConfiguration> newLayout,
            TNode node, ref TEnergyData energyData)
        {
            if (mapDescription.GetRoomDescription(node).IsCorridor)
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

                wrongDistanceNew += newNodeConfiguration.EnergyData.MinimumDistanceConstraintData.WrongDistanceCount -
                                    nodeConfiguration.EnergyData.MinimumDistanceConstraintData.WrongDistanceCount;
            }

            var constraintData = new MinimumDistanceConstraintData() {WrongDistanceCount = wrongDistanceNew};
            energyData.MinimumDistanceConstraintData = constraintData;

            return wrongDistanceNew == 0;
        }

        private bool HaveMinimumDistance(TConfiguration configuration1, TConfiguration configuration2)
        {
            return roomShapeGeometry.DoHaveMinimumDistance(configuration1, configuration2, minimumDistance);
        }

        private bool AreNeighbours(TNode node1, TNode node2)
        {
            return graph.HasEdge(node1, node2);
        }
    }
}