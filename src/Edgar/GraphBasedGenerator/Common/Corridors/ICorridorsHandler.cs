using System.Collections.Generic;

namespace Edgar.GraphBasedGenerator.Common.Corridors
{
    public interface ICorridorsHandler<in TLayout, in TRoom>
    {
        bool AddCorridors(TLayout layout, IEnumerable<TRoom> chain);
    }
}