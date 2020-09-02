namespace Edgar.GraphBasedGenerator.Common
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

        public override string ToString()
        {
            return $"Room: {Room}, Id: {Id}";
        }
    }
}