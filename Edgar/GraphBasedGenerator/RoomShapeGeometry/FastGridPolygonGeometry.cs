using Edgar.GraphBasedGenerator.Configurations;
using GeneralAlgorithms.Algorithms.Polygons;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.Configurations.Interfaces;
using MapGeneration.Core.MapDescriptions;

namespace Edgar.GraphBasedGenerator.RoomShapeGeometry
{
    public class FastGridPolygonGeometry<TConfiguration, TNode> : IRoomShapeGeometry<TConfiguration>
        where TConfiguration : IShapeConfiguration<RoomTemplateInstance>, IPositionConfiguration<IntVector2>
    {
        private readonly FastPolygonOverlap polygonOverlap = new FastPolygonOverlap();

        public int GetOverlapArea(TConfiguration configuration1, TConfiguration configuration2)
        {
            return polygonOverlap.OverlapArea(
                configuration1.RoomShape.RoomShapeAlias, configuration1.Position,
                configuration2.RoomShape.RoomShapeAlias, configuration2.Position
            );
        }

        public bool DoHaveMinimumDistance(TConfiguration configuration1, TConfiguration configuration2, int minimumDistance)
        {
            return polygonOverlap.DoHaveMinimumDistance(
                configuration1.RoomShape.RoomShapeAlias, configuration1.Position,
                configuration2.RoomShape.RoomShapeAlias, configuration2.Position,
                minimumDistance
            );
        }

        public int GetCenterDistance(TConfiguration configuration1, TConfiguration configuration2)
        {
            return IntVector2.ManhattanDistance(configuration1.RoomShape.RoomShape.BoundingRectangle.Center + configuration1.Position,
                configuration2.RoomShape.RoomShape.BoundingRectangle.Center + configuration2.Position);
        }
    }
}