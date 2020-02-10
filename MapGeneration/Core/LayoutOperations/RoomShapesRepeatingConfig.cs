namespace MapGeneration.Core.LayoutOperations
{
    // TODO: how to name this?
    public class RoomShapesRepeatingConfig
    {
        public RoomShapesRepeating Type { get; set; } = RoomShapesRepeating.Any;

        public bool ThrowIfNotSatisfied { get; set; } = true;
    }
}