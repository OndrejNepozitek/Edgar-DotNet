namespace Edgar.GraphBasedGenerator.Grid2D.Internal.Corridors
{
    public class CorridorsPathfindingConfiguration
    {
        public int CorridorWidth { get; set; }
        
        public int CorridorHeight { get; set; }

        public DoorGrid2D HorizontalDoor { get; set; }

        public DoorGrid2D VerticalDoor { get; set; }

        public int? MinimumRoomDistance { get; set; }

        public int MinimumPlacementDistance { get; set; }

        public int MaximumPlacementDistance { get; set; }

        public int MaximumPathCost { get; set; } = int.MaxValue;
    }
}