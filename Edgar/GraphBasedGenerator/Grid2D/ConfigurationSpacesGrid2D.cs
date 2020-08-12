using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.GraphBasedGenerator.Common;
using Edgar.GraphBasedGenerator.Common.Configurations;
using Edgar.GraphBasedGenerator.Common.ConfigurationSpaces;
using Edgar.Legacy.Core.ConfigurationSpaces;
using Edgar.Legacy.Core.Doors;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.Utils;
using Edgar.Legacy.Utils.Interfaces;
using IRoomDescription = Edgar.GraphBasedGenerator.Common.RoomTemplates.IRoomDescription;

namespace Edgar.GraphBasedGenerator.Grid2D
{
    public class ConfigurationSpacesGrid2D<TConfiguration, TNode> : IConfigurationSpaces<TConfiguration, Vector2Int>, IRandomInjectable
        where TConfiguration: IConfiguration<RoomTemplateInstance, Vector2Int, TNode>
    {
        private readonly ILineIntersection<OrthogonalLine> lineIntersection;
        private readonly ILevelDescription<TNode> levelDescription; // TODO: replace with LevelDescription when possible
        private readonly ConfigurationSpacesGenerator configurationSpacesGenerator;
        private Random random;

        private Dictionary<Tuple<TNode, TNode>, IRoomDescription> nodesToCorridorMapping;

        // TODO: far from ideal
        private readonly Dictionary<Tuple<RoomTemplateInstance, RoomTemplateInstance, RoomDescriptionGrid2D>, ConfigurationSpaceGrid2D> corridorToConfigurationSpaceMapping = new Dictionary<Tuple<RoomTemplateInstance, RoomTemplateInstance, RoomDescriptionGrid2D>, ConfigurationSpaceGrid2D>();

        public ConfigurationSpacesGrid2D(ILevelDescription<TNode> levelDescription, ILineIntersection<OrthogonalLine> lineIntersection = null)
        {
            this.levelDescription = levelDescription;
            this.lineIntersection = lineIntersection ?? new OrthogonalLineIntersection();
            configurationSpacesGenerator = new ConfigurationSpacesGenerator(new PolygonOverlap(), DoorHandler.DefaultHandler, this.lineIntersection, new GridPolygonUtils());
            Initialize();
        }

        private void Initialize()
        {
            nodesToCorridorMapping = configurationSpacesGenerator.GetNodesToCorridorMapping(levelDescription);
        }

        public bool HaveValidPosition(TConfiguration configuration1, TConfiguration configuration2)
        {
            var space = GetConfigurationSpace(configuration1, configuration2);
            var lines1 = new List<OrthogonalLine>() {new OrthogonalLine(configuration1.Position, configuration1.Position)};

            return lineIntersection.DoIntersect(space.Lines.Select(x => FastAddition(x, configuration2.Position)), lines1);
        }

        private OrthogonalLine FastAddition(OrthogonalLine line, Vector2Int position) 
        {
            return new OrthogonalLine(line.From + position, line.To + position);
        }

        IConfigurationSpace<Vector2Int> IConfigurationSpaces<TConfiguration, Vector2Int>.GetConfigurationSpace(TConfiguration configuration1, TConfiguration configuration2)
        {
            return GetConfigurationSpace(configuration1, configuration2);
        }

        public ConfigurationSpaceGrid2D GetConfigurationSpace(TConfiguration configuration1, TConfiguration configuration2)
        {
            // If is over corridor
            if (nodesToCorridorMapping.ContainsKey(new Tuple<TNode, TNode>(configuration1.Room, configuration2.Room)))
            {
                var roomDescription = (RoomDescriptionGrid2D) nodesToCorridorMapping[new Tuple<TNode, TNode>(configuration1.Room, configuration2.Room)];
                var selector = Tuple.Create(configuration1.RoomShape, configuration2.RoomShape, roomDescription);

                if (corridorToConfigurationSpaceMapping.TryGetValue(selector, out var cachedConfigurationSpace))
                {
                    return cachedConfigurationSpace;
                }

                var corridorRoomTemplateInstances = roomDescription.RoomTemplates
                    .SelectMany(configurationSpacesGenerator.GetRoomTemplateInstances).ToList();

                // var configurationSpace = configurationSpaces.GetConfigurationSpace(configuration1, configuration2);

                var configurationSpace = configurationSpacesGenerator.GetConfigurationSpaceOverCorridors(configuration1.RoomShape, configuration2.RoomShape, corridorRoomTemplateInstances);

                var configurationSpaceNew =
                    new ConfigurationSpaceGrid2D(configurationSpace.Lines, configurationSpace.ReverseDoors);
                corridorToConfigurationSpaceMapping[selector] = configurationSpaceNew;

                return configurationSpaceNew;
            }
            // Otherwise
            else
            {
                var configurationSpace = configurationSpacesGenerator.GetConfigurationSpace(configuration1.RoomShape, configuration2.RoomShape);

                // var configurationSpace = configurationSpaces.GetConfigurationSpace(configuration1, configuration2);

                return new ConfigurationSpaceGrid2D(configurationSpace.Lines, configurationSpace.ReverseDoors);
            }
        }

        public IConfigurationSpace<Vector2Int> GetMaximumIntersection(TConfiguration mainConfiguration, IEnumerable<TConfiguration> configurations)
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

        public IConfigurationSpace<Vector2Int> GetMaximumIntersection(TConfiguration mainConfiguration, IEnumerable<TConfiguration> configurations, out int configurationsSatisfied)
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