using System.Collections.Generic;
using MapGeneration.Core.MapDescriptions.Interfaces;
using Newtonsoft.Json;

namespace MapGeneration.Core.MapDescriptions
{
    public class CorridorRoomDescription : IRoomDescription
    {
        public int Stage => 2;

        public List<RoomTemplate> RoomTemplates { get; }

        [JsonConstructor]
        public CorridorRoomDescription(List<RoomTemplate> roomTemplates)
        {
            RoomTemplates = roomTemplates;
        }

        public CorridorRoomDescription()
        {
            // TODO: remove later
        }
    }
}