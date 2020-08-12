using System.Collections.Generic;
using Edgar.GraphBasedGenerator.General.RoomTemplates;
using MapGeneration.Core.MapDescriptions;

namespace Edgar.GraphBasedGenerator.Grid2D
{
    public class RoomDescriptionGrid2D : IRoomDescription
    {
        public bool IsCorridor { get; set; }

        public List<RoomTemplate> RoomTemplates { get; set; }
    }
}