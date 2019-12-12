using System.Collections.Generic;
using MapGeneration.MetaOptimization.Evolution;

namespace MapGeneration.MetaOptimization.Mutations
{
    public interface IPerformanceAnalyzer<TConfiguration, in TIndividual>
    {
        List<IMutation<TConfiguration>> ProposeMutations(TIndividual individual);
    }
}