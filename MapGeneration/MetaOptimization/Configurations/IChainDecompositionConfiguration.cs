using System.Collections.Generic;
using MapGeneration.Interfaces.Core.ChainDecompositions;

namespace MapGeneration.MetaOptimization.Configurations
{
    public interface IChainDecompositionConfiguration<TNode>
    {
        List<IChain<TNode>> Chains { get; set; }
    }
}