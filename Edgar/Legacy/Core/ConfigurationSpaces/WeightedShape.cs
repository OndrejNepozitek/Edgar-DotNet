using System;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;

namespace Edgar.Legacy.Core.ConfigurationSpaces
{
    public class WeightedShape
    {
		public IntAlias<PolygonGrid2D> Shape { get; }

        public double Weight { get; }

        public WeightedShape(IntAlias<PolygonGrid2D> shape, double weight)
        {
            if (weight <= 0)
                throw new ArgumentException("Weight must be greater than zero", nameof(weight));

            Shape = shape;
            Weight = weight;
        }
	}
}