using System;
using System.Collections.Generic;
using System.Linq;
using GeneralAlgorithms.Algorithms.Common;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Interfaces.Core.Configuration;
using MapGeneration.Utils;

namespace MapGeneration.Core.ConfigurationSpaces
{
    public class ConfigurationSpaces<TConfiguration> : AbstractConfigurationSpaces<int, IntAlias<GridPolygon>, TConfiguration>
        where TConfiguration : IConfiguration<IntAlias<GridPolygon>, int>
    {
        protected List<WeightedShape>[] ShapesForNodes;
        protected ConfigurationSpace[][] ConfigurationSpaces_;
        protected TwoWayDictionary<RoomTemplateInstance, IntAlias<GridPolygon>> IntAliasMapping;

        public ConfigurationSpaces(
            List<WeightedShape>[] shapesForNodes,
            ConfigurationSpace[][] configurationSpaces,
            ILineIntersection<OrthogonalLine> lineIntersection, TwoWayDictionary<RoomTemplateInstance, IntAlias<GridPolygon>> intAliasMapping) : base(lineIntersection)
        {
            ShapesForNodes = shapesForNodes;
            ConfigurationSpaces_ = configurationSpaces;
            IntAliasMapping = intAliasMapping;
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
			return ShapesForNodes[node].GetWeightedRandom(x => x.Weight, Random).Shape;
		}

		/// <inheritdoc />
		public override bool CanPerturbShape(int node)
		{
			// We need at least 2 shapes to choose from for it to be perturbed
			return ShapesForNodes[node].Count >= 2;
		}

		/// <inheritdoc />
		public override IReadOnlyCollection<IntAlias<GridPolygon>> GetShapesForNode(int node)
		{
			return ShapesForNodes[node].Select(x => x.Shape).ToList().AsReadOnly();
		}

		/// <inheritdoc />
		public override IEnumerable<IntAlias<GridPolygon>> GetAllShapes()
		{
			var usedShapes = new HashSet<int>();

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

        public TwoWayDictionary<RoomTemplateInstance, IntAlias<GridPolygon>> GetIntAliasMapping()
        {
            return IntAliasMapping;
        }
    }
}