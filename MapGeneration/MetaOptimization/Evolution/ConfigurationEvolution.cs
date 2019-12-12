using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MapGeneration.MetaOptimization.Mutations;
using MapGeneration.Utils.Logging;
using MapGeneration.Utils.Logging.Handlers;

namespace MapGeneration.MetaOptimization.Evolution
{
    public abstract class ConfigurationEvolution<TConfiguration, TIndividual> : IConfigurationEvolution<TConfiguration>
        where TIndividual : IIndividual<TConfiguration>
    {
        private readonly List<IPerformanceAnalyzer<TConfiguration, TIndividual>> analyzers;
        private readonly EvolutionOptions options;
        private int nextId;
        protected readonly Logger Logger;
        protected readonly string ResultsDirectory;

        protected ConfigurationEvolution(List<IPerformanceAnalyzer<TConfiguration, TIndividual>> analyzers, EvolutionOptions options, string resultsDirectory)
        {
            this.analyzers = analyzers;
            this.options = options;
            ResultsDirectory = resultsDirectory;
            Directory.CreateDirectory(resultsDirectory);
            Logger = new Logger(new ConsoleLoggerHandler(), new FileLoggerHandler($"{resultsDirectory}/log.txt"));
        }

        public TConfiguration Evolve(TConfiguration initialConfiguration)
        {
            nextId = 0;
            var populations = new List<Population<TIndividual>>();

            // Setup initial population
            Logger.WriteLine($"============ Generation 0 ============");
            var initialPopulation = new Population<TIndividual>();
            initialPopulation.Individuals.Add(CreateInitialIndividual(GetNextId(), initialConfiguration));
            populations.Add(initialPopulation);
            EvaluatePopulation(initialPopulation);

            // Evolve configurations
            for (var i = 0; i < options.MaxGenerations; i++)
            {
                Logger.WriteLine($"============ Generation {i + 1} ============");

                var parentPopulation = populations.Last();
                var offspring = ComputeNextGeneration(parentPopulation);

                populations.Add(offspring);

                Logger.WriteLine();
            }

            // Find the best individual
            var allIndividuals = populations.SelectMany(x => x.Individuals).ToList();
            allIndividuals.Sort((x1, x2) => x1.Fitness.CompareTo(x2.Fitness));

            return allIndividuals[0].Configuration;
        }

        public Population<TIndividual> ComputeNextGeneration(Population<TIndividual> parentPopulation)
        {
            var offspringPopulation = new Population<TIndividual>();

            foreach (var parent in parentPopulation.Individuals)
            {
                Logger.WriteLine($">>>> Getting mutations for individual {parent.Id} <<<<");
                // Logger.WriteLine($"-- Configuration {parent.Configuration}");

                var mutations = GetMutations(parent);
                var usedMutations = 0;

                foreach (var mutation in mutations)
                {
                    var individual = CreateIndividual(GetNextId(), parent, mutation);

                    Logger.WriteLine($"Individual {individual.Id}");
                    Logger.WriteLine($"Mutation: {mutation}");
                    Logger.WriteLine($"Configuration: {individual.Configuration}");

                    if (offspringPopulation.Individuals.Any(x => x.Configuration.Equals(individual.Configuration)))
                    {
                        Logger.WriteLine($"Not used - configuration already encountered");
                    }
                    else
                    {
                        offspringPopulation.Individuals.Add(individual);
                    }

                    Logger.WriteLine();
                    usedMutations++;

                    // Break the cycle if we already have enough mutations
                    if (usedMutations >= options.MaxMutationsPerIndividual)
                    {
                        break;
                    }
                }
            }

            EvaluatePopulation(offspringPopulation);

            var bestIndividuals = SelectBestIndividuals(offspringPopulation);

            return bestIndividuals;
        }

        private Population<TIndividual> SelectBestIndividuals(Population<TIndividual> population)
        {
            var individuals = new List<TIndividual>(population.Individuals);
            individuals.Sort((x1, x2) => x1.Fitness.CompareTo(x2.Fitness));

            var bestIndividuals = individuals.Take(options.MaxPopulationSize).ToList();

            return new Population<TIndividual>(bestIndividuals);
        }

        private List<IMutation<TConfiguration>> GetMutations(TIndividual individual)
        {
            var mutations = new List<IMutation<TConfiguration>>();

            foreach (var analyzer in analyzers)
            {
                mutations.AddRange(analyzer.ProposeMutations(individual));
            }

            mutations.Sort((x1, x2) => x1.Priority.CompareTo(x2.Priority));

            return mutations;
        }

        private int GetNextId()
        {
            return nextId++;
        }

        protected virtual Population<TIndividual> EvaluatePopulation(Population<TIndividual> population)
        {
            Logger.WriteLine("**** Evaluating the population ***");
            var evaluatedPopulation = new Population<TIndividual>(population.Individuals.Select(EvaluateIndividual).ToList());
            Logger.WriteLine();

            return evaluatedPopulation;
        }

        protected abstract TIndividual EvaluateIndividual(TIndividual individual);

        protected abstract TIndividual CreateInitialIndividual(int id, TConfiguration configuration);

        protected abstract TIndividual CreateIndividual(int id, TIndividual parent, IMutation<TConfiguration> mutation);
    }
}