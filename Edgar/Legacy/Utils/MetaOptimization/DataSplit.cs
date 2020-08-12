namespace Edgar.Legacy.Utils.MetaOptimization
{
    public class DataSplit
    {
        public double Start { get; }

        public double End { get; }

        public DataSplit(double start, double end)
        {
            Start = start;
            End = end;
        }
    }
}