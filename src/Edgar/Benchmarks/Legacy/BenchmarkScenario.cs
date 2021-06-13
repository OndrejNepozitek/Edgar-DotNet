using System;
using Edgar.Benchmarks.Interfaces;

namespace Edgar.Benchmarks.Legacy
{
    public delegate IGeneratorRunner GeneratorRunnerFactory<TLevelDescription>(GeneratorInput<TLevelDescription> input);

    /// <summary>
    /// Benchmark scenario.
    /// </summary>
    /// <typeparam name="TLevelDescription"></typeparam>
    public class BenchmarkScenario<TLevelDescription> : IBenchmarkScenario<GeneratorInput<TLevelDescription>>
    {
        public string Name { get; }

        private readonly GeneratorRunnerFactory<TLevelDescription> generatorRunnerFactory;

        public BenchmarkScenario(string name, GeneratorRunnerFactory<TLevelDescription> generatorRunnerFactory)
        {
            this.generatorRunnerFactory = generatorRunnerFactory ?? throw new ArgumentNullException(nameof(generatorRunnerFactory));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public IGeneratorRunner GetRunnerFor(GeneratorInput<TLevelDescription> input)
        {
            return generatorRunnerFactory(input);
        }
    }
}