using System.Collections.Generic;

namespace MapGeneration.MetaOptimization.Evolution
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