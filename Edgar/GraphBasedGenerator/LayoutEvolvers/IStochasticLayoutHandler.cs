namespace Edgar.GraphBasedGenerator.LayoutEvolvers
{
    public interface IStochasticLayoutHandler<in TLayout>
    {
        void PerturbLayout(TLayout layout);

        bool IsValid(TLayout layout);

        double GetEnergy(TLayout layout);
    }
}