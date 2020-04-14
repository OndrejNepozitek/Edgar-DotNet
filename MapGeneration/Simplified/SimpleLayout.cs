using System.Collections.Generic;
using System.Linq;
using GeneralAlgorithms.DataStructures.Graphs;
using MapGeneration.Core.Layouts.Interfaces;

namespace MapGeneration.Simplified
{
    public class SimpleLayout<TRoom> : ILayout<TRoom, SimpleConfiguration<TRoom>>
    {
        public IGraph<TRoom> Graph { get; set; } = new UndirectedAdjacencyListGraph<TRoom>();

        private readonly Dictionary<TRoom, SimpleConfiguration<TRoom>> configurations = new Dictionary<TRoom, SimpleConfiguration<TRoom>>();

        public SimpleConfiguration<TRoom> GetConfiguration(TRoom room)
        {
            if (GetConfiguration(room, out var configuration))
            {
                return configuration;
            }

            return null;
        }

        public bool GetConfiguration(TRoom room, out SimpleConfiguration<TRoom> configuration)
        {
            if (configurations.TryGetValue(room, out configuration))
            {
                return true;
            }

            return false;
        }

        public List<SimpleConfiguration<TRoom>> GetNeighborConfigurations(TRoom room)
        {
            return Graph
                .GetNeighbours(room)
                .Select(GetConfiguration)
                .Where(x => x != null)
                .ToList();
        }

        public void SetConfiguration(TRoom room, SimpleConfiguration<TRoom> configuration)
        {
            configurations[room] = configuration;
        }

        public void RemoveConfiguration(TRoom room)
        {
            configurations.Remove(room);
        }

        public IEnumerable<SimpleConfiguration<TRoom>> GetAllConfigurations()
        {
            return configurations.Values;
        }
    }
}