namespace MapGeneration.Core.Interfaces.Configuration.EnergyData
{
	using GeneralAlgorithms.DataStructures.Common;

	public interface IValidityVectorEnergyData<TEnergyData> : IEnergyData<TEnergyData>
	{
		SimpleBitVector32 ValidityVector { get; }

		TEnergyData SetValidityVector(SimpleBitVector32 validityVector);
	}
}