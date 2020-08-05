namespace Edgar.GraphBasedGenerator.Configurations
{
    /// <summary>
    /// TODO: rename later
    /// </summary>
    /// <typeparam name="TEnergyData"></typeparam>
    public interface ISimpleEnergyConfiguration<TEnergyData>
    {
        TEnergyData EnergyData { get; set; }
    }
}