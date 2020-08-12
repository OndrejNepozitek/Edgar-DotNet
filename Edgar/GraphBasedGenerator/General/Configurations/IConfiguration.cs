namespace Edgar.GraphBasedGenerator.General.Configurations
{
    public interface IShapeConfiguration<TRoomShape>
    {
        TRoomShape RoomShape { get; set; }
    }

    public interface IRoomConfiguration<TRoom>
    {
        TRoom Room { get; set; }
    }

    public interface IPositionConfiguration<TPosition>
    {
        TPosition Position { get; set; }
    }

    public interface IConfiguration<TRoomShape, TPosition, TRoom> : IShapeConfiguration<TRoomShape>, IPositionConfiguration<TPosition>, IRoomConfiguration<TRoom>
    {

    }
}