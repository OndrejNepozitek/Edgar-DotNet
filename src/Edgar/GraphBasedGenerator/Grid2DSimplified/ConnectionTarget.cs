using System.Collections.Generic;
using Edgar.GraphBasedGenerator.Grid2D.Internal;

namespace Edgar.GraphBasedGenerator.Grid2DSimplified
{
    public class ConnectionTarget<TRoom>
    {
        public SimpleConfiguration<TRoom> Configuration { get; }

        public List<RoomTemplateInstanceGrid2D> Corridors { get; }

        public ConnectionTarget(SimpleConfiguration<TRoom> configuration, List<RoomTemplateInstanceGrid2D> corridors = null)
        {
            Configuration = configuration;
            Corridors = corridors;
        }
    }
}