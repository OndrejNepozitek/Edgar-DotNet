using System.Collections.Generic;

namespace Edgar.Legacy.Utils.MetaOptimization.Evolution
{
    public class Population<TIndividual>
    {
        public List<TIndividual> Individuals { get; }

        public Population()
        {
            Individuals = new List<TIndividual>();
        }

        public Population(List<TIndividual> individuals)
        {
            Individuals = individuals;
        }
    }
}