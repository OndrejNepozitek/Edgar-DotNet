using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common.Configurations;
using Edgar.GraphBasedGenerator.Grid2D.Drawing;
using Edgar.Legacy.Core.Layouts.Interfaces;
using Priority_Queue;

namespace Edgar.GraphBasedGenerator.Grid2D.Internal.Corridors
{
    public class CorridorsPathfinder<TLayout, TRoom, TConfiguration>
        where TLayout : ILayout<TRoom, TConfiguration>
        where TConfiguration : IConfiguration<RoomTemplateInstanceGrid2D, Vector2Int, TRoom>, new()
    {
        private readonly RectangleGrid2D corridorShape;
        private ITilemap<Vector2Int> tilemap;
        private readonly int minimumRoomDistance;

        public CorridorsPathfinder(int corridorWidth, int corridorHeight, int minimumRoomDistance)
        {
            this.corridorShape = new RectangleGrid2D(new Vector2Int(0, 0), new Vector2Int(corridorWidth, corridorHeight));
            this.minimumRoomDistance = minimumRoomDistance;
        }

        public PathfindingResult FindPath(TLayout layout, TRoom fromRoom, TRoom toRoom)
        {
            layout.GetConfiguration(fromRoom, out var fromConfiguration);
            layout.GetConfiguration(toRoom, out var toConfiguration);

            var fromDoors = fromConfiguration.RoomShape.DoorLines;
            var toDoors = toConfiguration.RoomShape.DoorLines;

            //if (fromDoors.Any(x => x.Length != 1) || toDoors.Any(x => x.Length != 1))
            //{
            //    throw new ArgumentException("Only door length 1 is currently supported");
            //}

            if (fromDoors.Any(x => x.DoorSocket != null) || toDoors.Any(x => x.DoorSocket != null))
            {
                throw new ArgumentException("Door sockets are not supported");
            }

            tilemap = GetTilemap(layout);

            var fromDoorMapping = fromDoors
                .Where(IsDoorCorrectLength)
                .Select(x => x.Line + fromConfiguration.Position)
                .SelectMany(x => GetModifiedDoorLine(x).GetPoints().Select(y => (x, y)))
                .ToDictionary(x => x.y, x => x.x);
            var toDoorMapping = toDoors
                .Where(IsDoorCorrectLength)
                .Select(x => x.Line + toConfiguration.Position)
                .SelectMany(x => GetModifiedDoorLine(x).GetPoints().Select(y => (x, y)))
                .ToDictionary(x => x.y, x => x.x);

            var (path, costs) = FindPath(fromDoorMapping.Keys.ToList(), toDoorMapping.Keys.ToList(), fromDoorMapping, toDoorMapping);

            // No path was found
            if (path == null)
            {
                return null;
            }

            var corridor = GetCorridorFromPath(path, fromDoorMapping[path[0]], toDoorMapping[path.Last()]);
            corridor.Costs = costs;

            return corridor;
        }

        private bool IsDoorCorrectLength(DoorLineGrid2D doorLine)
        {
            if (doorLine.Line.GetDirectionVector().X != 0)
            {
                return doorLine.Length == corridorShape.Width;
            }

            return doorLine.Length == corridorShape.Height;
        }

        private (List<Vector2Int> path, Dictionary<Vector2Int, int> costs) FindPath(List<Vector2Int> startPoints, List<Vector2Int> goalPoints, Dictionary<Vector2Int, OrthogonalLineGrid2D> fromDoorMapping, Dictionary<Vector2Int, OrthogonalLineGrid2D> toDoorMapping)
        {
            var queue = new SimplePriorityQueue<Vector2Int>();

            var cameFrom = new Dictionary<Vector2Int?, Vector2Int?>();
            var previousDirection = new Dictionary<Vector2Int, Vector2Int>();
            var costSoFar = new Dictionary<Vector2Int, int>();

            foreach (var point in startPoints)
            {
                // TODO: slow
                queue.EnqueueWithoutDuplicates(point, 0);

                cameFrom.Add(point, null);
                costSoFar.Add(point, 0);
                previousDirection[point] = fromDoorMapping[point].GetDirectionVector().RotateAroundCenter(270);
            }

            while (queue.Count != 0)
            {
                var item = queue.Dequeue();

                if (goalPoints.Contains(item))
                {
                    return (GetPath(item, cameFrom), costSoFar);
                }

                foreach (var neighbor in item.GetAdjacentVectors())
                {
                    if (IsEmpty(neighbor))
                    {
                        var cost = costSoFar[item] + 1;
                        var direction = neighbor - item;

                        // Penalty
                        if (previousDirection[item] != direction)
                        {
                            cost++;
                        }

                        if (goalPoints.Contains(neighbor))
                        {
                            var line = toDoorMapping[neighbor];

                            if (direction != line.GetDirectionVector().RotateAroundCenter(90))
                            {
                                cost++;
                            }
                        }

                        float priority = cost + GetHeuristics(neighbor, goalPoints, direction);

                        if (!cameFrom.ContainsKey(neighbor) ||
                            (queue.Contains(neighbor) && queue.GetPriority(neighbor) > priority))
                        {
                            costSoFar[neighbor] = cost;
                            cameFrom[neighbor] = item;
                            previousDirection[neighbor] = direction;

                            if (queue.Contains(neighbor))
                            {
                                queue.UpdatePriority(neighbor, priority);
                            }
                            else
                            {
                                queue.EnqueueWithoutDuplicates(neighbor, priority);
                            }
                        }
                    }
                }

                // TODO: which constant should we use?
                if (cameFrom.Count > 1000)
                {
                    break;
                }
            }

            return (null, costSoFar);
        }

        private float GetHeuristics(Vector2Int point, List<Vector2Int> goals, Vector2Int previousDirection)
        {
            var nextStep = point + previousDirection;
            return goals.Min(x => Vector2Int.ManhattanDistance(nextStep, x)) - 1;
        }

        private bool IsEmpty(Vector2Int point)
        {
            // TODO: fix
            return tilemap.IsEmpty(point);
        }

        /// <summary>
        /// Gets the path and returns an outline with doors.
        /// </summary>
        /// <remarks>
        /// It may happen that the last point of the path is not on the toDoorLine. In that case, additional points are added until it reaches the door line.
        /// Both given door line should be the actual door line without any modifications.
        /// </remarks>
        private PathfindingResult GetCorridorFromPath(List<Vector2Int> path, OrthogonalLineGrid2D fromDoorLine, OrthogonalLineGrid2D toDoorLine)
        {
            var points = new HashSet<Vector2Int>();

            // If the door line is vertical and last point is already at the correct X position, we want to add only corridor height
            //if (toDoorLine.GetDirectionVector().Y != 0 && path.Last().X == toDoorLine.From.X)
            //{
            //    foreach (var point in path)
            //    {
            //        points.Add(point);
            //        points.Add(point + new Vector2Int(0, corridorShape.Height));

            //        // If the height is more than 1, fill all the points that are above this point
            //        // TODO: this may be slow as we only need the outline points and not all the points inside
            //        for (int i = 1; i <= corridorShape.Height; i++)
            //        {
            //            points.Add(point + new Vector2Int(0, i));
            //        }
            //    }
            //}
            //// If the door line is horizontal and last point is already at the correct Y position, we want to add only corridor width
            //else if (toDoorLine.GetDirectionVector().X != 0 && path.Last().Y == toDoorLine.From.Y)
            //{
            //    foreach (var point in path)
            //    {
            //        points.Add(point);
            //        points.Add(point + new Vector2Int(corridorShape.Width, 0));

            //        // If the height is more than 1, fill all the points that are above this point
            //        // TODO: this may be slow as we only need the outline points and not all the points inside
            //        for (int i = 1; i <= corridorShape.Width; i++)
            //        {
            //            points.Add(point + new Vector2Int(i, 0));
            //        }
            //    }
            //}
            //// If there are missing points to reach the door line, we want to add both corridor width and corridor height
            //else
            {
                foreach (var point in path)
                {
                    points.Add(point);
                    points.Add(point + new Vector2Int(0, corridorShape.Height));
                    points.Add(point + new Vector2Int(corridorShape.Width, 0));

                    // If the height is more than 1, fill all the points that are above this point
                    // TODO: this may be slow as we only need the outline points and not all the points inside
                    for (int i = 1; i <= corridorShape.Height; i++)
                    {
                        points.Add(point + new Vector2Int(0, i));

                        // If the height is more than 1, fill all the points that are next to this point
                        // TODO: this may be slow as we only need the outline points and not all the points inside
                        for (int j = 1; j <= corridorShape.Width; j++)
                        {
                            points.Add(point + new Vector2Int(j, 0));

                            points.Add(point + new Vector2Int(j, i));
                        }
                    }
                }
            }

            var polygon = Helpers.GetPolygonFromTiles(points);

            var modifiedFrom = GetModifiedDoorLine(fromDoorLine);
            var modifiedTo = GetModifiedDoorLine(toDoorLine);

            var fromDiff = modifiedFrom.From - fromDoorLine.From;
            var toDiff = modifiedTo.From - toDoorLine.From;

            var startDoor = GetDoor(path[0], fromDoorLine, fromDiff);
            var endDoor = GetDoor(path[path.Count - 1], toDoorLine, toDiff);

            return new PathfindingResult(polygon, startDoor, endDoor);
        }

        private DoorGrid2D GetDoor(Vector2Int from, OrthogonalLineGrid2D line, Vector2Int diff)
        {
            from += -1 * diff;

            var length = line.GetDirectionVector().X != 0 ? corridorShape.Width : corridorShape.Height;


            var to = from + length * line.GetDirectionVector();

            return new DoorGrid2D(from, to);
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

        private OrthogonalLineGrid2D GetModifiedDoorLine(OrthogonalLineGrid2D line)
        {
            switch (line.GetDirection())
            {
                case OrthogonalLineGrid2D.Direction.Bottom:
                    return line + new Vector2Int(0, -corridorShape.Height);

                case OrthogonalLineGrid2D.Direction.Left:
                    return line + new Vector2Int(-corridorShape.Width, -corridorShape.Height);

                case OrthogonalLineGrid2D.Direction.Top:
                    return line + new Vector2Int(-corridorShape.Width, 0);

                case OrthogonalLineGrid2D.Direction.Right:
                    return line;

                default:
                    throw new ArgumentException();
            }
        }

        private ITilemap<Vector2Int> GetTilemap(TLayout layout)
        {
            var tilemap = new HashSetTilemap<Vector2Int>();

            foreach (var configuration in layout.GetAllConfigurations())
            {
                var outline = configuration.RoomShape.RoomShape + configuration.Position;

                AddPolygonToTilemap(tilemap, outline);
            }

            return tilemap;
        }

        private void AddPolygonToTilemap(ITilemap<Vector2Int> tilemap, PolygonGrid2D polygon)
        {
            foreach (var line in polygon.GetLines())
            {
                var perpendicularOutside = line.GetDirectionVector().RotateAroundCenter(270);
                var doubleWall = line + line.GetDirectionVector().RotateAroundCenter(90);

                if (doubleWall.Length > 2)
                {
                    AddPointsToTilemap(tilemap, doubleWall.Shrink(1, 1).GetPoints());
                }
                
                for (int i = 0; i < minimumRoomDistance; i++)
                {
                    var wall = line + i * perpendicularOutside;
                    AddPointsToTilemap(tilemap, wall.GetPoints());
                }
            }
        }

        private void AddPointsToTilemap(ITilemap<Vector2Int> tilemap, IEnumerable<Vector2Int> points)
        {
            var list = new List<Vector2Int>();

            foreach (var point in points)
            {
                list.Add(point);

                for (int i = 1; i <= corridorShape.Width; i++)
                {
                    list.Add(point + new Vector2Int(-i, 0));

                    for (int j = 1; j <= corridorShape.Height; j++)
                    {
                        list.Add(point + new Vector2Int(-i, -j));

                        // TODO: not optimal, added multiple times
                        list.Add(point + new Vector2Int(0, -j));
                    }
                }
            }

            foreach (var point in list)
            {
                tilemap.AddPoint(point);
            }

            // // Console.WriteLine($"Add points: {string.Join(", ", list.Select(x => x.ToStringShort()))}");
        }

        public class PathfindingResult
        {
            public PolygonGrid2D Outline { get; }

            public DoorGrid2D DoorFrom { get; }

            public DoorGrid2D DoorTo { get; }

            public Dictionary<Vector2Int, int> Costs { get; set; }

            public PathfindingResult(PolygonGrid2D outline, DoorGrid2D doorFrom, DoorGrid2D doorTo)
            {
                Outline = outline;
                DoorFrom = doorFrom;
                DoorTo = doorTo;
            }
        }
    }
}