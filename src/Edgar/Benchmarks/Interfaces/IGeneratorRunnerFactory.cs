namespace Edgar.Benchmarks.Interfaces
{
    public interface IGeneratorRunnerFactory<in TInput>
    {
        IGeneratorRunner GetRunnerFor(TInput levelDescription);
    }
}