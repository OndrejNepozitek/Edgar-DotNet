﻿using System;
using System.Collections.Generic;
using System.Linq;
using GeneralAlgorithms.Algorithms.Common;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.Configurations.Interfaces;
using MapGeneration.Core.MapDescriptions;

namespace MapGeneration.Core.ConfigurationSpaces
{
    public class ConfigurationSpaces<TConfiguration> : AbstractConfigurationSpaces<int, IntAlias<GridPolygon>, TConfiguration>
        where TConfiguration : IConfiguration<IntAlias<GridPolygon>, int>
    {
        private readonly Func<TConfiguration, TConfiguration, int> configurationSpaceSelector;
        protected List<List<WeightedShape>> ShapesForNodes;
        protected ConfigurationSpace[][][] ConfigurationSpaces_;
        protected TwoWayDictionary<RoomTemplateInstance, IntAlias<GridPolygon>> IntAliasMapping = new TwoWayDictionary<RoomTemplateInstance, IntAlias<GridPolygon>>();

        public ConfigurationSpaces(
            ILineIntersection<OrthogonalLine> lineIntersection, int roomTemplateInstancesCount, int nodesCount, Func<TConfiguration, TConfiguration, int> configurationSpaceSelector) : base(lineIntersection)
        {
            this.configurationSpaceSelector = configurationSpaceSelector;
            // Init configuration spaces array
			ConfigurationSpaces_ = new ConfigurationSpace[roomTemplateInstancesCount][][];
            for (var i = 0; i < roomTemplateInstancesCount; i++)
            {
                ConfigurationSpaces_[i] = new ConfigurationSpace[roomTemplateInstancesCount][];
            }

			// Init shapes for node lists
			ShapesForNodes = new List<List<WeightedShape>>(nodesCount);
            for (var i = 0; i < nodesCount; i++)
            {
                ShapesForNodes.Add(new List<WeightedShape>());
            }
        }

		/// <inheritdoc />
		protected override List<Tuple<TConfiguration, ConfigurationSpace>> GetConfigurationSpaces(TConfiguration mainConfiguration, List<TConfiguration> configurations)
		{
			var spaces = new List<Tuple<TConfiguration, ConfigurationSpace>>();
			var chosenSpaces = ConfigurationSpaces_[mainConfiguration.ShapeContainer.Alias];

			foreach (var configuration in configurations)
			{
				spaces.Add(Tuple.Create(configuration, chosenSpaces[configuration.ShapeContainer.Alias][configurationSpaceSelector(mainConfiguration, configuration)]));
			}

			return spaces;
		}

		/// <inheritdoc />
		public override ConfigurationSpace GetConfigurationSpace(TConfiguration mainConfiguration, TConfiguration configuration)
		{
            return ConfigurationSpaces_[mainConfiguration.ShapeContainer.Alias][configuration.ShapeContainer.Alias][configurationSpaceSelector(mainConfiguration, configuration)];
		}

		/// <inheritdoc />
		public override ConfigurationSpace GetConfigurationSpace(IntAlias<GridPolygon> movingPolygon, IntAlias<GridPolygon> fixedPolygon)
		{
			throw new InvalidOperationException();
			return ConfigurationSpaces_[movingPolygon.Alias][fixedPolygon.Alias][0]; // TODO: is this ok?
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

        public void AddConfigurationSpace(RoomTemplateInstance roomTemplateInstance1, RoomTemplateInstance roomTemplateInstance2, ConfigurationSpace[] configurationSpace)
        {
            var alias1 = GetRoomTemplateInstanceAlias(roomTemplateInstance1);
            var alias2 = GetRoomTemplateInstanceAlias(roomTemplateInstance2);

            ConfigurationSpaces_[alias1.Alias][alias2.Alias] = configurationSpace;
        }

        public void AddShapeForNode(int node, RoomTemplateInstance roomTemplateInstance, double probability)
        {
            var alias = GetRoomTemplateInstanceAlias(roomTemplateInstance);

            ShapesForNodes[node].Add(new WeightedShape(alias, probability));
        }

        private IntAlias<GridPolygon> GetRoomTemplateInstanceAlias(RoomTemplateInstance roomTemplateInstance)
        {
            if (IntAliasMapping.TryGetValue(roomTemplateInstance, out var alias))
            {
                return alias;
            }

			var newAlias = new IntAlias<GridPolygon>(IntAliasMapping.Count, roomTemplateInstance.RoomShape); 

			IntAliasMapping.Add(roomTemplateInstance, newAlias);

            return newAlias;
        }
    }
}