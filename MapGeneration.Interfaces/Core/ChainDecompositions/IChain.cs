using System.Collections.Generic;

namespace MapGeneration.Interfaces.Core.ChainDecompositions
{
    public interface IChain<TNode>
    {
        IList<TNode> Nodes { get; }

        int Number { get; }
    }
}