using System.Collections.Generic;
using MapGeneration.Core.ChainDecompositions;
using MapGeneration.Core.LayoutEvolvers.Interfaces;

namespace MapGeneration.Core.GeneratorPlanners.Interfaces
{
    /// <summary>
    /// Represents types that can generate a complete level from an initial layout and a set of chains.
    /// </summary>
    public interface IGeneratorPlanner<TLayout, TNode>
    {
        TLayout Generate(TLayout initialLayout, List<Chain<TNode>> chains, ILayoutEvolver<TLayout, TNode> layoutEvolver);
    }
}