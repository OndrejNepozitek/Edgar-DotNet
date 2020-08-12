using System.Collections.Generic;
using MapGeneration.Core.MapDescriptions.Interfaces;
using Newtonsoft.Json;

namespace MapGeneration.Core.MapDescriptions
{
    /// <summary>
    /// Class that describes a corridor room.
    /// </summary>
    public class CorridorRoomDescription : IRoomDescription
    {
        public bool IsCorridor => true;

        /// <summary>
        /// This room is handled in the second stage.
        /// </summary>
        public int Stage => 2;

        /// <summary>
        /// List of room templates available for this room.
        /// </summary>
        public List<RoomTemplate> RoomTemplates { get; }

        [JsonConstructor]
        public CorridorRoomDescription(List<RoomTemplate> roomTemplates)
        {
            RoomTemplates = roomTemplates;
        }
    }
}