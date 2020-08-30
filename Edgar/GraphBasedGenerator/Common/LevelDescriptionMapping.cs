using Edgar.Graphs;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using IRoomDescription = Edgar.GraphBasedGenerator.Common.RoomTemplates.IRoomDescription;

namespace Edgar.GraphBasedGenerator.Common
{
    /// <summary>
    /// Mapping from a generic map description to an integer version in order to improve performance.
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    public class LevelDescriptionMapping<TNode> : ILevelDescription<RoomNode<TNode>>
    {
        private readonly ILevelDescription<TNode> levelDescription;
        private readonly TwoWayDictionary<TNode, RoomNode<TNode>> nodeToIntMapping = new TwoWayDictionary<TNode, RoomNode<TNode>>();
        private readonly IRoomDescription[] roomDescriptions;
        private readonly IGraph<RoomNode<TNode>> mappedGraph = new UndirectedAdjacencyListGraph<RoomNode<TNode>>();
        private readonly IGraph<RoomNode<TNode>> mappedStageOneGraph = new UndirectedAdjacencyListGraph<RoomNode<TNode>>();

        public LevelDescriptionMapping(ILevelDescription<TNode> levelDescription)
        {
            this.levelDescription = levelDescription;
            roomDescriptions = new IRoomDescription[levelDescription.GetGraph().VerticesCount];
            DoMapping();
        }

        private void DoMapping()
        {
            var graph = levelDescription.GetGraph();
            var stageOneGraph = levelDescription.GetGraphWithoutCorridors();

            foreach (var vertex in graph.Vertices)
            {
                var roomNode = CreateRoomNode(vertex);

                // Create vertices mapping
                mappedGraph.AddVertex(roomNode);

                // Store room description
                roomDescriptions[roomNode.Id] = levelDescription.GetRoomDescription(vertex);
            }

            // Handle main graph edges
            foreach (var edge in graph.Edges)
            {
                mappedGraph.AddEdge(GetRoomNode(edge.From), GetRoomNode(edge.To));
            }

            // Handle stage one graph vertices
            foreach (var vertex in stageOneGraph.Vertices)
            {
                mappedStageOneGraph.AddVertex(GetRoomNode(vertex));
            }

            // Handle stage one graph edges
            foreach (var edge in stageOneGraph.Edges)
            {
                mappedStageOneGraph.AddEdge(GetRoomNode(edge.From), GetRoomNode(edge.To));
            }
        }

        private RoomNode<TNode> GetRoomNode(TNode node)
        {
            return nodeToIntMapping[node];
        }

        private RoomNode<TNode> CreateRoomNode(TNode node)
        {
            var roomNode = new RoomNode<TNode>(nodeToIntMapping.Count, node);
            nodeToIntMapping.Add(node, roomNode);

            return roomNode;
        }

        public IGraph<RoomNode<TNode>> GetGraph()
        {
            return mappedGraph;
        }

        public IGraph<RoomNode<TNode>> GetGraphWithoutCorridors()
        {
            return mappedStageOneGraph;
        }

        public IRoomDescription GetRoomDescription(RoomNode<TNode> node)
        {
            return roomDescriptions[node.Id];
        }

        public IGraph<RoomNode<TNode>> GetStageOneGraph()
        {
            return mappedStageOneGraph;
        }

        public TwoWayDictionary<TNode, RoomNode<TNode>> GetMapping()
        {
            return nodeToIntMapping;
        }
    }
}