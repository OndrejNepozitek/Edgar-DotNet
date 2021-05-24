namespace Edgar.GraphBasedGenerator.Common.Constraints.FixedConfigurationConstraint
{
    public interface IFixedConfigurationConstraint<TRoom>
    {
        bool IsFixedShape(TRoom room);

        bool IsFixedPosition(TRoom room);
    }

    public interface IFixedConfigurationConstraint<TRoomShape, TPosition, TRoom> : IFixedConfigurationConstraint<TRoom>
    {
        bool TryGetFixedShape(TRoom room, out TRoomShape shape);

        bool TryGetFixedPosition(TRoom room, out TPosition position);
    }
}