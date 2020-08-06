namespace Edgar.GraphBasedGenerator.RoomTemplates
{
    public interface IRoomTemplate<TRoomShape>
    {
        TRoomShape RoomShape { get; }
    }
}