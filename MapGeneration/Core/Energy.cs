namespace MapGeneration.Core
{
	public struct EnergyData
	{
		public readonly float Energy;
		public readonly int Area;

		// We need an int here because there must be no rounding of the value
		public readonly int MoveDistance;

		public EnergyData(float energy, int area, int moveDistance)
		{
			Energy = energy;
			Area = area;
			MoveDistance = moveDistance;
		}
	}
}