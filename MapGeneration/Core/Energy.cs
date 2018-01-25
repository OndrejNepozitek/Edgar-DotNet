namespace MapGeneration.Core
{
	public struct EnergyData
	{
		public readonly float Energy;
		public readonly int Area;
		public readonly float MoveDistance;

		public EnergyData(float energy, int area, float moveDistance)
		{
			Energy = energy;
			Area = area;
			MoveDistance = moveDistance;
		}
	}
}