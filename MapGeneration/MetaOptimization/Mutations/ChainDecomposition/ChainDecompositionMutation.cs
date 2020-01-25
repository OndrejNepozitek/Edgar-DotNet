using System.Collections.Generic;
using System.Linq;
using MapGeneration.Core.ChainDecompositions;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Interfaces.Core.ChainDecompositions;
using MapGeneration.Interfaces.Utils;
using MapGeneration.MetaOptimization.Configurations;

namespace MapGeneration.MetaOptimization.Mutations.ChainDecomposition
{
    public class ChainDecompositionMutation<TConfiguration, TNode> : IMutation<TConfiguration>
        where TConfiguration : IChainDecompositionConfiguration<TNode>, ISimulatedAnnealingConfiguration, ISmartCloneable<TConfiguration>
    {
        public int Priority { get; }

        public bool UseDfsTrees { get; }

        public int MaxTreeSize { get; }

        public bool MergeSmallChains { get; }

        public bool StartTreeWithMultipleVertices { get; }

        public List<IChain<TNode>> Chains { get; }
        
        public ChainDecompositionMutation(int priority, List<IChain<TNode>> chains, bool useDfsTrees, int maxTreeSize, bool mergeSmallChains, bool startTreeWithMultipleVertices)
        {
            Priority = priority;
            Chains = chains;
            UseDfsTrees = useDfsTrees;
            MaxTreeSize = maxTreeSize;
            MergeSmallChains = mergeSmallChains;
            StartTreeWithMultipleVertices = startTreeWithMultipleVertices;
        }
        public TConfiguration Apply(TConfiguration configuration)
        {
            var newConfigurations = new List<SimulatedAnnealingConfiguration>();
            for (int i = 0; i < Chains.Count; i++)
            {
                newConfigurations.Add(SimulatedAnnealingConfiguration.GetDefaultConfiguration());
            }

            var copy = configuration.SmartClone();
            copy.Chains = Chains;
            copy.SimulatedAnnealingConfiguration = new SimulatedAnnealingConfigurationProvider(newConfigurations);
            return copy;
        }

        public override string ToString()
        {
            return $"ChainDecomposition, priority {Priority}, MaxTreeSize {MaxTreeSize}, MergeSmallChains {MergeSmallChains}, stwmv {StartTreeWithMultipleVertices}";
        }

        #region Equals

        protected bool Equals(ChainDecompositionMutation<TConfiguration, TNode> other)
        {
            return UseDfsTrees == other.UseDfsTrees && MaxTreeSize == other.MaxTreeSize && MergeSmallChains == other.MergeSmallChains && StartTreeWithMultipleVertices == other.StartTreeWithMultipleVertices;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ChainDecompositionMutation<TConfiguration, TNode>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = UseDfsTrees.GetHashCode();
                hashCode = (hashCode * 397) ^ MaxTreeSize;
                hashCode = (hashCode * 397) ^ MergeSmallChains.GetHashCode();
                hashCode = (hashCode * 397) ^ StartTreeWithMultipleVertices.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(ChainDecompositionMutation<TConfiguration, TNode> left, ChainDecompositionMutation<TConfiguration, TNode> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ChainDecompositionMutation<TConfiguration, TNode> left, ChainDecompositionMutation<TConfiguration, TNode> right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}