using System;
using System.Collections.Generic;
using Edgar.GraphBasedGenerator.Common.RoomTemplates;

namespace Edgar.GraphBasedGenerator.Grid2D
{
    /// <summary>
    /// Represents a description of a room in a level.
    /// </summary>
    public class RoomDescriptionGrid2D : IRoomDescription
    {
        /// <summary>
        /// Whether the room is a corridor or not.
        /// </summary>
        public bool IsCorridor { get; }

        /// <summary>
        /// Room templates available for the room.
        /// </summary>
        public List<RoomTemplateGrid2D> RoomTemplates { get; }

        public RoomDescriptionGrid2D(bool isCorridor, List<RoomTemplateGrid2D> roomTemplates)
        {
            if (roomTemplates == null) 
                throw new ArgumentNullException(nameof(roomTemplates));

            if (roomTemplates.Count == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(roomTemplates));

            IsCorridor = isCorridor;
            RoomTemplates = roomTemplates;
        }
    }
}