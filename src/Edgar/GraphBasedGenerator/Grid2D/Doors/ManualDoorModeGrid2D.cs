using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

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

        /// <param name="doors">See the <see cref="Doors"/> property.</param>
        public ManualDoorModeGrid2D(List<DoorGrid2D> doors)
        {
            Doors = doors ?? throw new ArgumentNullException(nameof(doors));
        }

        public List<DoorLineGrid2D> GetDoors(PolygonGrid2D roomShape)
        {
            if (Doors.Distinct().Count() != Doors.Count)
                throw new ArgumentException("All door positions must be unique");

            var doors = new List<DoorLineGrid2D>();

            foreach (var doorPosition in Doors)
            {
                doors.AddRange(GetDoorLine(roomShape, doorPosition));
            }

            return doors;
        }

        private IEnumerable<DoorLineGrid2D> GetDoorLine(PolygonGrid2D polygon, DoorGrid2D door)
        {
            var found = false;
            var doorLine = new OrthogonalLineGrid2D(door.From, door.To);

            foreach (var side in polygon.GetLines())
            {
                if (side.Contains(doorLine.From) == -1 || side.Contains(doorLine.To) == -1)
                    continue;

                var isGoodDirection = doorLine.From + doorLine.Length * side.GetDirectionVector() == doorLine.To;
                var from = isGoodDirection ? doorLine.From : doorLine.To;

                found = true;
                yield return new DoorLineGrid2D(new OrthogonalLineGrid2D(from, from, side.GetDirection()), doorLine.Length, door.Socket, door.Type);
            }

            if (found == false)
            {
                throw new ArgumentException($"The door line {doorLine.ToStringShort()} is not on the outline of the polygon {polygon}. Make sure that all the door lines of a manual door mode are on the outline of the polygon.");
            }
        }
    }
}