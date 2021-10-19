namespace Edgar.GraphBasedGenerator.Common.Doors
{
    /// <summary>
    /// Represents a door socket which can decide if it is compatible with another socket.
    /// </summary>
    /// <remarks>
    /// Intended use:
    /// Make some doors compatible only with some other specific doors.
    /// For example, a door position can be reserved for a secret room.
    /// </remarks>
    public interface IDoorSocket
    {
        /// <summary>
        /// Decides whether the door socket is compatible with the other socket.
        /// </summary>
        /// <remarks>
        /// See <see cref="DefaultDoorSocketResolver"/> for more information.
        /// </remarks>
        /// <param name="otherSocket"></param>
        /// <returns></returns>
        bool IsCompatibleWith(IDoorSocket otherSocket);
    }
}