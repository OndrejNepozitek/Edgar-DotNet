using Edgar.Benchmarks.Interfaces;

namespace Edgar.Benchmarks
{
    /// <summary>
    /// Factory class that creates an instance of an IGeneratorRunner for each level description.
    /// </summary>
    /// <typeparam name="TLevelDescription"></typeparam>
    public class GeneratorRunnerFactory<TLevelDescription> : IGeneratorRunnerFactory<TLevelDescription>
    {
        private readonly GeneratorRunnerFactoryFunc generatorRunnerFactory;

        public GeneratorRunnerFactory(GeneratorRunnerFactoryFunc generatorRunnerFactory)
        {
            this.generatorRunnerFactory = generatorRunnerFactory;
        }

        /// <inheritdoc />
        public IGeneratorRunner GetRunnerFor(TLevelDescription levelDescription)
        {
            return generatorRunnerFactory(levelDescription);
        }

        /// <summary>
        /// Delegate that takes a level description as an input and returns a regenerator runner for that level description.
        /// </summary>
        /// <param name="levelDescription">Level description for which a generator runner should be created.</param>
        /// <returns></returns>
        public delegate IGeneratorRunner GeneratorRunnerFactoryFunc(TLevelDescription levelDescription);
    }
}