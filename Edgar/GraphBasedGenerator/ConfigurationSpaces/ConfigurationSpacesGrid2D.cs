using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.GraphBasedGenerator.Configurations;
using GeneralAlgorithms.Algorithms.Common;
using GeneralAlgorithms.Algorithms.Polygons;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.Configurations.Interfaces;
using MapGeneration.Core.ConfigurationSpaces;
using MapGeneration.Core.ConfigurationSpaces.Interfaces;
using MapGeneration.Core.Doors;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.MapDescriptions.Interfaces;
using MapGeneration.Utils;
using MapGeneration.Utils.Interfaces;

namespace Edgar.GraphBasedGenerator.ConfigurationSpaces
{
    public class ConfigurationSpacesGrid2D<TConfiguration, TNode> : IConfigurationSpaces<TConfiguration, IntVector2>, IRandomInjectable
        where TConfiguration: IConfiguration<RoomTemplateInstance, IntVector2, TNode>
    {
        private readonly IConfigurationSpaces<TNode, IntAlias<GridPolygon>, TConfiguration, ConfigurationSpace> configurationSpaces;
        private readonly ILineIntersection<OrthogonalLine> lineIntersection;
        private readonly IMapDescription<TNode> levelDescription; // TODO: replace with LevelDescription when possible
        private readonly ConfigurationSpacesGenerator configurationSpacesGenerator;
        private Random random;

        private Dictionary<Tuple<TNode, TNode>, CorridorRoomDescription> nodesToCorridorMapping;

        public ConfigurationSpacesGrid2D(IConfigurationSpaces<TNode, IntAlias<GridPolygon>, TConfiguration, ConfigurationSpace> configurationSpaces, IMapDescription<TNode> levelDescription, ILineIntersection<OrthogonalLine> lineIntersection = null)
        {
            this.configurationSpaces = configurationSpaces;
            this.levelDescription = levelDescription;
            this.lineIntersection = lineIntersection ?? new OrthogonalLineIntersection();
            configurationSpacesGenerator = new ConfigurationSpacesGenerator(new PolygonOverlap(), DoorHandler.DefaultHandler, lineIntersection, new GridPolygonUtils());
            Initialize();
        }

        private void Initialize()
        {
            nodesToCorridorMapping = configurationSpacesGenerator.GetNodesToCorridorMapping(levelDescription);
        }

        public bool HaveValidPosition(TConfiguration configuration1, TConfiguration configuration2)
        {
            return configurationSpaces.HaveValidPosition(configuration1, configuration2);
        }

        IConfigurationSpace<IntVector2> IConfigurationSpaces<TConfiguration, IntVector2>.GetConfigurationSpace(TConfiguration configuration1, TConfiguration configuration2)
        {
            return GetConfigurationSpace(configuration1, configuration2);
        }

        public ConfigurationSpaceGrid2D GetConfigurationSpace(TConfiguration configuration1, TConfiguration configuration2)
        {
            // If is over corridor
            if (nodesToCorridorMapping.ContainsKey(new Tuple<TNode, TNode>(configuration1.Room, configuration2.Room)))
            {
                var configurationSpace = configurationSpaces.GetConfigurationSpace(configuration1, configuration2);

                return new ConfigurationSpaceGrid2D(configurationSpace.Lines, configurationSpace.ReverseDoors);
            }
            // Otherwise
            else
            {
                // configurationSpacesGenerator.GetConfigurationSpace(configuration1.Shape, )

                var configurationSpace = configurationSpaces.GetConfigurationSpace(configuration1, configuration2);

                return new ConfigurationSpaceGrid2D(configurationSpace.Lines, configurationSpace.ReverseDoors);
            }
        }

        public IConfigurationSpace<IntVector2> GetMaximumIntersection(TConfiguration mainConfiguration, IEnumerable<TConfiguration> configurations)
        {
            return GetMaximumIntersection(mainConfiguration, configurations, out var _);
        }

        private IList<Tuple<TConfiguration, ConfigurationSpaceGrid2D>> GetConfigurationSpaces(TConfiguration mainConfiguration, IList<TConfiguration> configurations)
        {
            var spaces = new List<Tuple<TConfiguration, ConfigurationSpaceGrid2D>>();

            foreach (var configuration in configurations)
            {
                spaces.Add(Tuple.Create(configuration, GetConfigurationSpace(mainConfiguration, configuration)));
            }

            return spaces;
        }

        public IConfigurationSpace<IntVector2> GetMaximumIntersection(TConfiguration mainConfiguration, IEnumerable<TConfiguration> configurations, out int configurationsSatisfied)
        {
            // TODO: weird ToList()
            var configurationsList = configurations.ToList();

            var spaces = GetConfigurationSpaces(mainConfiguration, configurationsList);
            spaces.Shuffle(random);

            for (var i = configurationsList.Count; i > 0; i--)
            {
                foreach (var indices in configurationsList.GetCombinations(i))
                {
                    List<OrthogonalLine> intersection = null;

                    foreach (var index in indices)
                    {
                        var linesToIntersect = spaces[index].Item2.Lines.Select(x => x + spaces[index].Item1.Position).ToList();
                        intersection = intersection != null
                            ? lineIntersection.GetIntersections(linesToIntersect, intersection)
                            : linesToIntersect;

                        if (intersection.Count == 0)
                        {
                            break;
                        }
                    }

                    if (intersection != null && intersection.Count != 0)
                    {
                        configurationsSatisfied = indices.Length;
                        
                        return new ConfigurationSpaceGrid2D(intersection);
                    }
                }
            }

            configurationsSatisfied = 0;
            return null;
            
            //// TODO: weird ToList()
            //var intersection =
            //    configurationSpaces.GetMaximumIntersection(mainConfiguration, configurations.ToList(),
            //        out configurationsSatisfied);

            //// TODO: weird ToList()
            //return new ConfigurationSpace(intersection.ToList());
        }



        public void InjectRandomGenerator(Random random)
        {
            this.random = random;
        }
    }
}