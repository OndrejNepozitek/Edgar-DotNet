namespace Edgar.GraphBasedGenerator.Common.RoomTemplates
{
    public interface IRoomTemplate<out TRoomShape>
    {
        TRoomShape RoomShape { get; }
    }
}