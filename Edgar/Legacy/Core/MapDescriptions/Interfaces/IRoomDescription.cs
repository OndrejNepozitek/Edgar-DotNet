using System.Collections.Generic;

namespace Edgar.Legacy.Core.MapDescriptions.Interfaces
{
    public interface  IRoomDescription : Edgar.GraphBasedGenerator.General.RoomTemplates.IRoomDescription
    {
        int Stage { get; }

        List<RoomTemplate> RoomTemplates { get; }
    }
}