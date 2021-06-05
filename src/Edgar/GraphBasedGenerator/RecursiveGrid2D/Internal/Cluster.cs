using System.Collections.Generic;

namespace Edgar.GraphBasedGenerator.RecursiveGrid2D.Internal
{
    public class Cluster<TNode>
    {
        public List<TNode> Nodes { get; }

        public List<ClusterEdge<TNode>> Edges { get; } = new List<ClusterEdge<TNode>>();

        public Cluster(List<TNode> nodes)
        {
            Nodes = nodes;
        }
    }
}