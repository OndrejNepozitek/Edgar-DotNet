namespace MapGeneration.MetaOptimization.Evolution
{
    public interface IIndividual<out TConfiguration>
    {
        int Id { get; }

        TConfiguration Configuration { get; }

        double Fitness { get; }
    }
}