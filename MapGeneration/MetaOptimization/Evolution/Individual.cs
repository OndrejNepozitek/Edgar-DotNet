using System.Collections.Generic;
using System.Linq;
using MapGeneration.MetaOptimization.Mutations;

namespace MapGeneration.MetaOptimization.Evolution
{
    public class Individual<TConfiguration, TConfigurationEvaluation> : IIndividual<TConfiguration>
    {
        public int Id { get; }

        public TConfiguration Configuration { get; }

        public TConfigurationEvaluation ConfigurationEvaluation { get; set; }

        public List<IMutation<TConfiguration>> Mutations { get; }

        public double Fitness { get; set; }
        
        public Individual(int id, Individual<TConfiguration, TConfigurationEvaluation> parent, IMutation<TConfiguration> mutation)
        {
            Id = id;
            Configuration = mutation.Apply(parent.Configuration);
            Mutations = new List<IMutation<TConfiguration>>(parent.Mutations) {mutation};
        }

        public Individual(int id, TConfiguration configuration)
        {
            Id = id;
            Configuration = configuration;
            Mutations = new List<IMutation<TConfiguration>>();
        }

        public override string ToString()
        {
            return $"{Id} ({(Mutations.Count != 0 ? Mutations.Last() : null)})";
        }
    }
}