namespace Edgar.GraphBasedGenerator.RoomTemplates
{
    public interface IRoomTemplateInstance<TRoomShape>
    {
        int Id { get; }

        IRoomTemplate<TRoomShape> RoomTemplate { get; }
    }
}