﻿using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Graphs;
using Edgar.Legacy.Core.MapDescriptions.Interfaces;
using Newtonsoft.Json;

namespace Edgar.Legacy.Core.MapDescriptions
{
    /// <summary>
    /// Description of a level.
    /// </summary>
    /// <typeparam name="TRoom"></typeparam>
    public class MapDescription<TRoom> : IMapDescription<TRoom>
    {
        [JsonProperty]
        private readonly Dictionary<TRoom, IRoomDescription> roomDescriptions =
            new Dictionary<TRoom, IRoomDescription>();

        [JsonProperty] private readonly List<Passage> passages = new List<Passage>();

        /// <summary>
        /// Adds a given room to the level description.
        /// </summary>
        public void AddRoom(TRoom room, IRoomDescription roomDescription)
        {
            if (roomDescription == null) throw new ArgumentNullException(nameof(roomDescription));

            if (roomDescriptions.ContainsKey(room))
            {
                throw new ArgumentException($"Map description already contains node {room}", nameof(room));
            }

            roomDescriptions.Add(room, roomDescription);
        }

        /// <summary>
        /// Adds a connection between given two rooms.
        /// </summary>
        public void AddConnection(TRoom room1, TRoom room2)
        {
            var passage = new Passage(room1, room2);

            if (!roomDescriptions.ContainsKey(room1))
            {
                throw new ArgumentException($"Map description does not contain room {room1}", nameof(room1));
            }

            if (!roomDescriptions.ContainsKey(room2))
            {
                throw new ArgumentException($"Map description does not contain room {room2}", nameof(room2));
            }

            if (passages.Contains(passage))
            {
                throw new ArgumentException("Map description already contains given connection");
            }

            passages.Add(passage);
        }

        /// <summary>
        /// Gets the graph of rooms with all rooms (including corridor rooms)
        /// </summary>
        /// <returns></returns>
        public IGraph<TRoom> GetGraph()
        {
            var graph = new UndirectedAdjacencyListGraph<TRoom>();

            foreach (var room in roomDescriptions.Keys)
            {
                graph.AddVertex(room);
            }

            foreach (var passage in passages)
            {
                graph.AddEdge(passage.Room1, passage.Room2);
            }

            CheckIfValid(graph);

            return graph;
        }

        /// <summary>
        /// Gets a graph of rooms for the first stage of the generate (excluding corridor rooms)
        /// </summary>
        /// <returns></returns>
        public IGraph<TRoom> GetStageOneGraph()
        {
            var graph = GetGraph();
            var stageOneGraph = new UndirectedAdjacencyListGraph<TRoom>();

            foreach (var room in graph.Vertices)
            {
                if (roomDescriptions[room].Stage == 1)
                {
                    stageOneGraph.AddVertex(room);
                }
            }

            foreach (var room in graph.Vertices)
            {
                var roomDescription = roomDescriptions[room];

                if (roomDescription.Stage == 2)
                {
                    var neighbors = graph.GetNeighbors(room).ToList();
                    stageOneGraph.AddEdge(neighbors[0], neighbors[1]);
                }
            }

            foreach (var edge in graph.Edges)
            {
                var roomDescription1 = roomDescriptions[edge.From];
                var roomDescription2 = roomDescriptions[edge.To];

                if (roomDescription1.Stage == 1 && roomDescription2.Stage == 1)
                {
                    stageOneGraph.AddEdge(edge.From, edge.To);
                }
            }

            return stageOneGraph;
        }

        /// <summary>
        /// Get room description of a given room.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IRoomDescription GetRoomDescription(TRoom node)
        {
            return roomDescriptions[node];
        }

        private void CheckIfValid(IGraph<TRoom> graph)
        {
            foreach (var room in graph.Vertices)
            {
                var roomDescription = roomDescriptions[room];

                if (roomDescription.Stage == 2)
                {
                    var neighbors = graph.GetNeighbors(room).ToList();

                    if (neighbors.Count != 2)
                    {
                        throw new ArgumentException(
                            $"Each corridor must have exactly 2 neighbors but room {room} has {neighbors.Count} neighbors");
                    }

                    foreach (var neighbor in neighbors)
                    {
                        var neighborRoomDescription = roomDescriptions[neighbor];

                        if (neighborRoomDescription.Stage == 2)
                        {
                            throw new ArgumentException(
                                $"Each corridor must be connected only to basic rooms but room {room} is connected to room {neighbor} which is a corridors");
                        }
                    }
                }
            }
        }

        private class Passage
        {
            public TRoom Room1 { get; }

            public TRoom Room2 { get; }

            public Passage(TRoom room1, TRoom room2)
            {
                Room1 = room1;
                Room2 = room2;
            }

            #region Equals

            private bool Equals(Passage other)
            {
                return (EqualityComparer<TRoom>.Default.Equals(Room1, other.Room1) &&
                        EqualityComparer<TRoom>.Default.Equals(Room2, other.Room2)) ||
                       (EqualityComparer<TRoom>.Default.Equals(Room1, other.Room2) &&
                        EqualityComparer<TRoom>.Default.Equals(Room2, other.Room1));
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Passage) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (EqualityComparer<TRoom>.Default.GetHashCode(Room1) * 397) ^
                           EqualityComparer<TRoom>.Default.GetHashCode(Room2);
                }
            }

            public static bool operator ==(Passage left, Passage right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(Passage left, Passage right)
            {
                return !Equals(left, right);
            }

            #endregion
        }
    }
}