namespace MapGeneration.Core.Experimental
{
	using System.Diagnostics.Contracts;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces;

	public struct Configuration<TEnergyData> : IEnergyConfiguration<Configuration<TEnergyData>, IntAlias<GridPolygon>, TEnergyData>
		where TEnergyData : IEnergyData<TEnergyData>
	{
		public IntAlias<GridPolygon> ShapeContainer { get; }

		public GridPolygon Shape => ShapeContainer.Value;

		public IntVector2 Position { get; }

		public TEnergyData EnergyData { get; }

		public bool IsValid => EnergyData.IsValid;

		public Configuration(IntAlias<GridPolygon> shape, IntVector2 position, TEnergyData energyData)
		{
			ShapeContainer = shape;
			Position = position;
			EnergyData = energyData;
		}

		[Pure]
		public Configuration<TEnergyData> SetShape(IntAlias<GridPolygon> shape)
		{
			// TODO: unify names - shape vs shapeContainer
			return new Configuration<TEnergyData>(
				shape,
				Position,
				EnergyData
			);
		}

		[Pure]
		public Configuration<TEnergyData> SetPosition(IntVector2 position)
		{
			return new Configuration<TEnergyData>(
				ShapeContainer,
				position,
				EnergyData
			);
		}

		[Pure]
		public Configuration<TEnergyData> SetEnergyData(TEnergyData energyData)
		{
			return new Configuration<TEnergyData>(
				ShapeContainer,
				Position,
				energyData
			);
		}
	}
}