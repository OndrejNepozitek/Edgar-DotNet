using System.Collections.Generic;

namespace MapGeneration.Core.MapDescriptions.Interfaces
{
    public interface  IRoomDescription : Edgar.GraphBasedGenerator.RoomTemplates.IRoomDescription
    {
        int Stage { get; }

        List<RoomTemplate> RoomTemplates { get; }
    }
}