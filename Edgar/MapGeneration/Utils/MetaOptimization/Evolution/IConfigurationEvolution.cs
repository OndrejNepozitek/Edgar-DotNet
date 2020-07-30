namespace MapGeneration.MetaOptimization.Evolution
{
    public interface IConfigurationEvolution<TConfiguration>
    {
        TConfiguration Evolve(TConfiguration initialConfiguration);
    }
}