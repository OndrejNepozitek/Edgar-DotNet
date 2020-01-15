using System.Collections.Generic;
using MapGeneration.Interfaces.Core.MapDescriptions;
using Newtonsoft.Json;

namespace MapGeneration.Core.MapDescriptions
{
    public class CorridorRoomDescription : IRoomDescription
    {
        public int Stage => 2;

        public List<IRoomTemplate> RoomTemplates { get; }

        [JsonConstructor]
        public CorridorRoomDescription(List<IRoomTemplate> roomTemplates)
        {
            RoomTemplates = roomTemplates;
        }

        public CorridorRoomDescription()
        {
            // TODO: remove later
        }
    }
}