namespace Edgar.GraphBasedGenerator.RoomTemplates
{
    public class RoomTemplateInstance<TRoomShape> : IRoomTemplateInstance<TRoomShape>
    {
        public int Id { get; }

        public IRoomTemplate<TRoomShape> RoomTemplate { get; }
    }
}