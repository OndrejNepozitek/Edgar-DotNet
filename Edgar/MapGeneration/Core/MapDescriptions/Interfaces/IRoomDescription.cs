using System.Collections.Generic;

namespace MapGeneration.Core.MapDescriptions.Interfaces
{
    public interface IRoomDescription
    {
        bool IsCorridor { get; }

        int Stage { get; }

        List<RoomTemplate> RoomTemplates { get; }
    }
}