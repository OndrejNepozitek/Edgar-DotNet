using System.Collections.Generic;

namespace Edgar.Legacy.Utils.MetaOptimization.Mutations
{
    public interface IPerformanceAnalyzer<TConfiguration, in TIndividual>
    {
        List<IMutation<TConfiguration>> ProposeMutations(TIndividual individual);
    }
}