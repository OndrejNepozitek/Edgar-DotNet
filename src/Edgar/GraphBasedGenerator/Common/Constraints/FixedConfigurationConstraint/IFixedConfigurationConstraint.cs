namespace Edgar.GraphBasedGenerator.Common.Constraints.FixedConfigurationConstraint
{
    public interface IFixedConfigurationConstraint<TRoomShape, TPosition, TRoom>
    {
        bool IsFixedShape(TRoom room);

        bool TryGetFixedShape(TRoom room, out TRoomShape shape);

        bool IsFixedPosition(TRoom room);

        bool TryGetFixedPosition(TRoom room, out TPosition position);
    }
}