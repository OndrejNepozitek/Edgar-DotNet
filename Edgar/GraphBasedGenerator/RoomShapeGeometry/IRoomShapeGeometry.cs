namespace Edgar.GraphBasedGenerator.RoomShapeGeometry
{
    public interface IRoomShapeGeometry<in TConfiguration>
    {
        int GetOverlapArea(TConfiguration configuration1, TConfiguration configuration2);

        bool DoHaveMinimumDistance(TConfiguration configuration1, TConfiguration configuration2, int minimumDistance);

        /// <summary>
        /// Gets the distance of centers of the two rooms.
        /// </summary>
        /// <remarks>
        /// Will be probably removed later.
        /// </remarks>
        int GetCenterDistance(TConfiguration configuration1, TConfiguration configuration2);
    }
}