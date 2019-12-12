namespace MapGeneration.MetaOptimization
{
    public interface IGeneratorEvaluation<TStats>
    {
        TStats GetAverageStatistics(DataSplit dataSplit);
    }
}