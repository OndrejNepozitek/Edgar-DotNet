namespace MapGeneration.Benchmarks.Interfaces
{
    public interface IBenchmarkScenario<in TInput>
    {
        string Name { get; }

        IGeneratorRunner GetRunnerFor(TInput input);
    }
}