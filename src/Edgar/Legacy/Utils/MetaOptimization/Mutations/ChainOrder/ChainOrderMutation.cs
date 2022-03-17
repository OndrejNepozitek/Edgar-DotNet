using System.Collections.Generic;
using Edgar.GraphBasedGenerator.Common.ChainDecomposition;
using Edgar.Legacy.Core.ChainDecompositions;
using Edgar.Legacy.Utils.Interfaces;
using Edgar.Legacy.Utils.MetaOptimization.Configurations;

namespace Edgar.Legacy.Utils.MetaOptimization.Mutations.ChainOrder
{
    public class ChainOrderMutation<TConfiguration, TNode> : IMutation<TConfiguration>
        where TConfiguration : IChainDecompositionConfiguration<TNode>, ISmartCloneable<TConfiguration>
    {
        public int Priority { get; }

        public int FirstChainNumber { get; }

        public ChainOrderMutation(int priority, int firstChainNumber)
        {
            Priority = priority;
            FirstChainNumber = firstChainNumber;
        }

        public TConfiguration Apply(TConfiguration configuration)
        {
            var counter = 0;
            var copy = configuration.SmartClone();
            var newChains = new List<Chain<TNode>>
            {
                new Chain<TNode>(copy.Chains[FirstChainNumber].Nodes, counter++)
            };


            for (int i = 1; i < copy.Chains.Count; i++)
            {
                var firstIndex = FirstChainNumber - i;
                var secondIndex = FirstChainNumber + i;

                if (firstIndex >= 0)
                {
                    newChains.Add(new Chain<TNode>(copy.Chains[firstIndex].Nodes, counter++));
                }

                if (secondIndex < copy.Chains.Count)
                {
                    newChains.Add(new Chain<TNode>(copy.Chains[secondIndex].Nodes, counter++));
                }
            }

            copy.Chains = newChains;

            return copy;
        }

        public override string ToString()
        {
            return $"ChainOrder {FirstChainNumber}, priority {Priority}";
        }
    }
}