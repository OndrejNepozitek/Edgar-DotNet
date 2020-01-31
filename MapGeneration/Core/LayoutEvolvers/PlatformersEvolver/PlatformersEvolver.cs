using System;
using System.Collections.Generic;
using System.Threading;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Interfaces.Core.ChainDecompositions;
using MapGeneration.Interfaces.Core.Configuration;
using MapGeneration.Interfaces.Core.LayoutEvolvers;
using MapGeneration.Interfaces.Core.LayoutOperations;
using MapGeneration.Interfaces.Core.Layouts;
using MapGeneration.Interfaces.Utils;

namespace MapGeneration.Core.LayoutEvolvers.PlatformersEvolver
{
    public class PlatformersEvolver<TLayout, TNode, TConfiguration> : ILayoutEvolver<TLayout, TNode>, IRandomInjectable, ICancellable
        where TLayout : ILayout<TNode, TConfiguration>, ISmartCloneable<TLayout>
        where TConfiguration : IConfiguration<IntAlias<GridPolygon>, TNode>
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

        public IEnumerable<TLayout> Evolve(TLayout initialLayout, IChain<TNode> chain, int count)
        {
            for (int i = 0; i < 10; i++)
            {
                var copy = initialLayout.SmartClone();
                layoutOperations.AddChain(copy, chain.Nodes, true);

                foreach (var chainNode in chain.Nodes)
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