namespace Edgar.GraphBasedGenerator.General.Configurations
{
    public interface IEnergyConfiguration<TEnergyData>
    {
        TEnergyData EnergyData { get; set; }
    }
}