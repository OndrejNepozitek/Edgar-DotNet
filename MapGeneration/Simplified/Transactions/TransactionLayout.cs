using System;
using System.Collections.Generic;
using System.Linq;
using GeneralAlgorithms.DataStructures.Graphs;
using MapGeneration.Core.Layouts.Interfaces;

namespace MapGeneration.Simplified.Transactions
{
    public class TransactionLayout<TRoom> : ILayout<TRoom, SimpleConfiguration<TRoom>>
    {
        private UndirectedAdjacencyListGraph<TRoom> graph = new UndirectedAdjacencyListGraph<TRoom>();

        public IGraph<TRoom> Graph => graph;

        private Dictionary<TRoom, SimpleConfiguration<TRoom>> configurations = new Dictionary<TRoom, SimpleConfiguration<TRoom>>();

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

        public Transaction BeginChanges()
        {
            return new Transaction(this);
        }

        public class Transaction : IDisposable
        {
            private readonly TransactionLayout<TRoom> layout;
            private readonly UndirectedAdjacencyListGraph<TRoom> graphBackup;
            private readonly Dictionary<TRoom, SimpleConfiguration<TRoom>> configurationsBackup;
            private bool committed;

            public Transaction(TransactionLayout<TRoom> layout)
            {
                this.layout = layout;
                graphBackup = new UndirectedAdjacencyListGraph<TRoom>(layout.graph);
                configurationsBackup = new Dictionary<TRoom, SimpleConfiguration<TRoom>>(layout.configurations);
            }

            public void Rollback()
            {
                layout.graph = graphBackup;
                layout.configurations = configurationsBackup;
            }

            public void Commit()
            {
                committed = true;
            }

            public void Dispose()
            {
                if (!committed)
                {
                    Rollback();
                }
            }
        }
    }
}