using System;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Interfaces.Benchmarks;
using MapGeneration.Interfaces.Core.LayoutGenerator;
using MapGeneration.Interfaces.Core.MapLayouts;

namespace MapGeneration.Benchmarks
{
    public class BenchmarkScenario<TMapDescription, TLayout> : IBenchmarkScenario<GeneratorInput<TMapDescription>, TMapDescription, TLayout>
    {
        public string Name { get; }

        private readonly Func<GeneratorInput<TMapDescription>, IBenchmarkableLayoutGenerator<TMapDescription, TLayout>> generatorFactory;

        public BenchmarkScenario(string name, Func<GeneratorInput<TMapDescription>, IBenchmarkableLayoutGenerator<TMapDescription, TLayout>> generatorFactory)
        {
            this.generatorFactory = generatorFactory ?? throw new ArgumentNullException(nameof(generatorFactory));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public IBenchmarkableLayoutGenerator<TMapDescription, TLayout> GetGeneratorFor(GeneratorInput<TMapDescription> input)
        {
            return generatorFactory(input);
        }
    }

    public static class BenchmarkScenario
    {
        public static BenchmarkScenario<MapDescription<TNode>, IMapLayout<TNode>> CreateForNodeType<TNode>(string name,
            Func<GeneratorInput<MapDescription<TNode>>, IBenchmarkableLayoutGenerator<MapDescription<TNode>, IMapLayout<TNode>>> generatorFactory)
        {
            return new BenchmarkScenario<MapDescription<TNode>, IMapLayout<TNode>>(name, generatorFactory);
        }
    }
}