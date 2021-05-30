using System.Collections.Generic;

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
            return $"{Id}: {Room}";
        }

        //protected bool Equals(RoomNode<TRoom> other)
        //{
        //    return EqualityComparer<TRoom>.Default.Equals(Room, other.Room) && Id == other.Id;
        //}

        //public override bool Equals(object obj)
        //{
        //    if (ReferenceEquals(null, obj)) return false;
        //    if (ReferenceEquals(this, obj)) return true;
        //    if (obj.GetType() != this.GetType()) return false;
        //    return Equals((RoomNode<TRoom>)obj);
        //}

        //public override int GetHashCode()
        //{
        //    return Id;
        //}

        //public static bool operator ==(RoomNode<TRoom> left, RoomNode<TRoom> right)
        //{
        //    return Equals(left, right);
        //}

        //public static bool operator !=(RoomNode<TRoom> left, RoomNode<TRoom> right)
        //{
        //    return !Equals(left, right);
        //}
    }
}