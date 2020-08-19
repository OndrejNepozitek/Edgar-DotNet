using Edgar.GraphBasedGenerator.Common;
using Edgar.Legacy.Core.MapDescriptions;

namespace Edgar.GraphBasedGenerator.Grid2D
{
    public class LevelDescriptionGrid2D<TRoom> : LevelDescription<TRoom, RoomDescriptionGrid2D>
    {
        public string Name { get; set; }

        public OutlineMode OutlineMode { get; set; } = OutlineMode.Points;

        public int MinimumRoomDistance { get; set; } = 0;

        public RoomTemplateRepeatMode? RoomTemplateRepeatModeDefault { get; set; }

        public RoomTemplateRepeatMode? RoomTemplateRepeatModeOverride { get; set; }
    }
}