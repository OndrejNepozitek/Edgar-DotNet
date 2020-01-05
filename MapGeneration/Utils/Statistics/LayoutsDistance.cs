using System.Collections.Generic;
using System.Linq;
using GeneralAlgorithms.DataStructures.Common;
using MapGeneration.Interfaces.Core.MapDescriptions;
using MapGeneration.Interfaces.Core.MapLayouts;

namespace MapGeneration.Utils.Statistics
{
    public static class LayoutsDistance
    {
        public static double PositionOnlyDistance<TNode>(IMapLayout<TNode> layout1, IMapLayout<TNode> layout2)
        {
            return PositionAndShapeDistance(layout1, layout2, 0);
        }

        public static double PositionAndShapeDistance<TNode>(IMapLayout<TNode> layout1, IMapLayout<TNode> layout2, double differentShapePenalty)
        {
            var nodeToRoom1 = layout1.Rooms.ToDictionary(x => x.Node, x => x.Shape + x.Position);
            var nodeToRoom2 = layout2.Rooms.ToDictionary(x => x.Node, x => x.Shape + x.Position);
            var distances = new List<double>();

            var minX1 = nodeToRoom1.Values.Min(x => x.BoundingRectangle.A.X);
            var minY1 = nodeToRoom1.Values.Min(x => x.BoundingRectangle.A.Y);
            var minX2 = nodeToRoom2.Values.Min(x => x.BoundingRectangle.A.X);
            var minY2 = nodeToRoom2.Values.Min(x => x.BoundingRectangle.A.Y);

            foreach (var node in nodeToRoom1.Keys)
            {
                var shape1Center = nodeToRoom1[node].BoundingRectangle.Center - new IntVector2(minX1, minY1);
                var shape2Center = nodeToRoom2[node].BoundingRectangle.Center - new IntVector2(minX2, minY2);

                distances.Add(IntVector2.ManhattanDistance(shape1Center, shape2Center) + (nodeToRoom1[node].Equals(nodeToRoom2[node]) ? 0 : differentShapePenalty));
            }

            return distances.Average();
        }

        public static double GetAverageRoomTemplateSize<TNode>(IMapDescription<TNode> mapDescription)
        {
            var roomTemplates = mapDescription
                .GetGraph()
                .Vertices
                .SelectMany(x => mapDescription.GetRoomDescription(x).RoomTemplates)
                .Distinct()
                .ToList();

            var averageSize = roomTemplates
                .Select(x => x.Shape.BoundingRectangle.Width + x.Shape.BoundingRectangle.Height)
                .Average();

            return averageSize;
        }
    }
}