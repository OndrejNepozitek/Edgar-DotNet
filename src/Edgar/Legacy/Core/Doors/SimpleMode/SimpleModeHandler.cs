using System;
using System.Collections.Generic;
using Edgar.Geometry;
using Edgar.Legacy.Core.Doors.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;

namespace Edgar.Legacy.Core.Doors.SimpleMode
{
    /// <summary>
    /// Generates door positions for <see cref="SimpleDoorMode"/>.
    /// </summary>
    public class SimpleModeHandler : IDoorHandler
    {
        /// <inheritdoc />
        public List<DoorLine> GetDoorPositions(PolygonGrid2D polygon, IDoorMode doorModeRaw)
        {
            if (!(doorModeRaw is SimpleDoorMode doorMode))
                throw new InvalidOperationException("Invalid door mode supplied");

            var lines = new List<DoorLine>();

            foreach (var line in polygon.GetLines())
            {
                if (line.Length < 2 * doorMode.CornerDistance + doorMode.DoorLength)
                    continue;

                lines.Add(new DoorLine(
                    line.Shrink(doorMode.CornerDistance, doorMode.CornerDistance + doorMode.DoorLength),
                    doorMode.DoorLength));
            }

            return lines;
        }
    }
}