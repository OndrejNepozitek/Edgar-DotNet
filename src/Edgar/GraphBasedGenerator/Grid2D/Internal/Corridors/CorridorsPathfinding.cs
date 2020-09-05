using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common.Configurations;
using Edgar.Legacy.Core.Layouts.Interfaces;

namespace Edgar.GraphBasedGenerator.Grid2D.Internal.Corridors
{
    public class CorridorsPathfinding<TLayout, TRoom, TConfiguration>
        where TLayout : ILayout<TRoom, TConfiguration>
        where TConfiguration : IConfiguration<RoomTemplateInstanceGrid2D, Vector2Int, TRoom>, new()
    {
        private readonly RectangleGrid2D corridorShape;
        private readonly int minimumRoomDistance;
        private readonly DoorGrid2D horizontalDoor;
        private readonly DoorGrid2D verticalDoor;

        private readonly CorridorsPathfinder corridorsPathfinder = new CorridorsPathfinder();

        public CorridorsPathfinding(int corridorWidth, int corridorHeight, int minimumRoomDistance, DoorGrid2D horizontalDoor, DoorGrid2D verticalDoor)
        {
            this.corridorShape = new RectangleGrid2D(new Vector2Int(0, 0), new Vector2Int(corridorWidth, corridorHeight));
            this.minimumRoomDistance = minimumRoomDistance;
            this.horizontalDoor = horizontalDoor ?? new DoorGrid2D(new Vector2Int(0, 0), new Vector2Int(corridorWidth, 0));
            this.verticalDoor = verticalDoor ?? new DoorGrid2D(new Vector2Int(0, 0), new Vector2Int(0, corridorHeight));
        }

        public PathfindingResult FindPath(TLayout layout, TRoom fromRoom, TRoom toRoom)
        {
            var tilemap = new CorridorsPathfindingTilemap<TRoom,TConfiguration>(corridorShape, minimumRoomDistance);
            var tilemapWithoutRoomDistance = new CorridorsPathfindingTilemap<TRoom,TConfiguration>(corridorShape, 0);

            foreach (var configuration in layout.GetAllConfigurations())
            {
                tilemap.AddRoom(configuration);
                tilemapWithoutRoomDistance.AddRoom(configuration);
            }

            return FindPath(layout, fromRoom, toRoom, tilemap, tilemapWithoutRoomDistance);
        }

        public PathfindingResult FindPath(TLayout layout, TRoom fromRoom, TRoom toRoom, ITilemap<Vector2Int, TRoom> tilemap, ITilemap<Vector2Int> tilemapWithoutRoomDistance)
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

            var (path, costs) = corridorsPathfinder.FindPath(fromDoorMapping.Keys.ToList(), toDoorMapping.Keys.ToList(), fromDoorMapping, toDoorMapping, tilemapWithoutRoomDistance, 2 * minimumRoomDistance, false);

            if (path == null)
            {
                var (fromPoints, fromShortcuts) = GetEndPoints(fromDoorMapping, fromRoom, tilemap, tilemapWithoutRoomDistance);
                var (toPoints, toShortcuts) = GetEndPoints(toDoorMapping, toRoom, tilemap, tilemapWithoutRoomDistance, true);

                (path, costs) = corridorsPathfinder.FindPath(fromPoints, toPoints, fromDoorMapping, toDoorMapping, tilemap);

                if (path != null)
                {
                    if (fromShortcuts.ContainsKey(path[0]))
                    {
                        path = fromShortcuts[path[0]].Concat(path).ToList();
                    }

                    if (toShortcuts.ContainsKey(path.Last()))
                    {
                        path = path.Concat(toShortcuts[path.Last()]).ToList();
                    }
                }
            }

            // No path was found
            if (path == null)
            {
                var result = new PathfindingResult(false, null, null, null);
                result.Costs = costs;
                result.Tilemap = tilemap;

                return result;
            }

            var corridor = GetCorridorFromPath(path, fromDoorMapping[path[0]], toDoorMapping[path.Last()]);
            corridor.Costs = costs;
            corridor.Tilemap = tilemap;

            return corridor;
        }

        private (List<Vector2Int> endPoints, Dictionary<Vector2Int, List<Vector2Int>> shortcuts) GetEndPoints(Dictionary<Vector2Int, OrthogonalLineGrid2D> pointToDoorMapping, TRoom room, ITilemap<Vector2Int, TRoom> tilemap, ITilemap<Vector2Int> tilemapWithoutRoomDistance, bool reverseShortcut = false)
        {
            if (minimumRoomDistance == 0)
            {
                return (pointToDoorMapping.Keys.ToList(), new Dictionary<Vector2Int, List<Vector2Int>>());
            }

            var shortcuts = new Dictionary<Vector2Int, List<Vector2Int>>();
            var endPoints = new List<Vector2Int>();

            var minimumDistanceExceptions = new List<TRoom>()
            {
                room
            };

            foreach (var pair in pointToDoorMapping.ToList())
            {
                var point = pair.Key;
                var line = pair.Value;

                var perpendicularDirection = line.GetDirectionVector().RotateAroundCenter(270);

                var newPoint = point + minimumRoomDistance * perpendicularDirection;


                var shortcut = new List<Vector2Int>();
                var isSuccessful = true;
                for (int i = 0; i < minimumRoomDistance; i++)
                {
                    var shortcutPoint = point + i * perpendicularDirection;

                    if (!IsEmpty(shortcutPoint, minimumDistanceExceptions, tilemap, tilemapWithoutRoomDistance))
                    {
                        isSuccessful = false;
                        break;
                    }

                    shortcut.Add(shortcutPoint);
                }

                if (!isSuccessful)
                {
                    continue;
                }

                if (reverseShortcut)
                {
                    shortcut.Reverse();
                }

                // TODO: should I overwrite possible duplicates?
                pointToDoorMapping[newPoint] = line;
                endPoints.Add(newPoint);
                shortcuts[newPoint] = shortcut;
            }

            return (endPoints, shortcuts);
        }

        private bool IsDoorCorrectLength(DoorLineGrid2D doorLine)
        {
            if (doorLine.Line.GetDirectionVector().X != 0)
            {
                return doorLine.Length == horizontalDoor.GetLine().Length;
            }

            return doorLine.Length == verticalDoor.GetLine().Length;
        }

        private bool IsEmpty(Vector2Int point, List<TRoom> minimumDistanceExceptions, ITilemap<Vector2Int, TRoom> tilemap, ITilemap<Vector2Int> tilemapWithoutRoomDistance)
        {
            if (tilemap.IsEmpty(point))
            {
                return true;
            }

            if (tilemapWithoutRoomDistance.IsEmpty(point))
            {
                var minimumDistanceRooms = tilemap.GetRoomsOnTile(point);

                return minimumDistanceRooms.All(minimumDistanceExceptions.Contains);
            }

            return false;
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

            return new PathfindingResult(true, polygon, startDoor, endDoor);
        }

        private DoorGrid2D GetDoor(Vector2Int from, OrthogonalLineGrid2D line, Vector2Int diff)
        {
            from += -1 * diff;

            var length = line.GetDirectionVector().X != 0 ? horizontalDoor.GetLine().Length : verticalDoor.GetLine().Length;


            var to = from + length * line.GetDirectionVector();

            return new DoorGrid2D(from, to);
        }



        private OrthogonalLineGrid2D GetModifiedDoorLine(OrthogonalLineGrid2D line)
        {
            switch (line.GetDirection())
            {
                case OrthogonalLineGrid2D.Direction.Bottom:
                    return line + new Vector2Int(0, -corridorShape.Height + verticalDoor.From.Y);

                case OrthogonalLineGrid2D.Direction.Left:
                    return line + new Vector2Int(-corridorShape.Width + horizontalDoor.From.X, -corridorShape.Height);

                case OrthogonalLineGrid2D.Direction.Top:
                    return line + new Vector2Int(-corridorShape.Width, -verticalDoor.From.Y);

                case OrthogonalLineGrid2D.Direction.Right:
                    return line + new Vector2Int(-horizontalDoor.From.X, 0);

                default:
                    throw new ArgumentException();
            }
        }

        public class PathfindingResult
        {
            public bool IsSuccessful { get; }

            public PolygonGrid2D Outline { get; }

            public DoorGrid2D DoorFrom { get; }

            public DoorGrid2D DoorTo { get; }

            public Dictionary<Vector2Int, int> Costs { get; set; }

            public ITilemap<Vector2Int, TRoom> Tilemap { get; set; }

            public PathfindingResult(bool isSuccessful, PolygonGrid2D outline, DoorGrid2D doorFrom, DoorGrid2D doorTo)
            {
                Outline = outline;
                DoorFrom = doorFrom;
                DoorTo = doorTo;
                IsSuccessful = isSuccessful;
            }
        }
    }
}