using System.Collections.Generic;
using MapGeneration.Interfaces.Core.ChainDecompositions;

namespace MapGeneration.MetaOptimization
{
    public interface IGeneratorConfig<TNode>
    {
        List<IChain<TNode>> Chains { get; }
    }
}