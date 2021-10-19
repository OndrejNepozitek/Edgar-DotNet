namespace Edgar.GraphBasedGenerator.Common.Doors
{
    /// <summary>
    /// An interface that can decide whether two door sockets are compatible with each other.
    /// </summary>
    public interface IDoorSocketResolver
    {
        /// <summary>
        /// Checks whether two sockets are compatible with one another.
        /// </summary>
        /// <param name="socket1"></param>
        /// <param name="socket2"></param>
        /// <returns></returns>
        bool AreCompatible(IDoorSocket socket1, IDoorSocket socket2);
    }
}