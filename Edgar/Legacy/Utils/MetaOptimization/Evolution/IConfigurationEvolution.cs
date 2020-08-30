namespace Edgar.Legacy.Utils.MetaOptimization.Evolution
{
    public interface IConfigurationEvolution<TConfiguration>
    {
        TConfiguration Evolve(TConfiguration initialConfiguration);
    }
}