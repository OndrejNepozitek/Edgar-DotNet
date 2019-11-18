using System.Collections.Generic;
using MapGeneration.Interfaces.Core.ChainDecompositions;

namespace MapGeneration.Core.ChainDecompositions
{
    public class Chain<TNode> : IChain<TNode>
    {
        public IList<TNode> Nodes { get; }

        public int Number { get; }

        public Chain(IList<TNode> nodes, int number)
        {
            Nodes = nodes;
            Number = number;
        }
    }
}