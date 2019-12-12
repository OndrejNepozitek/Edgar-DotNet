namespace MapGeneration.Interfaces.Benchmarks
{
    public interface IGeneratorRun
    {
        bool IsSuccessful { get; }

        double Time { get; }

        int Iterations { get; }
    }

    public interface IGeneratorRun<TAdditionalData> : IGeneratorRun
    {
        TAdditionalData AdditionalData { get; }
    }
}