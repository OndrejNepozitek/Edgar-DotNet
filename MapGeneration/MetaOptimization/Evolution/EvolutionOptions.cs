namespace MapGeneration.MetaOptimization.Evolution
{
    public class EvolutionOptions
    {
        /// <summary>
        /// Maximum number of suggestions applied to the initial configuration.
        /// </summary>
        public int MaxGenerations { get; set; } = 4;

        /// <summary>
        /// Maximum number of individuals after each generation
        /// </summary>
        public int MaxPopulationSize { get; set; } = 5;

        /// <summary>
        /// Maximum depth of the tree of suggestions.
        /// </summary>
        public int MaxMutationsPerIndividual { get; set; } = 3;

        /// <summary>
        /// Minimum priority for a suggestions to be considered.
        /// </summary>
        public int MinPriority { get; set; } = 2;

        /// <summary>
        /// Whether to use the best suggestion from each analyzer even though it is not in the MaxSuggestionsPerStep top suggestions.
        /// </summary>
        public bool UseBestMutationFromEachAnalyzer { get; set; } = true;

        /// <summary>
        /// Whether to allow individuals with less than 100% success rate.
        /// </summary>
        public bool AllowNotPerfectSuccessRate { get; set; } = false;

        /// <summary>
        /// For how many iterations to evaluate each individual.
        /// </summary>
        public int EvaluationIterations { get; set; } = 500;

        public bool WithConsoleOutput { get; set; } = true;
    }
}