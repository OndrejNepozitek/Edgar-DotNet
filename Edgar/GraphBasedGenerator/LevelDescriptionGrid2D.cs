using Edgar.GraphBasedGenerator.RoomTemplates;

namespace Edgar.GraphBasedGenerator
{
    public class LevelDescriptionGrid2D<TRoom> : LevelDescription<TRoom, RoomDescriptionGrid2D>
    {
        public string Name { get; set; }

        public OutlineMode OutlineMode { get; set; } = OutlineMode.Points;

        public int MinimumRoomDistance { get; set; } = 0;
    }
}