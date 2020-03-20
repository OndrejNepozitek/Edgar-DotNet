using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MapGeneration.MetaOptimization.Mutations;
using MapGeneration.Utils.Logging;
using MapGeneration.Utils.Logging.Handlers;
using MapGeneration.Utils.Statistics;

namespace MapGeneration.MetaOptimization.Evolution
{
    public abstract class ConfigurationEvolution<TConfiguration, TIndividual>
        where TIndividual : IIndividual<TConfiguration>
    {
        private int nextId;

        protected readonly EvolutionOptions Options;
        protected readonly Logger Logger;
        protected readonly string ResultsDirectory;
        protected TIndividual InitialIndividual;
        protected readonly List<IPerformanceAnalyzer<TConfiguration, TIndividual>> Analyzers;

        protected event Action OnEvolutionStarted;

        protected ConfigurationEvolution(List<IPerformanceAnalyzer<TConfiguration, TIndividual>> analyzers, EvolutionOptions options, string resultsDirectory)
        {
            this.Analyzers = analyzers;
            this.Options = options;
            ResultsDirectory = resultsDirectory;
            Directory.CreateDirectory(resultsDirectory);

            // TODO: make better
            var loggerHandlers = new List<ILoggerHandler>()
            {
                new FileLoggerHandler($"{resultsDirectory}/log.txt")
            };

            if (options.WithConsoleOutput)
            {
                loggerHandlers.Add(new ConsoleLoggerHandler());
            }

            Logger = new Logger(loggerHandlers.ToArray());
        }

        public EvolutionResult Evolve(TConfiguration initialConfiguration)
        {
            nextId = 0;
            var populations = new List<Population<TIndividual>>();
            var allIndividuals = new List<TIndividual>();

            OnEvolutionStarted?.Invoke();

            // Setup initial population
            Logger.WriteLine($"============ Generation 0 ============");
            InitialIndividual = CreateInitialIndividual(GetNextId(), initialConfiguration);
            var initialPopulation = new Population<TIndividual>();
            initialPopulation.Individuals.Add(InitialIndividual);
            allIndividuals.Add(InitialIndividual);
            populations.Add(initialPopulation);

            Logger.WriteLine($"Initial configuration: {InitialIndividual.Configuration}");
            Logger.WriteLine();
            EvaluatePopulation(initialPopulation);

            // Evolve configurations
            for (var i = 0; i < Options.MaxGenerations; i++)
            {
                Logger.WriteLine($"============ Generation {i + 1} ============");

                var parentPopulation = populations.Last();
                var offspring = ComputeNextGeneration(parentPopulation);
                var bestIndividuals = SelectBestIndividuals(offspring, parentPopulation);

                populations.Add(bestIndividuals);
                allIndividuals.AddRange(offspring.Individuals);

                Logger.WriteLine();
            }

            // Find the best individual
            allIndividuals.Sort((x1, x2) => x1.Fitness.CompareTo(x2.Fitness));

            var bestIndividual = allIndividuals[0];
            var fitnessDifference =
                StatisticsUtils.DifferenceToReference(bestIndividual, InitialIndividual, x => x.Fitness);

            Logger.WriteLine($">>>> Best individual <<<<");
            Logger.WriteLine($"Individual {bestIndividual.Id}");
            Logger.WriteLine($"Configuration: {bestIndividual.Configuration}");
            Logger.WriteLine($"Fitness: {bestIndividual.Fitness:F}, {fitnessDifference:F}% difference");
            
            return new EvolutionResult(bestIndividual.Configuration, allIndividuals);
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

                    if (!Options.AllowRepeatingConfigurations && offspringPopulation.Individuals.Any(x => x.Configuration.Equals(individual.Configuration)))
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
                    if (usedMutations >= Options.MaxMutationsPerIndividual)
                    {
                        break;
                    }
                }
            }

            EvaluatePopulation(offspringPopulation);

            return offspringPopulation;
        }

        protected virtual Population<TIndividual> SelectBestIndividuals(Population<TIndividual> population, Population<TIndividual> previousPopulation)
        {
            var individuals = new List<TIndividual>(population.Individuals);

            if (Options.AddPreviousGenerationWhenComputingNext)
            {
                individuals.AddRange(previousPopulation.Individuals);
            }

            if (!Options.AllowNotPerfectSuccessRate)
            {
                individuals = individuals.Where(x => x.SuccessRate >= 1 - double.Epsilon).ToList();
            }

            if (!Options.AllowWorseThanInitial)
            {
                individuals = individuals.Where(x => x.Fitness < InitialIndividual.Fitness).ToList();
            }

            individuals.Sort((x1, x2) => x1.Fitness.CompareTo(x2.Fitness));

            var bestIndividuals = individuals.Take(Options.MaxPopulationSize).ToList();

            return new Population<TIndividual>(bestIndividuals);
        }

        protected virtual List<IMutation<TConfiguration>> GetMutations(TIndividual individual)
        {
            var mutations = new List<IMutation<TConfiguration>>();

            foreach (var analyzer in Analyzers)
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

        public class EvolutionResult
        {
            public TConfiguration BestConfiguration { get; }

            public List<TIndividual> AllIndividuals { get; }

            public EvolutionResult(TConfiguration bestConfiguration, List<TIndividual> allIndividuals)
            {
                BestConfiguration = bestConfiguration;
                AllIndividuals = allIndividuals;
            }
        }
    }
}