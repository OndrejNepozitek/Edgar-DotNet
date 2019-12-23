using System;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;

namespace MapGeneration.Core.ConfigurationSpaces
{
    public class WeightedShape
    {
		public IntAlias<GridPolygon> Shape { get; }

        public double Weight { get; }

        public WeightedShape(IntAlias<GridPolygon> shape, double weight)
        {
            if (weight <= 0)
                throw new ArgumentException("Weight must be greater than zero", nameof(weight));

            Shape = shape;
            Weight = weight;
        }
	}
}