using System.Collections.Generic;
using Edgar.GraphBasedGenerator.RoomTemplates;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.Configurations.Interfaces;
using MapGeneration.Core.Configurations.Interfaces.EnergyData;
using MapGeneration.Utils.Interfaces;

namespace Edgar.GraphBasedGenerator.Configurations
{
    /// <inheritdoc cref="IEnergyConfiguration{TShapeContainer,TEnergyData}" />
	/// <summary>
	/// Basic implementation of an IEnergyConfiguration interface.
	/// </summary>
	/// <typeparam name="TEnergyData"></typeparam>
	public class ConfigurationNew<TEnergyData> : /*IEnergyConfiguration<IntAlias<GridPolygon>, int, TEnergyData>,*/ ISmartCloneable<ConfigurationNew<TEnergyData>>, ISimpleEnergyConfiguration<TEnergyData>, IConfiguration<IntAlias<GridPolygon>, IntVector2, int>
		where TEnergyData : IEnergyData, ISmartCloneable<TEnergyData>
	{
        public IntAlias<GridPolygon> ShapeContainer
        {
            get => RoomShape;
            set => RoomShape = value;
        }

        public GridPolygon Shape => ShapeContainer.Value;

		public IntVector2 Position { get; set; }

		public TEnergyData EnergyData { get; set; }

		public bool IsValid => EnergyData.IsValid;

        public int Node
        {
            get => Room;
            set => Room = value;
        }

        public int Room { get; set; }

        public IntAlias<GridPolygon> RoomShape { get; set; }

        public ConfigurationNew()
		{
			/* empty */
		}

		public ConfigurationNew(IntAlias<GridPolygon> shape, IntVector2 position, TEnergyData energyData, int node)
		{
			ShapeContainer = shape;
			Position = position;
			EnergyData = energyData;
            Node = node;
        }

		public ConfigurationNew<TEnergyData> SmartClone()
		{
			return new ConfigurationNew<TEnergyData>(
				ShapeContainer,
				Position,
				EnergyData.SmartClone(),
				Node
			);
		}

		public override bool Equals(object obj)
		{
			return obj is ConfigurationNew<TEnergyData> configuration &&
				   EqualityComparer<IntAlias<GridPolygon>>.Default.Equals(ShapeContainer, configuration.ShapeContainer) &&
				   EqualityComparer<GridPolygon>.Default.Equals(Shape, configuration.Shape) &&
				   Position.Equals(configuration.Position) &&
				   EqualityComparer<TEnergyData>.Default.Equals(EnergyData, configuration.EnergyData) &&
				   IsValid == configuration.IsValid;
		}

		public override int GetHashCode()
		{
			var hashCode = 1750933667;
			hashCode = hashCode * -1521134295 + EqualityComparer<IntAlias<GridPolygon>>.Default.GetHashCode(ShapeContainer);
			hashCode = hashCode * -1521134295 + EqualityComparer<GridPolygon>.Default.GetHashCode(Shape);
			hashCode = hashCode * -1521134295 + EqualityComparer<IntVector2>.Default.GetHashCode(Position);
			hashCode = hashCode * -1521134295 + EqualityComparer<TEnergyData>.Default.GetHashCode(EnergyData);
			hashCode = hashCode * -1521134295 + IsValid.GetHashCode();
			return hashCode;
		}
    }
}