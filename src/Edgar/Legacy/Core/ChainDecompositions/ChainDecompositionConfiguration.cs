using Edgar.GraphBasedGenerator.Common.ChainDecomposition;
using Edgar.Legacy.Utils.Interfaces;

namespace Edgar.Legacy.Core.ChainDecompositions
{
    /// <summary>
    /// Chain decomposition configuration.
    /// </summary>
    public class ChainDecompositionConfiguration : ISmartCloneable<ChainDecompositionConfiguration>
    {
        public int MaxTreeSize { get; set; } = 8;

        public bool MergeSmallChains { get; set; } = true;

        public bool StartTreeWithMultipleVertices { get; set; } = true;

        public TreeComponentStrategy TreeComponentStrategy { get; set; } = TreeComponentStrategy.BreadthFirst;

        public bool PreferSmallCycles { get; set; } = true;

        public ChainDecompositionConfiguration SmartClone()
        {
            return new ChainDecompositionConfiguration()
            {
                MaxTreeSize = MaxTreeSize,
                MergeSmallChains = MergeSmallChains,
                StartTreeWithMultipleVertices = StartTreeWithMultipleVertices,
                TreeComponentStrategy = TreeComponentStrategy,
                PreferSmallCycles = PreferSmallCycles,
            };
        }
    }
}