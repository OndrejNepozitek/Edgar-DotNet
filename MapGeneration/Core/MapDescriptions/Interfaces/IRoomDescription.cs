using System.Collections.Generic;

namespace MapGeneration.Core.MapDescriptions.Interfaces
{
    public interface IRoomDescription
    {
        int Stage { get; }

        List<IRoomTemplate> RoomTemplates { get; }
    }
}