using System.Collections.Generic;
using Edgar.Legacy.Core.ChainDecompositions;

namespace Edgar.Legacy.Utils.MetaOptimization.Configurations
{
    public interface IChainDecompositionConfiguration<TNode>
    {
        ChainDecompositionConfiguration ChainDecompositionConfiguration { get; set; }

        List<Chain<TNode>> Chains { get; set; }
    }
}