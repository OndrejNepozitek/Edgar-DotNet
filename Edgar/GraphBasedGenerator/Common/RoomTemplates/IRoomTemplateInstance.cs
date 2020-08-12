namespace Edgar.GraphBasedGenerator.Common.RoomTemplates
{
    public interface IRoomTemplateInstance<out TRoomShape>
    {
        int Id { get; }

        IRoomTemplate<TRoomShape> RoomTemplate { get; }
    }
}