using MapGeneration.Core.MapDescriptions.Interfaces;

namespace SandboxEvolutionRunner.Utils
{
    public class NamedMapDescription
    {
        public IMapDescription<int> MapDescription { get; }

        public string Name { get; }

        public bool IsWithCorridors { get; }

        public NamedMapDescription(IMapDescription<int> mapDescription, string name, bool isWithCorridors)
        {
            MapDescription = mapDescription;
            Name = name;
            IsWithCorridors = isWithCorridors;
        }

        public NamedMapDescription(IMapDescription<int> mapDescription, NamedGraph graph, bool isWithCorridors)
        {
            MapDescription = mapDescription;
            IsWithCorridors = isWithCorridors;
            Name = graph.Name;
        }
    }
}