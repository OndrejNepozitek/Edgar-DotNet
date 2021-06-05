using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common;
using Edgar.GraphBasedGenerator.Common.ChainDecomposition;
using Edgar.GraphBasedGenerator.Common.ChainDecomposition.Legacy;
using Edgar.GraphBasedGenerator.Common.Configurations;
using Edgar.GraphBasedGenerator.Common.ConfigurationSpaces;
using Edgar.GraphBasedGenerator.Common.Constraints;
using Edgar.GraphBasedGenerator.Common.Constraints.BasicConstraint;
using Edgar.GraphBasedGenerator.Common.Constraints.CorridorConstraint;
using Edgar.GraphBasedGenerator.Common.Constraints.FixedConfigurationConstraint;
using Edgar.GraphBasedGenerator.Common.Constraints.MinimumDistanceConstraint;
using Edgar.GraphBasedGenerator.Common.RoomShapeGeometry;
using Edgar.Legacy.Core.ChainDecompositions;
using Edgar.Legacy.Core.ConfigurationSpaces;
using Edgar.Legacy.Core.Constraints.Interfaces;
using Edgar.Legacy.Core.Layouts.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace Edgar.GraphBasedGenerator.Grid2D.Internal
{
    public static class GraphBasedGeneratorGrid2DUtils
    {
        public static IChainDecomposition<TRoom> GetChainDecomposition<TRoom>(
            ILevelDescription<TRoom> levelDescription,
            IFixedConfigurationConstraint<RoomTemplateInstanceGrid2D, Vector2Int, TRoom> fixedConfigurationConstraint,
            ChainDecompositionConfiguration chainDecompositionConfiguration)
        {
            var fixedPositionRooms = levelDescription
                .GetGraph()
                .Vertices
                .Where(fixedConfigurationConstraint.IsFixedPosition)
                .ToList();
            var fixedRooms = fixedPositionRooms
                .Where(fixedConfigurationConstraint.IsFixedShape)
                .ToList();

            // Prepare chain decomposition algorithm
            chainDecompositionConfiguration = chainDecompositionConfiguration ?? new ChainDecompositionConfiguration();
            var chainDecomposition = new FixedRoomsChainDecomposition<TRoom>(chainDecompositionConfiguration, fixedRooms: fixedPositionRooms);
            var twoStageChainDecomposition = new Common.ChainDecomposition.TwoStageChainDecomposition<TRoom>(
                levelDescription,
                chainDecomposition
            );
            var fixedRoomsChainDecomposition = new FixedRoomsChainDecompositionPreprocessing<TRoom>(fixedRooms, twoStageChainDecomposition);

            return fixedRoomsChainDecomposition;
        }

        public static Dictionary<TRoom, List<WeightedShape>> GetLegacyShapesForNodes<TRoom>(
            ILevelDescription<TRoom> levelDescription,
            LevelGeometryData<TRoom> geometryData
            )
        {
            var shapesForNodes = new Dictionary<TRoom, List<WeightedShape>>();
            foreach (var vertex in levelDescription.GetGraph().Vertices)
            {
                shapesForNodes.Add(vertex, new List<WeightedShape>());
                var roomDescription = geometryData.RoomDescriptions[vertex];

                foreach (var roomTemplate in roomDescription.RoomTemplates)
                {
                    var roomTemplateInstances = geometryData.RoomTemplateInstances[roomTemplate];

                    foreach (var roomTemplateInstance in roomTemplateInstances)
                    {
                        shapesForNodes[vertex].Add(new WeightedShape(
                            geometryData.RoomTemplateInstanceToPolygonMapping[roomTemplateInstance],
                            1d / roomTemplateInstances.Count)
                        );
                    }
                }
            }

            return shapesForNodes;
        }

        public static int GetLegacyAverageSize<TRoom>(
            ILevelDescription<TRoom> levelDescription,
            Dictionary<TRoom, List<WeightedShape>> shapesForNodes
            )
        {
            var usedShapes = new HashSet<int>();
            var allShapes = new List<IntAlias<PolygonGrid2D>>();
            foreach (var vertex in levelDescription.GetGraph().Vertices)
            {
                var shapes = shapesForNodes[vertex];

                foreach (var shape in shapes)
                {
                    if (!usedShapes.Contains(shape.Shape.Alias))
                    {
                        allShapes.Add(shape.Shape);
                        usedShapes.Add(shape.Shape.Alias);
                    }
                }
            }

            var averageSize = (int) allShapes
                    .Select(x => x.Value.BoundingRectangle)
                    .Average(x => (x.Width + x.Height) / 2);

            return averageSize;
        }

        public static ConstraintsEvaluator<RoomNode<TRoom>, ConfigurationGrid2D<TRoom, EnergyData>, EnergyData> GetConstraintsEvaluator<TRoom>(
            ILevelDescription<RoomNode<TRoom>> levelDescription,
            IRoomShapeGeometry<ConfigurationGrid2D<TRoom, EnergyData>> roomShapeGeometry,
            IConfigurationSpaces<ConfigurationGrid2D<TRoom, EnergyData>> configurationSpaces,
            int averageRoomSize,
            int minimumRoomDistance,
            bool optimizeCorridorConstraints
            )
        {
            var energyUpdater = new BasicEnergyUpdater<RoomNode<TRoom>, ConfigurationGrid2D<TRoom, EnergyData>>(10 * averageRoomSize);

            // Create generator constraints
            var stageOneConstraints =
                new List<INodeConstraint<ILayout<RoomNode<TRoom>, ConfigurationGrid2D<TRoom, EnergyData>>, RoomNode<TRoom>, ConfigurationGrid2D<TRoom, EnergyData>,
                    EnergyData>>
                {
                    new BasicConstraint<RoomNode<TRoom>, ConfigurationGrid2D<TRoom, EnergyData>, EnergyData>(
                        roomShapeGeometry,
                        configurationSpaces,
                        levelDescription,
                        optimizeCorridorConstraints
                    ),
                    new CorridorConstraint<RoomNode<TRoom>, ConfigurationGrid2D<TRoom, EnergyData>, EnergyData>(
                        levelDescription,
                        configurationSpaces,
                        roomShapeGeometry
                    ),
                };

            if (minimumRoomDistance > 0)
            {
                stageOneConstraints.Add(new MinimumDistanceConstraint<RoomNode<TRoom>, ConfigurationGrid2D<TRoom, EnergyData>, EnergyData>(
                    levelDescription,
                    roomShapeGeometry,
                    minimumRoomDistance
                ));
            }

            var constraintsEvaluator = new ConstraintsEvaluator<RoomNode<TRoom>, ConfigurationGrid2D<TRoom, EnergyData>, EnergyData>(stageOneConstraints, energyUpdater);

            return constraintsEvaluator;
        }
    }
}