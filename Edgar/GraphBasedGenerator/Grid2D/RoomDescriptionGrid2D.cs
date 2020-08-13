using System.Collections.Generic;
using Edgar.GraphBasedGenerator.Common.RoomTemplates;

namespace Edgar.GraphBasedGenerator.Grid2D
{
    public class RoomDescriptionGrid2D : IRoomDescription
    {
        public bool IsCorridor { get; set; }

        public List<RoomTemplateGrid2D> RoomTemplates { get; set; }
    }
}