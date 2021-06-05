namespace Edgar.GraphBasedGenerator.RecursiveGrid2D.Internal
{
    public class ClusterEdge<TNode>
    {
        public TNode FromNode { get; }

        public TNode ToNode { get; }

        public Cluster<TNode> ToCluster { get; }

        public ClusterEdge(TNode fromNode, TNode toNode, Cluster<TNode> toCluster)
        {
            FromNode = fromNode;
            ToNode = toNode;
            ToCluster = toCluster;
        }
    }
}