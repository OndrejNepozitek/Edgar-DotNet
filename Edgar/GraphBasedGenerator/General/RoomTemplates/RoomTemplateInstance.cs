using MapGeneration.Core.MapDescriptions;

namespace Edgar.GraphBasedGenerator.General.RoomTemplates
{
    public class RoomTemplateInstance<TRoomShape> : IRoomTemplateInstance<TRoomShape>
    {
        public int Id { get; }

        public IRoomTemplate<TRoomShape> RoomTemplate { get; }
    }
}