using MapGeneration.Core.Configurations.Interfaces;
using MapGeneration.Core.Configurations.Interfaces.EnergyData;
using MapGeneration.Utils.Interfaces;

namespace MapGeneration.Core.Configurations
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;

	/// <inheritdoc cref="IEnergyConfiguration{TShapeContainer,TEnergyData}" />
	/// <summary>
	/// Basic implementation of an IEnergyConfiguration interface.
	/// </summary>
	/// <typeparam name="TEnergyData"></typeparam>
	public class Configuration<TEnergyData> : IEnergyConfiguration<IntAlias<GridPolygon>, int, TEnergyData>, ISmartCloneable<Configuration<TEnergyData>>
		where TEnergyData : IEnergyData, ISmartCloneable<TEnergyData>
	{
		/// <inheritdoc cref="IEnergyConfiguration{TShapeContainer,TEnergyData}" />
		public IntAlias<GridPolygon> ShapeContainer { get; set; }

		/// <inheritdoc />
		public GridPolygon Shape => ShapeContainer.Value;

		/// <inheritdoc cref="IEnergyConfiguration{TShapeContainer,TEnergyData}" />
		public IntVector2 Position { get; set; }

		/// <inheritdoc cref="IEnergyConfiguration{TShapeContainer,TEnergyData}" />
		public TEnergyData EnergyData { get; set; }

		/// <inheritdoc cref="IEnergyConfiguration{TShapeContainer,TEnergyData}" />
		public bool IsValid => EnergyData.IsValid;

        public int Node { get; set; }

        public Configuration()
		{
			/* empty */
		}

		public Configuration(IntAlias<GridPolygon> shape, IntVector2 position, TEnergyData energyData, int node)
		{
			ShapeContainer = shape;
			Position = position;
			EnergyData = energyData;
            Node = node;
        }

		public Configuration<TEnergyData> SmartClone()
		{
			return new Configuration<TEnergyData>(
				ShapeContainer,
				Position,
				EnergyData.SmartClone(),
				Node
			);
		}

		public override bool Equals(object obj)
		{
			return obj is Configuration<TEnergyData> configuration &&
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