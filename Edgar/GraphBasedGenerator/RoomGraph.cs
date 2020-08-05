using System;
using System.Collections.Generic;
using GeneralAlgorithms.DataStructures.Graphs;

namespace Edgar.GraphBasedGenerator
{
    public class RoomGraph<TRoom> : IGraph<RoomNode<TRoom>>
    {
        public bool IsDirected => false;

        public IEnumerable<RoomNode<TRoom>> Vertices { get; }

        public IEnumerable<IEdge<RoomNode<TRoom>>> Edges { get; }

        public int VerticesCount { get; }

        public RoomGraph(IGraph<RoomNode<TRoom>> originalGraph)
        {
            Initialize(originalGraph);
        }

        private void Initialize(IGraph<RoomNode<TRoom>> originalGraph)
        {

        }

        public void AddVertex(RoomNode<TRoom> vertex)
        {
            throw new NotSupportedException("The graph cannot be modified once constructed");
        }

        public void AddEdge(RoomNode<TRoom> from, RoomNode<TRoom> to)
        {
            throw new NotSupportedException("The graph cannot be modified once constructed");
        }

        public IEnumerable<RoomNode<TRoom>> GetNeighbours(RoomNode<TRoom> vertex)
        {
            throw new NotImplementedException();
        }

        public bool HasEdge(RoomNode<TRoom> from, RoomNode<TRoom> to)
        {
            throw new NotImplementedException();
        }
    }
}