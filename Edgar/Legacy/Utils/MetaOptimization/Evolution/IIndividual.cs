namespace Edgar.Legacy.Utils.MetaOptimization.Evolution
{
    public interface IIndividual<out TConfiguration>
    {
        int Id { get; }

        TConfiguration Configuration { get; }

        double Fitness { get; }

        double SuccessRate { get; }
    }
}