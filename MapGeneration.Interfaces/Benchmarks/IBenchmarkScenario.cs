namespace MapGeneration.Interfaces.Benchmarks
{
    public interface IBenchmarkScenario<in TInput>
    {
        string Name { get; }

        IGeneratorRunner GetRunnerFor(TInput input);
    }
}