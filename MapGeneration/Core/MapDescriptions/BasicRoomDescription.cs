using System.Collections.Generic;
using MapGeneration.Interfaces.Core.MapDescriptions;

namespace MapGeneration.Core.MapDescriptions
{
    public class BasicRoomDescription : IRoomDescription
    {
        public int Stage => 1;

        public List<IRoomTemplate> RoomTemplates { get; }

        public BasicRoomDescription(List<IRoomTemplate> roomTemplates)
        {
            RoomTemplates = roomTemplates;
        }

        public BasicRoomDescription()
        {
            // TODO: remove later
        }
    }
}