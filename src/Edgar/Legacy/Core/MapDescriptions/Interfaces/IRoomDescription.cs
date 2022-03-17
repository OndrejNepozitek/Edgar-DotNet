using System.Collections.Generic;

namespace Edgar.Legacy.Core.MapDescriptions.Interfaces
{
    public interface IRoomDescription : GraphBasedGenerator.Common.RoomTemplates.IRoomDescription
    {
        int Stage { get; }

        List<RoomTemplate> RoomTemplates { get; }
    }
}