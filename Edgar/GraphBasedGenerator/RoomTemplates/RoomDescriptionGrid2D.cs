using System.Collections.Generic;
using MapGeneration.Core.MapDescriptions;

namespace Edgar.GraphBasedGenerator.RoomTemplates
{
    public class RoomDescriptionGrid2D : IRoomDescription
    {
        public bool IsCorridor { get; set; }

        public List<RoomTemplate> RoomTemplates { get; set; }
    }
}