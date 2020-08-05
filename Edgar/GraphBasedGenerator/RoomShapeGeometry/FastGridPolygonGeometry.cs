using GeneralAlgorithms.Algorithms.Polygons;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.Configurations.Interfaces;

namespace Edgar.GraphBasedGenerator.RoomShapeGeometry
{
    public class FastGridPolygonGeometry<TConfiguration, TNode> : IRoomShapeGeometry<TConfiguration>
        where TConfiguration : IConfiguration<IntAlias<GridPolygon>, TNode>
    {
        private readonly FastPolygonOverlap polygonOverlap = new FastPolygonOverlap();

        public int GetOverlapArea(TConfiguration configuration1, TConfiguration configuration2)
        {
            return polygonOverlap.OverlapArea(
                configuration1.ShapeContainer, configuration1.Position,
                configuration2.ShapeContainer, configuration2.Position
            );
        }

        public bool DoHaveMinimumDistance(TConfiguration configuration1, TConfiguration configuration2, int minimumDistance)
        {
            return polygonOverlap.DoHaveMinimumDistance(
                configuration1.ShapeContainer, configuration1.Position,
                configuration2.ShapeContainer, configuration2.Position,
                minimumDistance
            );
        }

        public int GetCenterDistance(TConfiguration configuration1, TConfiguration configuration2)
        {
            return IntVector2.ManhattanDistance(configuration1.Shape.BoundingRectangle.Center + configuration1.Position,
                configuration2.Shape.BoundingRectangle.Center + configuration2.Position);
        }
    }
}