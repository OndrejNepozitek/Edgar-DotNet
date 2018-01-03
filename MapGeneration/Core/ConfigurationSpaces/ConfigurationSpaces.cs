namespace MapGeneration.Core.ConfigurationSpaces
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.Algorithms.Common;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;

	public class ConfigurationSpaces : AbstractConfigurationSpaces<int, IntAlias<GridPolygon>, Configuration>
	{
		protected List<WeightedShape> Shapes;
		protected List<WeightedShape>[] ShapesForNodes;
		protected ConfigurationSpace[][] ConfigurationSpaces_;

		public ConfigurationSpaces(
			List<WeightedShape> shapes,
			List<WeightedShape>[] shapesForNodes,
			ConfigurationSpace[][] configurationSpaces,
			ILineIntersection<OrthogonalLine> lineIntersection) : base(lineIntersection)
		{
			Shapes = shapes;
			ShapesForNodes = shapesForNodes;
			ConfigurationSpaces_ = configurationSpaces;
		}

		protected override IList<Tuple<Configuration, ConfigurationSpace>> GetConfigurationSpaces(Configuration mainConfiguration, IList<Configuration> configurations)
		{
			var spaces = new List<Tuple<Configuration, ConfigurationSpace>>();
			var chosenSpaces = ConfigurationSpaces_[mainConfiguration.ShapeContainer.Alias];

			foreach (var configuration in configurations)
			{
				spaces.Add(Tuple.Create(configuration, chosenSpaces[configuration.ShapeContainer.Alias]));
			}

			return spaces;
		}

		protected override ConfigurationSpace GetConfigurationSpace(Configuration mainConfiguration, Configuration configurations)
		{
			return ConfigurationSpaces_[mainConfiguration.ShapeContainer.Alias][configurations.ShapeContainer.Alias];
		}

		public override IntAlias<GridPolygon> GetRandomShape(int node)
		{
			return (ShapesForNodes[node] ?? Shapes).GetWeightedRandom(x => x.Weight, Random).Shape;
		}

		public override bool CanPerturbShape(int node)
		{
			// We need at least 2 shapes to choose from for it to be perturbed
			return GetShapesForNode(node).Count >= 2;
		}

		public override IReadOnlyCollection<IntAlias<GridPolygon>> GetAllShapes(int node)
		{
			return GetShapesForNode(node).Select(x => x.Shape).ToList().AsReadOnly();
		}

		protected IList<WeightedShape> GetShapesForNode(int node)
		{
			return ShapesForNodes[node] ?? Shapes;
		}

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
}
