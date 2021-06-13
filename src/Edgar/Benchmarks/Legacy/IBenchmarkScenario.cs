using Edgar.Benchmarks.Interfaces;

namespace Edgar.Benchmarks.Legacy
{
    public interface IBenchmarkScenario<in TInput>
    {
        string Name { get; }

        IGeneratorRunner GetRunnerFor(TInput input);
    }
}