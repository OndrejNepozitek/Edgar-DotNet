using Edgar.Graphs;

namespace SandboxEvolutionRunner.Utils
{
    public class NamedGraph<TNode>
    {
        public IGraph<TNode> Graph { get; }

        public string Name { get; }

        public NamedGraph(IGraph<TNode> graph, string name = "Graph")
        {
            Graph = graph;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class NamedGraph : NamedGraph<int>
    {
        public NamedGraph(IGraph<int> graph, string name = "Graph") : base(graph, name)
        {
            /* empty */
        }
    }
}