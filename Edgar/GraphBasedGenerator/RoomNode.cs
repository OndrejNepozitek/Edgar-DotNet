namespace Edgar.GraphBasedGenerator
{
    public class RoomNode<TRoom>
    {
        public TRoom Room { get; }

        public int Id { get; }

        public RoomNode(int id, TRoom room)
        {
            Id = id;
            Room = room;
        }
    }
}