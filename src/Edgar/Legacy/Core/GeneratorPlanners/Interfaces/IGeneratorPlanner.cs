using System.Collections.Generic;
using Edgar.GraphBasedGenerator.Common.ChainDecomposition;
using Edgar.Legacy.Core.ChainDecompositions;
using Edgar.Legacy.Core.LayoutEvolvers.Interfaces;

namespace Edgar.Legacy.Core.GeneratorPlanners.Interfaces
{
    /// <summary>
    /// Represents types that can generate a complete level from an initial layout and a set of chains.
    /// </summary>
    public interface IGeneratorPlanner<TLayout, TNode>
    {
        TLayout Generate(TLayout initialLayout, List<Chain<TNode>> chains,
            ILayoutEvolver<TLayout, TNode> layoutEvolver);
    }
}