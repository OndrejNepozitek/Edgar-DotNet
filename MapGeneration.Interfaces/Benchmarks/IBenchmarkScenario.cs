using MapGeneration.Interfaces.Core.LayoutGenerator;

namespace MapGeneration.Interfaces.Benchmarks
{
    public interface IBenchmarkScenario<in TInput, in TMapDescription, TLayout>
    {
        string Name { get; }

        IBenchmarkableLayoutGenerator<TMapDescription, TLayout> GetGeneratorFor(TInput input);
    }
}