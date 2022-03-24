using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Grid2D.Exceptions;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;

namespace Edgar.GraphBasedGenerator.Grid2D
{
    /// <summary>
    /// Represents a manual door mode which consists of a list of doors provided by the user.
    /// </summary>
    public class ManualDoorModeGrid2D : IDoorModeGrid2D
    {
        /// <summary>
        /// List of available doors.
        /// </summary>
        public List<DoorGrid2D> Doors { get; }

        /// <summary>
        /// List of available door lines.
        /// </summary>
        public List<DoorLineGrid2D> DoorLines { get; }

        private static readonly OrthogonalLineIntersection LineIntersection = new OrthogonalLineIntersection();

        /// <param name="doors">See the <see cref="Doors"/> property.</param>
        public ManualDoorModeGrid2D(List<DoorGrid2D> doors)
        {
            Doors = doors ?? throw new ArgumentNullException(nameof(doors));
        }

        /// <param name="doorLines">See the <see cref="DoorLines"/> property.</param>
        public ManualDoorModeGrid2D(List<DoorLineGrid2D> doorLines)
        {
            DoorLines = doorLines ?? throw new ArgumentNullException(nameof(doorLines));
        }

        public List<DoorLineGrid2D> GetDoors(PolygonGrid2D roomShape)
        {
            if (Doors != null)
            {
                return GetDoorLinesFromDoors(roomShape);
            }
            else
            {
                return GetDoorLinesFromDoorLines(roomShape);
            }
        }

        private List<DoorLineGrid2D> GetDoorLinesFromDoorLines(PolygonGrid2D roomShape)
        {
            var distinctDoorLinesCount = DoorLines.Distinct().Count();

            if (DoorLines.Count != distinctDoorLinesCount)
            {
                throw new DuplicateDoorPositionException(
                    "There are duplicate door lines. All door positions must be unique.");
            }

            var doorLines = new List<DoorLineGrid2D>();

            foreach (var doorLine in DoorLines)
            {
                var compatibleDoorLines = DoorLines
                    .Where(x => x.DoorSocket == doorLine.DoorSocket && x.Length == doorLine.Length &&
                                !x.Equals(doorLine))
                    .Select(x => x.Line)
                    .ToList();

                var doIntersect = LineIntersection.DoIntersect(compatibleDoorLines,
                    new List<OrthogonalLineGrid2D>() {doorLine.Line});

                if (doIntersect)
                {
                    throw new DuplicateDoorPositionException(
                        "Some door lines with the same length and socket intersect each other. All door positions must be unique.");
                }

                doorLines.AddRange(GetDoorLinesFromDoorLine(roomShape, doorLine));
            }

            return doorLines;
        }

        private IEnumerable<DoorLineGrid2D> GetDoorLinesFromDoorLine(PolygonGrid2D roomShape,
            DoorLineGrid2D originalDoorLine)
        {
            // Keep track of whether we found a suitable side of the polygon for the door line
            // We cannot simply return from the for each because door lines on the corner of a polygon may produce two door lines
            var found = false;

            var originalLine = originalDoorLine.Line;

            foreach (var side in roomShape.GetLines())
            {
                if (side.Contains(originalLine.From) == -1 || side.Contains(originalLine.To) == -1)
                    continue;

                bool isGoodDirection;

                // Door lines with both lengths equal to 0 are an exception where the direction is always good
                if (originalDoorLine.Length == 0 && originalLine.Length == 0)
                {
                    isGoodDirection = true;
                }
                // Otherwise the direction must be explicitly provided because of originalLine.Length == 0 cases
                else
                {
                    if (originalLine.GetDirection() == OrthogonalLineGrid2D.Direction.Undefined)
                    {
                        throw new UndirectedDoorLine(
                            $"The door line {originalDoorLine.Line.ToStringShort()} ({originalDoorLine.Length}) is not directed. Please provide a direction to doors with line length 0.");
                    }

                    isGoodDirection = originalLine.GetDirection() == side.GetDirection();
                }

                var fullLineFrom = originalLine.From;
                var fullLineTo = originalLine.To;

                // The door line normally contains only the possible starting positions of the doors
                // Se if we want to check that the whole door line is contained in some side of the polygon, we have to add the length of the door
                if (originalDoorLine.Length > 0)
                {
                    var direction = isGoodDirection ? side.GetDirectionVector() : -1 * side.GetDirectionVector();
                    fullLineTo += originalDoorLine.Length * direction;

                    if (side.Contains(fullLineTo) == -1)
                    {
                        continue;
                    }
                }

                // If the door line has zero length and is directed, then the side must match its horizontal/vertical direction
                // In practice, it means that the user should be able to choose to which side a zero-length corner door belongs
                if (originalLine.Length == 0 && originalLine.GetDirection() != OrthogonalLineGrid2D.Direction.Undefined)
                {
                    var isDoorHorizontal = originalLine.GetDirectionVector().X != 0;
                    var isSideHorizontal = side.GetDirectionVector().X != 0;

                    if (isDoorHorizontal != isSideHorizontal)
                        continue;
                }

                var from = isGoodDirection ? fullLineFrom : fullLineTo;
                var to = from + originalLine.Length * side.GetDirectionVector();

                found = true;
                yield return new DoorLineGrid2D(new OrthogonalLineGrid2D(from, to, side.GetDirection()),
                    originalDoorLine.Length, originalDoorLine.DoorSocket, originalDoorLine.Type);
            }

            if (found == false)
            {
                throw new DoorLineOutsideOfOutlineException(
                    $"The door line {originalDoorLine.Line.ToStringShort()} ({originalDoorLine.Length}) is not on the outline of the polygon {roomShape}. Make sure that all the door lines of a manual door mode are on the outline of the polygon.");
            }
        }

        private List<DoorLineGrid2D> GetDoorLinesFromDoors(PolygonGrid2D roomShape)
        {
            if (Doors.Distinct().Count() != Doors.Count)
            {
                throw new DuplicateDoorPositionException("All door positions must be unique");
            }

            var doorLines = new List<DoorLineGrid2D>();

            foreach (var doorPosition in Doors)
            {
                doorLines.AddRange(GetDoorLinesFromDoor(roomShape, doorPosition));
            }

            return doorLines;
        }

        private IEnumerable<DoorLineGrid2D> GetDoorLinesFromDoor(PolygonGrid2D roomShape, DoorGrid2D door)
        {
            var found = false;
            var doorLine = new OrthogonalLineGrid2D(door.From, door.To);

            foreach (var side in roomShape.GetLines())
            {
                if (side.Contains(doorLine.From) == -1 || side.Contains(doorLine.To) == -1)
                    continue;

                // It is not possible to have the same behaviour here as with door lines and zero-length directed corner doors
                // Because doors do not have direction right now

                var isGoodDirection = doorLine.From + doorLine.Length * side.GetDirectionVector() == doorLine.To;
                var from = isGoodDirection ? doorLine.From : doorLine.To;

                found = true;
                yield return new DoorLineGrid2D(new OrthogonalLineGrid2D(from, from, side.GetDirection()),
                    doorLine.Length, door.Socket, door.Type);
            }

            if (found == false)
            {
                throw new DoorLineOutsideOfOutlineException(
                    $"The door line {doorLine.ToStringShort()} is not on the outline of the polygon {roomShape}. Make sure that all the door lines of a manual door mode are on the outline of the polygon.");
            }
        }
    }
}