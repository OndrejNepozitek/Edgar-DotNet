using GeneralAlgorithms.DataStructures.Common;

namespace MapGeneration.Core.Configurations.Interfaces
{
    // TODO: do we need this?
    public interface IPositionConfiguration
    {
        /// <summary>
        /// Position of a room.
        /// </summary>
        IntVector2 Position { get; }
    }
}