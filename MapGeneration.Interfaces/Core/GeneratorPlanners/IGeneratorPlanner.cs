using MapGeneration.Interfaces.Core.ChainDecompositions;
using MapGeneration.Interfaces.Core.LayoutEvolvers;

namespace MapGeneration.Interfaces.Core.GeneratorPlanners
{
    using System.Collections.Generic;

    public interface IGeneratorPlanner<TLayout, TNode>
    {
        TLayout Generate(TLayout initialLayout, IList<IChain<TNode>> chains, ILayoutEvolver<TLayout, TNode> layoutEvolver);
    }
}