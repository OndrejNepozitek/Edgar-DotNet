using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Graphs;
using Edgar.Legacy.Core.Layouts.Interfaces;

namespace Edgar.GraphBasedGenerator.Grid2DSimplified
{
    public class SimpleLayout<TRoom, TConfiguration> : ILayout<TRoom, TConfiguration>
        where TConfiguration : class
    {
        private UndirectedAdjacencyListGraph<TRoom> graph = new UndirectedAdjacencyListGraph<TRoom>();

        private UndirectedAdjacencyListGraph<TRoom> graphWithCorridors = new UndirectedAdjacencyListGraph<TRoom>();

        private Dictionary<TRoom, TConfiguration> configurations = new Dictionary<TRoom, TConfiguration>();

        public IGraph<TRoom> Graph => graph;

        public IGraph<TRoom> GraphWithCorridors => graphWithCorridors;

        public void AddRoom(TRoom room, TConfiguration configuration = null)
        {
            graph.AddVertex(room);
            graphWithCorridors.AddVertex(room);

            if (configuration != null)
            {
                SetConfiguration(room, configuration);
            }
        }

        public void AddConnection(TRoom room1, TRoom room2)
        {
            graph.AddEdge(room1, room2);
            graphWithCorridors.AddEdge(room1, room2);
        }

        public void AddCorridorRoom(TRoom room, TConfiguration configuration = null)
        {
            graphWithCorridors.AddVertex(room);

            if (configuration != null)
            {
                SetConfiguration(room, configuration);
            }
        }

        public void AddCorridorConnection(TRoom room1, TRoom room2, TRoom corridorRoom)
        {
            if (!graphWithCorridors.HasVertex(corridorRoom))
            {
                graphWithCorridors.AddVertex(corridorRoom);
            }

            graph.AddEdge(room1, room2);

            graphWithCorridors.AddEdge(room1, corridorRoom);
            graphWithCorridors.AddEdge(room2, corridorRoom);
        }

        public bool IsCorridor(TRoom room)
        {
            return !graph.HasVertex(room);
        }

        public TConfiguration GetConfiguration(TRoom room)
        {
            if (GetConfiguration(room, out var configuration))
            {
                return configuration;
            }

            return null;
        }

        public bool GetConfiguration(TRoom room, out TConfiguration configuration)
        {
            if (configurations.TryGetValue(room, out configuration))
            {
                return true;
            }

            return false;
        }

        public List<TConfiguration> GetNeighborConfigurations(TRoom room)
        {
            return Graph
                .GetNeighbours(room)
                .Select(GetConfiguration)
                .Where(x => x != null)
                .ToList();
        }

        public void SetConfiguration(TRoom room, TConfiguration configuration)
        {
            configurations[room] = configuration;
        }

        public void RemoveConfiguration(TRoom room)
        {
            configurations.Remove(room);
        }

        public IEnumerable<TConfiguration> GetAllConfigurations()
        {
            return configurations.Values;
        }

        public Transaction BeginChanges(bool autoCommit = false)
        {
            return new Transaction(this, autoCommit);
        }

        public class Transaction : IDisposable
        {
            private readonly SimpleLayout<TRoom, TConfiguration> layout;
            private readonly UndirectedAdjacencyListGraph<TRoom> graphBackup;
            private readonly UndirectedAdjacencyListGraph<TRoom> graphWithCorridorsBackup;
            private readonly Dictionary<TRoom, TConfiguration> configurationsBackup;
            private readonly bool autoCommit;
            private bool committed;

            public Transaction(SimpleLayout<TRoom, TConfiguration> layout, bool autoCommit)
            {
                this.layout = layout;
                this.autoCommit = autoCommit;
                graphBackup = new UndirectedAdjacencyListGraph<TRoom>(layout.graph);
                graphWithCorridorsBackup = new UndirectedAdjacencyListGraph<TRoom>(layout.graphWithCorridors);
                configurationsBackup = new Dictionary<TRoom, TConfiguration>(layout.configurations);
            }

            public void Rollback()
            {
                layout.graph = graphBackup;
                layout.graphWithCorridors = graphWithCorridorsBackup;
                layout.configurations = configurationsBackup;
            }

            public void Commit()
            {
                committed = true;
            }

            public void Dispose()
            {
                if (!autoCommit && !committed)
                {
                    Rollback();
                }
            }
        }
    }
}