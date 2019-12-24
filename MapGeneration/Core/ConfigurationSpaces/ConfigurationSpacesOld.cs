namespace MapGeneration.Core.ConfigurationSpaces
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.Algorithms.Common;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces.Core.Configuration;

	/// <inheritdoc />
	/// <summary>
	/// Basic implementation of configuration spaces.
	/// </summary>
	/// <remarks>
	/// Supports:
	/// - different shapes for different nodes
	/// - different probabilities for a shape to be choosen
	/// - fast retrieval of configuration spaces thanks to IntAlias and nodes being ints
	/// </remarks>
	/// <typeparam name="TConfiguration"></typeparam>
	public class ConfigurationSpacesOld<TConfiguration> : AbstractConfigurationSpaces<int, IntAlias<GridPolygon>, TConfiguration>
		where TConfiguration : IConfiguration<IntAlias<GridPolygon>, int>
	{
		protected List<WeightedShape> Shapes;
		protected List<WeightedShape>[] ShapesForNodes;
		protected ConfigurationSpace[][] ConfigurationSpaces_;

		/// <summary>
		/// ConfigurationSpaces constructor.
		/// </summary>
		/// <param name="shapes">Shapes for nodes that do not have any shapes specified.</param>
		/// <param name="shapesForNodes">Shapes for nodes that should not use default shapes.</param>
		/// <param name="configurationSpaces"></param>
		/// <param name="lineIntersection"></param>
		public ConfigurationSpacesOld(
			List<WeightedShape> shapes,
			List<WeightedShape>[] shapesForNodes,
			ConfigurationSpace[][] configurationSpaces,
			ILineIntersection<OrthogonalLine> lineIntersection) : base(lineIntersection)
		{
			Shapes = shapes;
			ShapesForNodes = shapesForNodes;
			ConfigurationSpaces_ = configurationSpaces;
		}

		/// <inheritdoc />
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

		/// <inheritdoc />
		public override ConfigurationSpace GetConfigurationSpace(TConfiguration mainConfiguration, TConfiguration configuration)
		{
			return GetConfigurationSpace(mainConfiguration.ShapeContainer, configuration.ShapeContainer);
		}

		/// <inheritdoc />
		public override ConfigurationSpace GetConfigurationSpace(IntAlias<GridPolygon> movingPolygon, IntAlias<GridPolygon> fixedPolygon)
		{
			return ConfigurationSpaces_[movingPolygon.Alias][fixedPolygon.Alias];
		}

		/// <inheritdoc />
		/// <summary>
		/// Get random shape for a given node based on probabilities of shapes.
		/// </summary>
		public override IntAlias<GridPolygon> GetRandomShape(int node)
		{
			return (ShapesForNodes[node] ?? Shapes).GetWeightedRandom(x => x.Weight, Random).Shape;
		}

		/// <inheritdoc />
		public override bool CanPerturbShape(int node)
		{
			// We need at least 2 shapes to choose from for it to be perturbed
			return GetShapesForNodeInternal(node).Count >= 2;
		}

		/// <inheritdoc />
		public override IReadOnlyCollection<IntAlias<GridPolygon>> GetShapesForNode(int node)
		{
			return GetShapesForNodeInternal(node).Select(x => x.Shape).ToList().AsReadOnly();
		}

		/// <inheritdoc />
		public override IEnumerable<IntAlias<GridPolygon>> GetAllShapes()
		{
			var usedShapes = new HashSet<int>();

			foreach (var shape in Shapes)
			{
				if (!usedShapes.Contains(shape.Shape.Alias))
				{
					yield return shape.Shape;
					usedShapes.Add(shape.Shape.Alias);
				}
				
			}

			foreach (var shapes in ShapesForNodes)
			{
				if (shapes == null)
					continue;

				foreach (var shape in shapes)
				{
					if (!usedShapes.Contains(shape.Shape.Alias))
					{
						yield return shape.Shape;
						usedShapes.Add(shape.Shape.Alias);
					}
				}
			}
		}

		/// <summary>
		/// Gets shapes for a given node.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		protected IList<WeightedShape> GetShapesForNodeInternal(int node)
		{
			return ShapesForNodes[node] ?? Shapes;
		}

		/// <summary>
		/// Class holding a shape and its probability.
		/// </summary>
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
