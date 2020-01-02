using System.Collections.Generic;

namespace MapGeneration.Interfaces.Core.MapDescriptions
{
    public interface IRoomDescription
    {
        int Stage { get; }

        List<IRoomTemplate> RoomTemplates { get; }
    }
}