using System;
using System.Collections.Generic;
using System.Threading;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common.ChainDecomposition;
using Edgar.Legacy.Core.ChainDecompositions;
using Edgar.Legacy.Core.Configurations.Interfaces;
using Edgar.Legacy.Core.LayoutEvolvers.Interfaces;
using Edgar.Legacy.Core.LayoutOperations.Interfaces;
using Edgar.Legacy.Core.Layouts.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;
using Edgar.Legacy.Utils.Interfaces;

namespace Edgar.Legacy.Core.LayoutEvolvers.PlatformersEvolver
{
    /// <summary>
    /// This evolver is used in the Platformer generator and will be deprecated soon.
    /// </summary>
    /// <typeparam name="TLayout"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    /// <typeparam name="TConfiguration"></typeparam>
    public class PlatformersEvolver<TLayout, TNode, TConfiguration> : ILayoutEvolver<TLayout, TNode>, IRandomInjectable,
        ICancellable
        where TLayout : ILayout<TNode, TConfiguration>, ISmartCloneable<TLayout>
        where TConfiguration : IConfiguration<IntAlias<PolygonGrid2D>, TNode>
    {
        public event EventHandler<TLayout> OnPerturbed;
        public event EventHandler<TLayout> OnValid;

        private Random random;
        private CancellationToken? cancellationToken;
        private readonly IChainBasedLayoutOperations<TLayout, TNode> layoutOperations;

        public PlatformersEvolver(IChainBasedLayoutOperations<TLayout, TNode> layoutOperations)
        {
            this.layoutOperations = layoutOperations;
        }

        public IEnumerable<TLayout> Evolve(TLayout initialLayout, Chain<TNode> chain, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var copy = initialLayout.SmartClone();
                layoutOperations.AddChain(copy, chain.Nodes, true, out var iterationsCount);

                foreach (var _ in chain.Nodes)
                {
                    OnPerturbed?.Invoke(this, copy);
                }

                if (layoutOperations.IsLayoutValid(copy))
                {
                    // TODO: why chain.Nodes instead of chain?
                    if (layoutOperations.TryCompleteChain(copy, chain.Nodes))
                    {
                        yield return copy;
                    }
                }
            }
        }

        public void InjectRandomGenerator(Random random)
        {
            this.random = random;
        }

        public void SetCancellationToken(CancellationToken? cancellationToken)
        {
            this.cancellationToken = cancellationToken;
        }
    }
}