using Edgar.GraphBasedGenerator.General.Configurations;
using Edgar.GraphBasedGenerator.General.RoomShapeGeometry;
using GeneralAlgorithms.Algorithms.Polygons;
using GeneralAlgorithms.DataStructures.Common;
using MapGeneration.Core.MapDescriptions;

namespace Edgar.GraphBasedGenerator.Grid2D
{
    public class FastGridPolygonGeometry<TConfiguration, TNode> : IRoomShapeGeometry<TConfiguration>
        where TConfiguration : IShapeConfiguration<RoomTemplateInstance>, IPositionConfiguration<Vector2Int>
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
            return Vector2Int.ManhattanDistance(configuration1.RoomShape.RoomShape.BoundingRectangle.Center + configuration1.Position,
                configuration2.RoomShape.RoomShape.BoundingRectangle.Center + configuration2.Position);
        }
    }
}