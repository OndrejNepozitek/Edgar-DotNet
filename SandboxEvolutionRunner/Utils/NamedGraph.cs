using GeneralAlgorithms.DataStructures.Graphs;

namespace SandboxEvolutionRunner.Utils
{
    public class NamedGraph
    {
        public IGraph<int> Graph { get; }

        public string Name { get; }

        public NamedGraph(IGraph<int> graph, string name = "Graph")
        {
            Graph = graph;
            Name = name;
        }
    }
}