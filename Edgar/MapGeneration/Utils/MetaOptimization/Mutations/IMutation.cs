namespace MapGeneration.MetaOptimization.Mutations
{
    public interface IMutation<TConfiguration>
    {
        int Priority { get; }

        TConfiguration Apply(TConfiguration configuration);
    }
}