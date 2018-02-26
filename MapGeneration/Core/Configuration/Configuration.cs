namespace MapGeneration.Core.Configuration
{
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces.Core;
	using Interfaces.Core.Configuration;
	using Interfaces.Core.Configuration.EnergyData;

	public class Configuration<TEnergyData> : IEnergyConfiguration<IntAlias<GridPolygon>, TEnergyData>, ISmartCloneable<Configuration<TEnergyData>>
		where TEnergyData : IEnergyData, ISmartCloneable<TEnergyData>
	{
		public IntAlias<GridPolygon> ShapeContainer { get; set; }

		public GridPolygon Shape => ShapeContainer.Value;

		public IntVector2 Position { get; set; }

		public TEnergyData EnergyData { get; set; }

		public bool IsValid => EnergyData.IsValid;

		public Configuration()
		{

		}

		public Configuration(IntAlias<GridPolygon> shape, IntVector2 position, TEnergyData energyData)
		{
			ShapeContainer = shape;
			Position = position;
			EnergyData = energyData;
		}

		public Configuration<TEnergyData> SmartClone()
		{
			return new Configuration<TEnergyData>(
				ShapeContainer,
				Position,
				EnergyData.SmartClone()
			);
		}
	}
}