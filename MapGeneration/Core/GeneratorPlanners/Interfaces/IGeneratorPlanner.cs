using System.Collections.Generic;
using MapGeneration.Core.ChainDecompositions;
using MapGeneration.Core.LayoutEvolvers.Interfaces;

namespace MapGeneration.Core.GeneratorPlanners.Interfaces
{
    public interface IGeneratorPlanner<TLayout, TNode>
    {
        TLayout Generate(TLayout initialLayout, List<Chain<TNode>> chains, ILayoutEvolver<TLayout, TNode> layoutEvolver);
    }
}