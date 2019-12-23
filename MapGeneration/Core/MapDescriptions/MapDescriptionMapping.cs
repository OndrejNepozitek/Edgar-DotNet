using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Graphs;
using MapGeneration.Interfaces.Core.MapDescriptions;

namespace MapGeneration.Core.MapDescriptions
{
    public class MapDescriptionMapping<TNode> : IMapDescription<int>
    {
        private readonly IMapDescription<TNode> mapDescription;
        private readonly TwoWayDictionary<TNode, int> nodeToIntMapping = new TwoWayDictionary<TNode, int>();
        private readonly IRoomDescription[] roomDescriptions;
        private readonly IGraph<int> mappedGraph = new UndirectedAdjacencyListGraph<int>();
        private readonly IGraph<int> mappedStageOneGraph = new UndirectedAdjacencyListGraph<int>();

        public MapDescriptionMapping(IMapDescription<TNode> mapDescription)
        {
            this.mapDescription = mapDescription;
            roomDescriptions = new IRoomDescription[mapDescription.GetGraph().VerticesCount];
            DoMapping();
        }

        private void DoMapping()
        {
            var graph = mapDescription.GetGraph();
            var stageOneGraph = mapDescription.GetStageOneGraph();

            foreach (var vertex in graph.Vertices)
            {
                // Create vertices mapping
                nodeToIntMapping.Add(vertex, nodeToIntMapping.Count);
                mappedGraph.AddVertex(nodeToIntMapping[vertex]);

                // Store room description
                roomDescriptions[nodeToIntMapping[vertex]] = mapDescription.GetRoomDescription(vertex);
            }

            // Handle main graph edges
            foreach (var edge in graph.Edges)
            {
                mappedGraph.AddEdge(nodeToIntMapping[edge.From], nodeToIntMapping[edge.To]);
            }

            // Handle stage one graph vertices
            foreach (var vertex in stageOneGraph.Vertices)
            {
                mappedStageOneGraph.AddVertex(nodeToIntMapping[vertex]);
            }

            // Handle stage one graph edges
            foreach (var edge in stageOneGraph.Edges)
            {
                mappedStageOneGraph.AddEdge(nodeToIntMapping[edge.From], nodeToIntMapping[edge.To]);
            }
        }

        public IGraph<int> GetGraph()
        {
            return mappedGraph;
        }

        public IGraph<int> GetStageOneGraph()
        {
            return mappedStageOneGraph;
        }

        public IRoomDescription GetRoomDescription(int node)
        {
            return roomDescriptions[node];
        }

        public TwoWayDictionary<TNode, int> GetMapping()
        {
            return nodeToIntMapping;
        }
    }
}