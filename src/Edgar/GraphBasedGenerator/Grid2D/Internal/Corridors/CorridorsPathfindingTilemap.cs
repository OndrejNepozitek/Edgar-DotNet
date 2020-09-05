using System.Collections.Generic;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common.Configurations;

namespace Edgar.GraphBasedGenerator.Grid2D.Internal.Corridors
{
    public class CorridorsPathfindingTilemap<TRoom, TConfiguration> : ITilemap<Vector2Int, TRoom>
        where TConfiguration : IConfiguration<RoomTemplateInstanceGrid2D, Vector2Int, TRoom>, new()
    {
        private readonly ITilemap<Vector2Int, TRoom> tilemap = new HashSetTilemap<Vector2Int,TRoom>();
        private readonly RectangleGrid2D corridorShape;
        private readonly int minimumRoomDistance;

        public CorridorsPathfindingTilemap(RectangleGrid2D corridorShape, int minimumRoomDistance)
        {
            this.corridorShape = corridorShape;
            this.minimumRoomDistance = minimumRoomDistance;
        }

        public bool IsEmpty(Vector2Int point)
        {
            return tilemap.IsEmpty(point);
        }

        public IEnumerable<Vector2Int> GetPoints()
        {
            return tilemap.GetPoints();
        }

        public void AddPoint(Vector2Int point, TRoom room)
        {
            tilemap.AddPoint(point, room);
        }

        public List<TRoom> GetRoomsOnTile(Vector2Int point)
        {
            return tilemap.GetRoomsOnTile(point);
        }

        public void AddRoom(TConfiguration configuration)
        {
            var outline = configuration.RoomShape.RoomShape + configuration.Position;

            foreach (var line in outline.GetLines())
            {
                AddLineToTilemap(configuration.Room, line);
            }
        }

        private void AddLineToTilemap(TRoom room, OrthogonalLineGrid2D line)
        {
            var points = new List<Vector2Int>();

            if (line.GetDirection() == OrthogonalLineGrid2D.Direction.Top)
            {
                var offsetDirection = new Vector2Int(-1, -1);

                var from = line.From + offsetDirection.ElementWiseProduct(corridorShape.B) + (minimumRoomDistance - 1) * offsetDirection;
                var to = line.To + new Vector2Int(0, minimumRoomDistance - 1);

                points.AddRange(GetRectanglePoints(from, to));
            } 
            else if (line.GetDirection() == OrthogonalLineGrid2D.Direction.Bottom)
            {
                {
                    var offsetDirection = new Vector2Int(-1, -1);

                    var from = line.To + offsetDirection.ElementWiseProduct(corridorShape.B) + (minimumRoomDistance - 1) * offsetDirection;
                    var to = line.From + new Vector2Int(minimumRoomDistance - 1, minimumRoomDistance - 1);

                    points.AddRange(GetRectanglePoints(from, to));
                }

                {
                    var from = line.To + new Vector2Int(-1, 0);
                    var to = line.From + new Vector2Int(-1, -1);

                    points.AddRange(GetRectanglePoints(from, to));
                }

            }
            else if (line.GetDirection() == OrthogonalLineGrid2D.Direction.Right)
            {
                if (minimumRoomDistance > 0)
                {
                    var from = line.From + new Vector2Int(0, -1);
                    var to = line.To + new Vector2Int(0, minimumRoomDistance - 1);

                    points.AddRange(GetRectanglePoints(from, to));
                }

                {
                    var from = line.From + new Vector2Int(0, -1);
                    var to = line.To + new Vector2Int(-1, -1);

                    points.AddRange(GetRectanglePoints(from, to));
                }
            }
            else if (line.GetDirection() == OrthogonalLineGrid2D.Direction.Left)
            {
                var from = line.To + new Vector2Int(0, - corridorShape.Height - minimumRoomDistance + 1);
                var to = line.From + new Vector2Int(-1, 0);

                points.AddRange(GetRectanglePoints(from, to));
            }


            var set = new HashSet<Vector2Int>(points);

            foreach (var point in set)
            {
                tilemap.AddPoint(point, room);
            }

            List<Vector2Int> GetRectanglePoints(Vector2Int from, Vector2Int to)
            {
                var rectanglePoints = new List<Vector2Int>();

                for (int i = 0; i <= to.X - from.X; i++)
                {
                    for (int j = 0; j <=  to.Y - from.Y; j++)
                    {
                        rectanglePoints.Add(from + new Vector2Int(i, j));
                    }
                }

                return rectanglePoints;
            }
        }
    }
}