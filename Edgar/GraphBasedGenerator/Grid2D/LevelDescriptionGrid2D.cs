using Edgar.GraphBasedGenerator.Common;

namespace Edgar.GraphBasedGenerator.Grid2D
{
    /// <summary>
    /// Describes the structure of a level on the 2D (integer) grid.
    /// </summary>
    /// <typeparam name="TRoom"></typeparam>
    public class LevelDescriptionGrid2D<TRoom> : LevelDescription<TRoom, RoomDescriptionGrid2D>
    {
        /// <summary>
        /// Name of the level description. Optional. Used mainly for debugging purposes.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Minimum distance of individual rooms. Must be a non-negative number. Defaults to 0.
        /// </summary>
        /// <remarks>
        /// n = 0 - a point can be contained on the outlines of multiple different rooms.
        /// n > 0 - the manhattan distance of 2 outline points of different rooms must be at least n.
        /// </remarks>
        public int MinimumRoomDistance { get; set; } = 0;

        /// <summary>
        /// Default room template repeat mode that is used if there is no repeat mode specified on the room template itself.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="RoomTemplateRepeatMode.NoRepeat"/>, i.e. room templates should not repeat in a level if possible.
        /// </remarks>
        public RoomTemplateRepeatMode? RoomTemplateRepeatModeDefault { get; set; } = RoomTemplateRepeatMode.NoRepeat;

        /// <summary>
        /// Room template repeat mode override that, when not null, overrides repeat modes from individual room templates.
        /// </summary>
        /// <remarks>
        /// Defaults to null.
        /// </remarks>
        public RoomTemplateRepeatMode? RoomTemplateRepeatModeOverride { get; set; }
    }
}