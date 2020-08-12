namespace Edgar.GraphBasedGenerator.General.RoomTemplates
{
    public interface IRoomTemplateInstance<out TRoomShape>
    {
        int Id { get; }

        IRoomTemplate<TRoomShape> RoomTemplate { get; }
    }
}