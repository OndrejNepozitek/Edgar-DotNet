using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common;
using Edgar.GraphBasedGenerator.Common.Configurations;
using Edgar.GraphBasedGenerator.Common.Corridors;
using Edgar.GraphBasedGenerator.Grid2D.Drawing;
using Edgar.Graphs;
using Edgar.Legacy.Core.Layouts.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.Utils.Interfaces;
using Priority_Queue;

namespace Edgar.GraphBasedGenerator.Grid2D.Internal.Corridors
{
    public class PathfindingCorridorsHandlerGrid2DOld<TLayout, TRoom, TConfiguration> : ICorridorsHandler<TLayout, TRoom>
        where TLayout : ILayout<TRoom, TConfiguration>, ISmartCloneable<TLayout> // TODO: is this necessary?
        where TConfiguration : IConfiguration<RoomTemplateInstanceGrid2D, Vector2Int, TRoom>, new()
    {
        private readonly ILevelDescription<TRoom> levelDescription;
        private readonly IGraph<TRoom> graph;
        private int aliasCounter;
        private readonly Random random = new Random(0);

        private ITilemap<Vector2Int> tilemap;

        public PathfindingCorridorsHandlerGrid2DOld(ILevelDescription<TRoom> levelDescription, int aliasCounter)
        {
            this.levelDescription = levelDescription;
            this.aliasCounter = aliasCounter;
            graph = levelDescription.GetGraph();
        }

        public bool AddCorridors(TLayout layout, IEnumerable<TRoom> chain)
        {
            var clone = layout.SmartClone();
            var corridors = chain.Where(x => levelDescription.GetRoomDescription(x).IsCorridor).ToList();
            tilemap = GetTilemap(layout);

            if (AddCorridors(clone, corridors))
            {
                foreach (var corridor in corridors)
                {
                    clone.GetConfiguration(corridor, out var configuration);
                    layout.SetConfiguration(corridor, configuration);
                }

                // Console.WriteLine("Success");
                return true;
            }

            // Console.WriteLine("Fail");
            return false;
        }

        private bool AddCorridors(TLayout layout, List<TRoom> corridorRooms)
        {
            foreach (var corridorRoom in corridorRooms)
            {
                if (!AddCorridor(layout, corridorRoom))
                {
                    return false;
                }
            }

            return true;
        }

        private bool AddCorridor(TLayout layout, TRoom room)
        {
            var neighbors = graph.GetNeighbours(room).ToList();
            var from = neighbors[0];
            var to = neighbors[1];

            layout.GetConfiguration(from, out var fromConfiguration);
            layout.GetConfiguration(to, out var toConfiguration);

            var fromDoors = fromConfiguration.RoomShape.DoorLines;
            var toDoors = toConfiguration.RoomShape.DoorLines;

            if (fromDoors.Any(x => x.Length != 1) || toDoors.Any(x => x.Length != 1))
            {
                throw new ArgumentException("Only door length 1 is currently supported");
            }

            if (fromDoors.Any(x => x.DoorSocket != null) || toDoors.Any(x => x.DoorSocket != null))
            {
                throw new ArgumentException("Door sockets are not supported");
            }

            var roomTemplate = FindPath(fromDoors.Select(x => x.Line + fromConfiguration.Position).ToList(), toDoors.Select(x => x.Line + toConfiguration.Position).ToList(), room);



            if (roomTemplate != null)
            {
                // TODO: should probably be moved so that the outline starts at (0,0)
                var configuration = new TConfiguration()
                {
                    Position = new Vector2Int(),
                    //Position = -1 * offset,
                    Room =  room, 
                    RoomShape = new RoomTemplateInstanceGrid2D(roomTemplate, roomTemplate.Outline, roomTemplate.Doors.GetDoors(roomTemplate.Outline), new List<TransformationGrid2D>() { TransformationGrid2D.Identity })
                    {
                        RoomShapeAlias = new IntAlias<PolygonGrid2D>(aliasCounter++, roomTemplate.Outline)
                    }
                };

                layout.SetConfiguration(room, configuration);

                AddPolygonToTilemap(tilemap, roomTemplate.Outline + configuration.Position);

                return true;
            }

            return false;
        }

        private RoomTemplateGrid2D FindPath(List<OrthogonalLineGrid2D> from, List<OrthogonalLineGrid2D> to, TRoom room)
        {
            var queue = new SimplePriorityQueue<Vector2Int>();
            var backEdges = new Dictionary<Vector2Int?, Vector2Int?>();

            var start = from.SelectMany(x => GetModifiedLine(x).GetPoints()).ToList();
            var goals = to.SelectMany(x => GetModifiedLine(x).GetPoints()).ToList();

            var fromDoorMapping = from.SelectMany(x => GetModifiedLine(x).GetPoints().Select(y => (x, y))).ToDictionary(x => x.y, x => x.x);
            var toDoorMapping = to.SelectMany(x => GetModifiedLine(x).GetPoints().Select(y => (x, y))).ToDictionary(x => x.y, x => x.x);

            foreach (var point in start)
            {
                // TODO: slow
                queue.EnqueueWithoutDuplicates(point, 1);
                backEdges.Add(point, null);
            }

            while (queue.Count != 0)
            {
                var item = queue.Dequeue();

                foreach (var neighbor in item.GetAdjacentVectors())
                {
                    if (goals.Contains(neighbor))
                    {
                        backEdges.Add(neighbor, item);
                        var path = GetPath(neighbor, backEdges);

                        // // Console.WriteLine($"Room: {room}, path: {string.Join(", ", path)}");
                        var polygon = GetPolygonFromPath(path, fromDoorMapping[path[0]], toDoorMapping[path.Last()]);
                        // Console.WriteLine($"Explored success: {backEdges.Count}");
                        return polygon;
                    }

                    if (!tilemap.IsEmpty(neighbor))
                    {
                    }

                    if (!backEdges.ContainsKey(neighbor) && tilemap.IsEmpty(neighbor))
                    {
                        backEdges.Add(neighbor, item);

                        //if (backEdges.Count < 100)
                        //{
                        //    queue.EnqueueWithoutDuplicates(neighbor, random.Next(1, 100));
                        //}
                        //else
                        {
                            queue.EnqueueWithoutDuplicates(neighbor, 100);
                        }
                    }
                }

                if (backEdges.Count > 1000)
                {
                    break;
                }
            }

            // Console.WriteLine($"Explored fail: {backEdges.Count}");

            return null;
        }

        private RoomTemplateGrid2D GetPolygonFromPath(List<Vector2Int> path, OrthogonalLineGrid2D fromLine, OrthogonalLineGrid2D toLine)
        {
            var points = new HashSet<Vector2Int>();

            foreach (var point in path)
            {
                points.Add(point);
                points.Add(point + new Vector2Int(1, 0));
                points.Add(point + new Vector2Int(1, 1));
                points.Add(point + new Vector2Int(0, 1));
            }

            var polygon = GetPolygonFromTiles(points);
            var startDoor = GetDoor(path[0], path[1], false);
            var endDoor = GetDoor(path[path.Count - 1], path[path.Count - 2], true);

            var modifiedFrom = GetModifiedLine(fromLine);
            var modifiedTo = GetModifiedLine(toLine);

            var fromDiff = modifiedFrom.From - fromLine.From;
            var toDiff = modifiedTo.From - toLine.From;

            var startDoor2 = GetDoor(path[0], fromLine, fromDiff);
            var endDoor2 = GetDoor(path[path.Count - 1], toLine, toDiff);

            var roomTemplate = new RoomTemplateGrid2D(polygon, new ManualDoorModeGrid2D(new List<DoorGrid2D>()
            {
                startDoor2,
                endDoor2,
            }));

            if (polygon.GetPoints().Count == 80)
            {
                var roomTemplatesDrawer = new RoomTemplateDrawer();
                var bitmap = roomTemplatesDrawer.DrawRoomTemplates(new List<RoomTemplateGrid2D>() {roomTemplate},
                    new DungeonDrawerOptions()
                    {
                        Width = 2000,
                        Height = 2000,
                    });
                bitmap.Save("pathfinding.png");
            }



            return roomTemplate;
        }

        private DoorGrid2D GetDoor(Vector2Int from, OrthogonalLineGrid2D line, Vector2Int diff)
        {
            from += -1 * diff;
            var rotated = line.GetDirectionVector().Y != 0 ? new Vector2Int(0, 1) : new Vector2Int(1, 0);
            
            var to = from + line.GetDirectionVector();

            return new DoorGrid2D(from, to);
        }

        private DoorGrid2D GetDoor(Vector2Int from, Vector2Int next, bool flip)
        {
            var direction = next - from;
            var rotated = direction.X != 0 ? new Vector2Int(0, 1) : new Vector2Int(1, 0);

            if (direction.X == -1)
            {
                from += new Vector2Int(1, 0);
            }

            if (direction.Y == -1)
            {
                from += new Vector2Int(0, 1);
            }

            var to = from + rotated;

            return new DoorGrid2D(from, to);
        }

        private static PolygonGrid2D GetPolygonFromTiles(HashSet<Vector2Int> allPoints)
        {
            if (allPoints.Count == 0)
            {
                throw new ArgumentException("There must be at least one point");
            }

            var orderedDirections = new Dictionary<Vector2Int, List<Vector2Int>>
            {
                {Vector2IntHelper.Top, new List<Vector2Int> {Vector2IntHelper.Left, Vector2IntHelper.Top, Vector2IntHelper.Right}},
                {Vector2IntHelper.Right, new List<Vector2Int> {Vector2IntHelper.Top, Vector2IntHelper.Right, Vector2IntHelper.Bottom}},
                {Vector2IntHelper.Bottom, new List<Vector2Int> {Vector2IntHelper.Right, Vector2IntHelper.Bottom, Vector2IntHelper.Left}},
                {Vector2IntHelper.Left, new List<Vector2Int> {Vector2IntHelper.Bottom, Vector2IntHelper.Left, Vector2IntHelper.Top}}
            };

            var allPointsInternal = allPoints;
            var smallestX = allPointsInternal.Min(x => x.X);
            var smallestXPoints = allPointsInternal.Where(x => x.X == smallestX).ToList();
            var smallestXYPoint = smallestXPoints[smallestXPoints.MinBy(x => x.Y)];

            var startingPoint = smallestXYPoint;
            var startingDirection = Vector2IntHelper.Top;

            var polygonPoints = new List<Vector2Int>();
            var currentPoint = startingPoint + startingDirection;
            var firstPoint = currentPoint;
            var previousDirection = startingDirection;
            var first = true;

            if (!allPointsInternal.Contains(currentPoint))
            {
                throw new ArgumentException("Invalid room shape.");
            }

            while (true)
            {
                var foundNeighbor = false;
                var currentDirection = new Vector2Int();

                foreach (var directionVector in orderedDirections[previousDirection])
                {
                    var newPoint = currentPoint + directionVector;

                    if (allPointsInternal.Contains(newPoint))
                    {
                        currentDirection = directionVector;
                        foundNeighbor = true;
                        break;
                    }
                }

                if (!foundNeighbor)
                    throw new ArgumentException("Invalid room shape.");

                if (currentDirection != previousDirection)
                {
                    polygonPoints.Add(currentPoint);
                }

                currentPoint += currentDirection;
                previousDirection = currentDirection;

                if (first)
                {
                    first = false;
                }
                else if (currentPoint == firstPoint)
                {
                    break;
                }
            }

            if (!IsClockwiseOriented(polygonPoints))
            {
                polygonPoints.Reverse();
            }

            return new PolygonGrid2D(polygonPoints);
        }

        private static bool IsClockwiseOriented(IList<Vector2Int> points)
        {
            var previous = points[points.Count - 1];
            var sum = 0L;

            foreach (var point in points)
            {
                sum += (point.X - previous.X) * (long) (point.Y + previous.Y);
                previous = point;
            }

            return sum > 0;
        }

        private OrthogonalLineGrid2D GetModifiedLine(OrthogonalLineGrid2D line)
        {
            switch (line.GetDirection())
            {
                case OrthogonalLineGrid2D.Direction.Bottom:
                    return line + new Vector2Int(0, -1);

                case OrthogonalLineGrid2D.Direction.Left:
                    return line + new Vector2Int(-1, -1);

                case OrthogonalLineGrid2D.Direction.Top:
                    return line + new Vector2Int(-1, 0);

                case OrthogonalLineGrid2D.Direction.Right:
                    return line;

                default:
                    throw new ArgumentException();
            }
        }

        private List<Vector2Int> GetPath(Vector2Int goal, Dictionary<Vector2Int?, Vector2Int?> backEdges)
        {
            var path = new List<Vector2Int>();
            var current = goal;

            while (true)
            {
                path.Add(current);

                var previous = backEdges[current];

                if (previous == null)
                {
                    break;
                }
                else
                {
                    current = previous.Value;
                }
            }

            path.Reverse();

            return path;
        }

        private ITilemap<Vector2Int> GetTilemap(TLayout layout)
        {
            throw new NotImplementedException();
            //var tilemap = new HashSetTilemap<Vector2Int>();

            //foreach (var configuration in layout.GetAllConfigurations())
            //{
            //    var outline = configuration.RoomShape.RoomShape + configuration.Position;

            //    AddPolygonToTilemap(tilemap, outline);
            //}

            //return tilemap;
        }

        private void AddPolygonToTilemap(ITilemap<Vector2Int> tilemap, PolygonGrid2D polygon)
        {
            foreach (var line in polygon.GetLines())
            {
                AddPointsToTilemap(tilemap, line.GetPoints());

                var doubleWall = line + line.GetDirectionVector().RotateAroundCenter(90);
                AddPointsToTilemap(tilemap, doubleWall.GetPoints());
            }
        }

        private void AddPointsToTilemap(ITilemap<Vector2Int> tilemap, IEnumerable<Vector2Int> points)
        {
            var list = new List<Vector2Int>();

            foreach (var point in points)
            {
                list.Add(point);
                list.Add(point + new Vector2Int(-1, 0));
                list.Add(point + new Vector2Int(-1, -1));
                list.Add(point + new Vector2Int(0, -1));
            }

            foreach (var point in list)
            {
                throw new NotImplementedException();
                // tilemap.AddPoint(point);
            }

            // // Console.WriteLine($"Add points: {string.Join(", ", list.Select(x => x.ToStringShort()))}");
        }
    }
}