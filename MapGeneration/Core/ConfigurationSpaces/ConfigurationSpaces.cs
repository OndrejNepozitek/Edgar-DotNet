namespace MapGeneration.Core.ConfigurationSpaces
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.Algorithms.Common;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces;
	using Interfaces.Configuration;

	public class ConfigurationSpaces<TConfiguration> : AbstractConfigurationSpaces<int, IntAlias<GridPolygon>, TConfiguration>
		where TConfiguration : IConfiguration<IntAlias<GridPolygon>>
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

		protected override IList<Tuple<TConfiguration, ConfigurationSpace>> GetConfigurationSpaces(TConfiguration mainConfiguration, IList<TConfiguration> configurations)
		{
			var spaces = new List<Tuple<TConfiguration, ConfigurationSpace>>();
			var chosenSpaces = ConfigurationSpaces_[mainConfiguration.ShapeContainer.Alias];

			foreach (var configuration in configurations)
			{
				spaces.Add(Tuple.Create(configuration, chosenSpaces[configuration.ShapeContainer.Alias]));
			}

			return spaces;
		}

		protected override ConfigurationSpace GetConfigurationSpace(TConfiguration mainConfiguration, TConfiguration configuration)
		{
			return GetConfigurationSpace(mainConfiguration.ShapeContainer, configuration.ShapeContainer);
		}

		public override ConfigurationSpace GetConfigurationSpace(IntAlias<GridPolygon> shape1, IntAlias<GridPolygon> shape2)
		{
			return ConfigurationSpaces_[shape1.Alias][shape2.Alias];
		}

		public override IntAlias<GridPolygon> GetRandomShape(int node)
		{
			return (ShapesForNodes[node] ?? Shapes).GetWeightedRandom(x => x.Weight, Random).Shape;
		}

		public override bool CanPerturbShape(int node)
		{
			// We need at least 2 shapes to choose from for it to be perturbed
			return GetShapesForNodeInternal(node).Count >= 2;
		}

		public override IReadOnlyCollection<IntAlias<GridPolygon>> GetShapesForNode(int node)
		{
			return GetShapesForNodeInternal(node).Select(x => x.Shape).ToList().AsReadOnly();
		}

		public override IEnumerable<IntAlias<GridPolygon>> GetAllShapes()
		{
			foreach (var shape in Shapes)
			{
				yield return shape.Shape;
			}

			foreach (var shapes in ShapesForNodes)
			{
				if (shapes == null)
					continue;

				foreach (var shape in Shapes)
				{
					yield return shape.Shape;
				}
			}
		}

		protected IList<WeightedShape> GetShapesForNodeInternal(int node)
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
