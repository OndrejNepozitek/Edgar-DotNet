namespace MapGeneration.Interfaces.Core.Configuration.EnergyData
{
	public interface IEnergyDataCorridors : IEnergyData
	{
		int CorridorDistance { get; set; }

		int NumberOfTouching { get; set; }
	}
}