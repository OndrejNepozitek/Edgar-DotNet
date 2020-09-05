namespace Edgar.GraphBasedGenerator.Grid2D.Internal.Corridors
{
    public class CorridorsPathfindingConfiguration
    {
        public int CorridorWidth { get; }
        
        public int CorridorHeight { get; }

        public DoorGrid2D HorizontalDoor { get; }

        public DoorGrid2D VerticalDoor { get; }

        public int? MinimumRoomDistance { get; }

        public CorridorsPathfindingConfiguration(int corridorWidth, int corridorHeight, DoorGrid2D horizontalDoor = null, DoorGrid2D verticalDoor = null, int? minimumRoomDistance = null)
        {
            CorridorWidth = corridorWidth;
            CorridorHeight = corridorHeight;
            HorizontalDoor = horizontalDoor;
            VerticalDoor = verticalDoor;
            MinimumRoomDistance = minimumRoomDistance;
        }
    }
}