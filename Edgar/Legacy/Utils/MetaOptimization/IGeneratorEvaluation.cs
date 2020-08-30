namespace Edgar.Legacy.Utils.MetaOptimization
{
    public interface IGeneratorEvaluation<TStats>
    {
        TStats GetAverageStatistics(DataSplit dataSplit);
    }
}