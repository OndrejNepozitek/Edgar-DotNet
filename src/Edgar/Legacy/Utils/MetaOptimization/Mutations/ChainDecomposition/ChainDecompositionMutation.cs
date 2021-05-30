using System.Collections.Generic;
using Edgar.GraphBasedGenerator.Common.ChainDecomposition;
using Edgar.GraphBasedGenerator.Common.ChainDecomposition.Legacy;
using Edgar.Legacy.Core.ChainDecompositions;
using Edgar.Legacy.Core.LayoutEvolvers.SimulatedAnnealing;
using Edgar.Legacy.Utils.Interfaces;
using Edgar.Legacy.Utils.MetaOptimization.Configurations;

namespace Edgar.Legacy.Utils.MetaOptimization.Mutations.ChainDecomposition
{
    public class ChainDecompositionMutation<TConfiguration, TNode> : IMutation<TConfiguration>
        where TConfiguration : IChainDecompositionConfiguration<TNode>, ISimulatedAnnealingConfiguration, ISmartCloneable<TConfiguration>
    {
        public int Priority { get; }

        public TreeComponentStrategy TreeComponentStrategy { get; }

        public int MaxTreeSize { get; }

        public bool MergeSmallChains { get; }

        public bool StartTreeWithMultipleVertices { get; }

        public List<Chain<TNode>> Chains { get; }
        
        public ChainDecompositionMutation(int priority, List<Chain<TNode>> chains, int maxTreeSize, bool mergeSmallChains, bool startTreeWithMultipleVertices, TreeComponentStrategy treeComponentStrategy)
        {
            Priority = priority;
            Chains = chains;
            MaxTreeSize = maxTreeSize;
            MergeSmallChains = mergeSmallChains;
            StartTreeWithMultipleVertices = startTreeWithMultipleVertices;
            TreeComponentStrategy = treeComponentStrategy;
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
            return $"ChainDecomposition, priority {Priority}, TreeStrategy {TreeComponentStrategy} MaxTreeSize {MaxTreeSize}, MergeSmallChains {MergeSmallChains}, stwmv {StartTreeWithMultipleVertices}";
        }

        #region Equals

        protected bool Equals(ChainDecompositionMutation<TConfiguration, TNode> other)
        {
            return TreeComponentStrategy == other.TreeComponentStrategy && MaxTreeSize == other.MaxTreeSize && MergeSmallChains == other.MergeSmallChains && StartTreeWithMultipleVertices == other.StartTreeWithMultipleVertices;
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
                var hashCode = (int) TreeComponentStrategy;
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