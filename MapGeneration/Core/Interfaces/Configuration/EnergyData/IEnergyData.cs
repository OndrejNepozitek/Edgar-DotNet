namespace MapGeneration.Core.Interfaces
{
	public interface IEnergyData<out TEnergyData>
	{
		float Energy { get; }

		int Overlap { get; }

		int MoveDistance { get; }

		bool IsValid { get; }

		TEnergyData SetEnergy(float energy);

		TEnergyData SetOverlap(int area);

		TEnergyData SetMoveDistance(int moveDistance);

		TEnergyData SetIsValid(bool isValid);
	}
}