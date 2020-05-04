using System.Collections.Generic;
using MapGeneration.Core.MapDescriptions;

namespace MapGeneration.Simplified
{
    public class ConnectionTarget<TRoom>
    {
        public SimpleConfiguration<TRoom> Configuration { get; }

        public List<RoomTemplateInstance> Corridors { get; }

        public ConnectionTarget(SimpleConfiguration<TRoom> configuration, List<RoomTemplateInstance> corridors = null)
        {
            Configuration = configuration;
            Corridors = corridors;
        }
    }
}