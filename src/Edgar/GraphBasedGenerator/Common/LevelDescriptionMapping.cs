using System;
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
        public string Name => levelDescription.Name;

        private readonly ILevelDescription<TNode> levelDescription;
        private readonly TwoWayDictionary<TNode, RoomNode<TNode>> nodeToIntMapping = new TwoWayDictionary<TNode, RoomNode<TNode>>();
        private readonly IRoomDescription[] roomDescriptions;

        private readonly IGraph<RoomNode<TNode>> mappedGraph = new UndirectedAdjacencyListGraph<RoomNode<TNode>>();
        private readonly IGraph<RoomNode<TNode>> mappedStageOneGraph = new UndirectedAdjacencyListGraph<RoomNode<TNode>>();

        private IGraph<RoomNode<TNode>> directedMappedGraph;
        private IGraph<RoomNode<TNode>> directedMappedStageOneGraph;

        public LevelDescriptionMapping(ILevelDescription<TNode> levelDescription)
        {
            this.levelDescription = levelDescription;
            roomDescriptions = new IRoomDescription[levelDescription.GetGraph().VerticesCount];

            DoMapping(levelDescription.GetGraph(), levelDescription.GetGraphWithoutCorridors(), ref mappedGraph, ref mappedStageOneGraph);
        }

        private void DoMapping(IGraph<TNode> graph, IGraph<TNode> stageOneGraph, ref IGraph<RoomNode<TNode>> mappedGraph, ref IGraph<RoomNode<TNode>> mappedStageOneGraph, bool isDirected = false)
        {
            foreach (var vertex in graph.Vertices)
            {
                var roomNode = GetRoomNode(vertex, true);

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

            if (!isDirected)
            {
                mappedGraph = new UndirectedImmutableGraph<RoomNode<TNode>>(mappedGraph);
                mappedStageOneGraph = new UndirectedImmutableGraph<RoomNode<TNode>>(mappedStageOneGraph);
            }
        }

        private RoomNode<TNode> GetRoomNode(TNode node, bool canCreateNew = false)
        {
            if (nodeToIntMapping.TryGetValue(node, out var roomNode))
            {
                return roomNode;
            }

            if (!canCreateNew)
            {
                throw new InvalidOperationException();
            }

            roomNode = new RoomNode<TNode>(nodeToIntMapping.Count, node);
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

        public IGraph<RoomNode<TNode>> GetGraph(bool withoutCorridors, bool directed)
        {
            if (directed)
            {
                if (directedMappedGraph == null)
                {
                    directedMappedGraph = new BagOfEdgesGraph<RoomNode<TNode>>();
                    directedMappedStageOneGraph = new BagOfEdgesGraph<RoomNode<TNode>>();

                    DoMapping(
                        levelDescription.GetGraph(false, true),
                        levelDescription.GetGraph(true, true),
                        ref directedMappedGraph,
                        ref directedMappedStageOneGraph,
                        true);
                }

                if (withoutCorridors)
                {
                    return directedMappedStageOneGraph;
                }
                else
                {
                    return directedMappedGraph;
                }
            }
            else
            {
                if (withoutCorridors)
                {
                    return GetGraphWithoutCorridors();
                }
                else
                {
                    return GetGraph();
                }
            }
        }

        public IRoomDescription GetRoomDescription(RoomNode<TNode> room)
        {
            return roomDescriptions[room.Id];
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