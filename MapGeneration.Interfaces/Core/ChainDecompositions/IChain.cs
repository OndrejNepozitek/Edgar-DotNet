using System.Collections.Generic;

namespace MapGeneration.Interfaces.Core.ChainDecompositions
{
    public interface IChain<TNode>
    {
        List<TNode> Nodes { get; }

        int Number { get; }
    }
}