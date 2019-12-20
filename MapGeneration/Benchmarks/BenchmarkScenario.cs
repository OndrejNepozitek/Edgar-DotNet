using System;
using MapGeneration.Benchmarks.GeneratorRunners;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.MapLayouts;
using MapGeneration.Interfaces.Benchmarks;
using MapGeneration.Interfaces.Core.LayoutGenerator;
using MapGeneration.Interfaces.Core.MapLayouts;

namespace MapGeneration.Benchmarks
{
    public delegate IGeneratorRunner GeneratorRunnerFactory<TMapDescription>(GeneratorInput<TMapDescription> input);

    public delegate IBenchmarkableLayoutGenerator<TMapDescription, TLayout> GeneratorFactory<TMapDescription, TLayout>(GeneratorInput<TMapDescription> input);

    public class BenchmarkScenario<TMapDescription> : IBenchmarkScenario<GeneratorInput<TMapDescription>>
    {
        public string Name { get; }

        private readonly GeneratorRunnerFactory<TMapDescription> generatorRunnerFactory;

        public BenchmarkScenario(string name, GeneratorRunnerFactory<TMapDescription> generatorRunnerFactory)
        {
            this.generatorRunnerFactory = generatorRunnerFactory ?? throw new ArgumentNullException(nameof(generatorRunnerFactory));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public IGeneratorRunner GetRunnerFor(GeneratorInput<TMapDescription> input)
        {
            return generatorRunnerFactory(input);
        }
    }

    public static class BenchmarkScenario
    {
        public static BenchmarkScenario<MapDescriptionOld<TNode>> CreateForNodeType<TNode>(string name, GeneratorFactory<MapDescriptionOld<TNode>, IMapLayout<TNode>> generatorFactory)
        {
            return new BenchmarkScenario<MapDescriptionOld<TNode>>(name, input =>
            {
                var layoutGenerator = generatorFactory(input);

                return new LambdaGeneratorRunner(() =>
                {
                    var layouts = layoutGenerator.GetLayouts(input.MapDescription, 1);

                    return new GeneratorRun(layouts.Count == 1, layoutGenerator.TimeTotal, layoutGenerator.IterationsCount);
                });
            });
        }

        public static BenchmarkScenario<MapDescriptionOld<TNode>> CreateCustomForNodeType<TNode>(string name, GeneratorRunnerFactory<MapDescriptionOld<TNode>> generatorRunnerFactory)
        {
            return new BenchmarkScenario<MapDescriptionOld<TNode>>(name, generatorRunnerFactory);
        }
    }
}