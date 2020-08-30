namespace Edgar.GraphBasedGenerator.Common.Configurations
{
    public interface IEnergyConfiguration<TEnergyData>
    {
        TEnergyData EnergyData { get; set; }
    }
}