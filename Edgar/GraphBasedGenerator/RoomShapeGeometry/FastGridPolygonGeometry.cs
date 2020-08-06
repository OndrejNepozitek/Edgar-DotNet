using Edgar.GraphBasedGenerator.Configurations;
using GeneralAlgorithms.Algorithms.Polygons;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.Configurations.Interfaces;

namespace Edgar.GraphBasedGenerator.RoomShapeGeometry
{
    public class FastGridPolygonGeometry<TConfiguration, TNode> : IRoomShapeGeometry<TConfiguration>
        where TConfiguration : IShapeConfiguration<IntAlias<GridPolygon>>, IPositionConfiguration<IntVector2>
    {
        private readonly FastPolygonOverlap polygonOverlap = new FastPolygonOverlap();

        public int GetOverlapArea(TConfiguration configuration1, TConfiguration configuration2)
        {
            return polygonOverlap.OverlapArea(
                configuration1.RoomShape, configuration1.Position,
                configuration2.RoomShape, configuration2.Position
            );
        }

        public bool DoHaveMinimumDistance(TConfiguration configuration1, TConfiguration configuration2, int minimumDistance)
        {
            return polygonOverlap.DoHaveMinimumDistance(
                configuration1.RoomShape, configuration1.Position,
                configuration2.RoomShape, configuration2.Position,
                minimumDistance
            );
        }

        public int GetCenterDistance(TConfiguration configuration1, TConfiguration configuration2)
        {
            return IntVector2.ManhattanDistance(configuration1.RoomShape.Value.BoundingRectangle.Center + configuration1.Position,
                configuration2.RoomShape.Value.BoundingRectangle.Center + configuration2.Position);
        }
    }
}