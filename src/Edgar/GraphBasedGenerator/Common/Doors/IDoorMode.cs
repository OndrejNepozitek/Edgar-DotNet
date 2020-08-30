using System.Collections.Generic;

namespace Edgar.GraphBasedGenerator.Common.Doors
{
    public interface IDoorMode<TDoor, in TRoomShape>
    {
        List<TDoor> GetDoors(TRoomShape roomShape);
    }
}