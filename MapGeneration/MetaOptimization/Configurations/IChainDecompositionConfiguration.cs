using System.Collections.Generic;
using MapGeneration.Core.ChainDecompositions;

namespace MapGeneration.MetaOptimization.Configurations
{
    public interface IChainDecompositionConfiguration<TNode>
    {
        List<Chain<TNode>> Chains { get; set; }
    }
}