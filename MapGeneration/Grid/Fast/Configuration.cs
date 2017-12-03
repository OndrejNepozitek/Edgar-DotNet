namespace MapGeneration.Grid.Fast
{
	using System.Collections.Specialized;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces;

	public struct Configuration : IConfiguration<GridPolygon, IntVector2>
	{
		public readonly GridPolygon Polygon;
		public readonly IntVector2 Position;
		public readonly BitVector32 InvalidNeigbours; // 0 = valid, 1 = invalid
		public readonly float Energy;

		public Configuration(GridPolygon polygon, IntVector2 position, int neighboursCount)
		{
			Polygon = polygon;
			Position = position;
			InvalidNeigbours = new BitVector32(0);
			Energy = 0;

			// TODO: too slow?
			for (var i = 0; i < neighboursCount; i++)
			{
				InvalidNeigbours[i] = true;
			}
		}

		public Configuration(GridPolygon polygon, IntVector2 position, float energy, BitVector32 invalidNeigbours)
		{
			Polygon = polygon;
			Position = position;
			Energy = energy;
			InvalidNeigbours = invalidNeigbours;
		}

		public Configuration(GridPolygon polygon, IntVector2 position)
		{
			Polygon = polygon;
			Position = position;
			Energy = 0;
			InvalidNeigbours = new BitVector32(-1);
		}

		public Configuration(Configuration old, BitVector32 invalidNeigbours)
		{
			Polygon = old.Polygon;
			Position = old.Position;
			Energy = old.Energy;
			InvalidNeigbours = invalidNeigbours;
		}

		public Configuration(Configuration old, float energy)
		{
			Polygon = old.Polygon;
			Position = old.Position;
			Energy = energy;
			InvalidNeigbours = old.InvalidNeigbours;
		}

		public bool IsValid()
		{
			return InvalidNeigbours.Data == 0;
		}

		GridPolygon IConfiguration<GridPolygon, IntVector2>.Shape => Polygon;
		IntVector2 IConfiguration<GridPolygon, IntVector2>.Position => Position;
	}
}