using System.Linq;
using MapGeneration.Core.ChainDecompositions;
using MapGeneration.Interfaces.Core.ChainDecompositions;
using MapGeneration.Interfaces.Utils;
using MapGeneration.MetaOptimization.Configurations;

namespace MapGeneration.MetaOptimization.Mutations.ChainMerge
{
    public class ChainMergeMutation<TConfiguration, TNode> : IMutation<TConfiguration>
        where TConfiguration : IChainDecompositionConfiguration<TNode>, ISmartCloneable<TConfiguration>
    {
        public int Priority { get; }

        public int FirstChainNumber { get; }

        public int SecondChainNumber { get; }

        public ChainMergeMutation(int priority, int firstChainNumber, int secondChainNumber)
        {
            Priority = priority;
            FirstChainNumber = firstChainNumber;
            SecondChainNumber = secondChainNumber;
        }
        public TConfiguration Apply(TConfiguration configuration)
        {
            var counter = 0;
            var copy = configuration.SmartClone();
            copy.Chains[FirstChainNumber].Nodes.AddRange(copy.Chains[SecondChainNumber].Nodes);
            copy.Chains = copy.Chains
                .Where(x => x.Number != SecondChainNumber)
                .Select(x => new Chain<TNode>(x.Nodes, counter++))
                .Cast<IChain<TNode>>()
                .ToList();
            return copy;
        }

        public override string ToString()
        {
            return $"ChainMerge {FirstChainNumber} with {SecondChainNumber}, priority {Priority}";
        }
    }
}