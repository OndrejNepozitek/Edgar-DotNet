namespace Edgar.GraphBasedGenerator.General.RoomTemplates
{
    public interface IRoomTemplate<out TRoomShape>
    {
        TRoomShape RoomShape { get; }
    }
}