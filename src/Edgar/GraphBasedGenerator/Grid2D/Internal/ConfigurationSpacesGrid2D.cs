using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common;
using Edgar.GraphBasedGenerator.Common.Configurations;
using Edgar.GraphBasedGenerator.Common.ConfigurationSpaces;
using Edgar.Graphs;
using Edgar.Legacy.Core.Doors;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;
using Edgar.Legacy.Utils;
using Edgar.Legacy.Utils.Interfaces;
using IRoomDescription = Edgar.GraphBasedGenerator.Common.RoomTemplates.IRoomDescription;

namespace Edgar.GraphBasedGenerator.Grid2D.Internal
{
    public class ConfigurationSpacesGrid2D<TConfiguration, TNode> : IConfigurationSpaces<TConfiguration, Vector2Int>,
        IRandomInjectable
        where TConfiguration : IConfiguration<RoomTemplateInstanceGrid2D, Vector2Int, TNode>
    {
        private readonly ILineIntersection<OrthogonalLineGrid2D> lineIntersection;
        private readonly ILevelDescription<TNode> levelDescription; // TODO: replace with LevelDescription when possible
        private readonly ConfigurationSpacesGenerator configurationSpacesGenerator;
        private readonly DirectedConfigurationSpacesGenerator directedConfigurationSpacesGenerator;
        private Random random;

        private readonly Dictionary<CacheKey, ConfigurationSpaceGrid2D> cache =
            new Dictionary<CacheKey, ConfigurationSpaceGrid2D>();

        private readonly bool isGraphDirected;
        private Dictionary<CorridorSelector, IRoomDescription> nodesToCorridorMapping;
        private IGraph<TNode> directedGraph;
        private IGraph<TNode> directedGraphWithoutCorridors;

        public ConfigurationSpacesGrid2D(ILevelDescription<TNode> levelDescription,
            ILineIntersection<OrthogonalLineGrid2D> lineIntersection = null, bool isGraphDirected = false)
        {
            this.levelDescription = levelDescription;
            this.isGraphDirected = isGraphDirected;
            this.lineIntersection = lineIntersection ?? new OrthogonalLineIntersection();
            var polygonOverlap = new PolygonOverlap();
            configurationSpacesGenerator = new ConfigurationSpacesGenerator(polygonOverlap, DoorHandler.DefaultHandler,
                this.lineIntersection, new GridPolygonUtils());
            directedConfigurationSpacesGenerator =
                new DirectedConfigurationSpacesGenerator(polygonOverlap, this.lineIntersection);
            Initialize();
        }

        private void Initialize()
        {
            nodesToCorridorMapping = configurationSpacesGenerator
                .GetNodesToCorridorMapping(levelDescription)
                .ToDictionary(
                    x => new CorridorSelector(x.Key.Item1, x.Key.Item2),
                    x => x.Value
                );

            if (isGraphDirected)
            {
                directedGraph = levelDescription.GetGraph(false, true);
                directedGraphWithoutCorridors = levelDescription.GetGraph(true, true);
            }
        }

        public bool HaveValidPosition(TConfiguration configuration1, TConfiguration configuration2)
        {
            var space = GetConfigurationSpace(configuration1, configuration2);
            var line = new OrthogonalLineGrid2D(configuration1.Position - configuration2.Position,
                configuration1.Position - configuration2.Position);

            return lineIntersection.DoIntersect(space.Lines, line);
        }

        private OrthogonalLineGrid2D FastAddition(OrthogonalLineGrid2D line, Vector2Int position)
        {
            return new OrthogonalLineGrid2D(line.From + position, line.To + position);
        }

        IConfigurationSpace<Vector2Int> IConfigurationSpaces<TConfiguration, Vector2Int>.GetConfigurationSpace(
            TConfiguration configuration1, TConfiguration configuration2)
        {
            return GetConfigurationSpace(configuration1, configuration2);
        }

        public ConfigurationSpaceGrid2D GetConfigurationSpace(TConfiguration configuration1,
            TConfiguration configuration2)
        {
            // If is over corridor
            if (nodesToCorridorMapping.ContainsKey(new CorridorSelector(configuration1.Room, configuration2.Room)))
            {
                return GetCorridorConfigurationSpace(configuration1, configuration2);
            }
            // Otherwise
            else
            {
                return GetNormalConfigurationSpace(configuration1, configuration2);
            }
        }

        private ConfigurationSpaceGrid2D GetNormalConfigurationSpace(TConfiguration configuration1,
            TConfiguration configuration2)
        {
            var direction = GetDirection(configuration1, configuration2, false);
            var cacheKey = new CacheKey(configuration1.RoomShape, configuration2.RoomShape, null, direction);

            if (cache.TryGetValue(cacheKey, out var configurationSpace))
            {
                return configurationSpace;
            }

            if (isGraphDirected)
            {
                configurationSpace =
                    directedConfigurationSpacesGenerator.GetConfigurationSpace(configuration1.RoomShape,
                        configuration2.RoomShape, direction);
            }
            else
            {
                configurationSpace =
                    configurationSpacesGenerator.GetConfigurationSpace(configuration1.RoomShape,
                        configuration2.RoomShape);
            }

            cache[cacheKey] = configurationSpace;

            return configurationSpace;
        }

        private ConfigurationSpaceDirection GetDirection(TConfiguration configuration1, TConfiguration configuration2,
            bool isOverCorridor)
        {
            if (isGraphDirected)
            {
                var graph = isOverCorridor ? directedGraphWithoutCorridors : directedGraph;

                if (graph.HasEdge(configuration1.Room, configuration2.Room))
                {
                    return ConfigurationSpaceDirection.FromMovingToFixed;
                }
                else
                {
                    return ConfigurationSpaceDirection.FromFixedToMoving;
                }
            }
            else
            {
                return ConfigurationSpaceDirection.FromFixedToMoving;
            }
        }

        private ConfigurationSpaceGrid2D GetCorridorConfigurationSpace(TConfiguration configuration1,
            TConfiguration configuration2)
        {
            var direction = GetDirection(configuration1, configuration2, true);
            var corridorRoomDescription =
                (RoomDescriptionGrid2D) nodesToCorridorMapping[
                    new CorridorSelector(configuration1.Room, configuration2.Room)];
            var cacheKey = new CacheKey(configuration1.RoomShape, configuration2.RoomShape, corridorRoomDescription,
                direction);

            if (cache.TryGetValue(cacheKey, out var configurationSpace))
            {
                return configurationSpace;
            }

            var corridorRoomTemplateInstances = corridorRoomDescription.RoomTemplates
                .SelectMany(configurationSpacesGenerator.GetRoomTemplateInstances).ToList();

            if (isGraphDirected)
            {
                var configurationSpaceOld =
                    directedConfigurationSpacesGenerator.GetConfigurationSpaceOverCorridors(configuration1.RoomShape,
                        configuration2.RoomShape, corridorRoomTemplateInstances, direction);
                configurationSpace = new ConfigurationSpaceGrid2D(configurationSpaceOld.Lines, null);
            }
            else
            {
                var configurationSpaceOld =
                    configurationSpacesGenerator.GetConfigurationSpaceOverCorridors(configuration1.RoomShape,
                        configuration2.RoomShape, corridorRoomTemplateInstances);
                configurationSpace = new ConfigurationSpaceGrid2D(configurationSpaceOld.Lines, null);
            }

            cache[cacheKey] = configurationSpace;

            return configurationSpace;
        }

        public IConfigurationSpace<Vector2Int> GetMaximumIntersection(TConfiguration mainConfiguration,
            IEnumerable<TConfiguration> configurations)
        {
            return GetMaximumIntersection(mainConfiguration, configurations, out var _);
        }

        private IList<Tuple<TConfiguration, ConfigurationSpaceGrid2D>> GetConfigurationSpaces(
            TConfiguration mainConfiguration, IList<TConfiguration> configurations)
        {
            var spaces = new List<Tuple<TConfiguration, ConfigurationSpaceGrid2D>>();

            foreach (var configuration in configurations)
            {
                spaces.Add(Tuple.Create(configuration, GetConfigurationSpace(mainConfiguration, configuration)));
            }

            return spaces;
        }

        public IConfigurationSpace<Vector2Int> GetMaximumIntersection(TConfiguration mainConfiguration,
            IEnumerable<TConfiguration> configurations, out int configurationsSatisfied)
        {
            // TODO: weird ToList()
            var configurationsList = configurations.ToList();

            var spaces = GetConfigurationSpaces(mainConfiguration, configurationsList);
            spaces.Shuffle(random);

            for (var i = configurationsList.Count; i > 0; i--)
            {
                foreach (var indices in configurationsList.GetCombinations(i))
                {
                    List<OrthogonalLineGrid2D> intersection = null;

                    foreach (var index in indices)
                    {
                        var linesToIntersect = spaces[index].Item2.Lines;
                        var offset = spaces[index].Item1.Position;

                        intersection = intersection != null
                            ? lineIntersection.GetIntersections(linesToIntersect, intersection, offset)
                            : linesToIntersect.Select(x => x + offset).ToList();

                        if (intersection.Count == 0)
                        {
                            break;
                        }
                    }

                    if (intersection != null && intersection.Count != 0)
                    {
                        configurationsSatisfied = indices.Length;

                        // TODO: this allocates new data because it creates an ImmutableArray
                        // Consider having something like two variants of configuration spaces, e.g. mutable and immutable
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

        private readonly struct CacheKey : IEquatable<CacheKey>
        {
            private readonly RoomTemplateInstanceGrid2D roomTemplate1;

            private readonly RoomTemplateInstanceGrid2D roomTemplate2;

            private readonly RoomDescriptionGrid2D corridorRoomDescription;

            private readonly ConfigurationSpaceDirection direction;

            public CacheKey(RoomTemplateInstanceGrid2D roomTemplate1, RoomTemplateInstanceGrid2D roomTemplate2,
                RoomDescriptionGrid2D corridorRoomDescription, ConfigurationSpaceDirection direction)
            {
                this.roomTemplate1 = roomTemplate1;
                this.roomTemplate2 = roomTemplate2;
                this.corridorRoomDescription = corridorRoomDescription;
                this.direction = direction;
            }

            #region Equals

            public bool Equals(CacheKey other)
            {
                return Equals(roomTemplate1, other.roomTemplate1) && Equals(roomTemplate2, other.roomTemplate2) &&
                       Equals(corridorRoomDescription, other.corridorRoomDescription) && direction == other.direction;
            }

            public override bool Equals(object obj)
            {
                return obj is CacheKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = (roomTemplate1 != null ? roomTemplate1.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (roomTemplate2 != null ? roomTemplate2.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^
                               (corridorRoomDescription != null ? corridorRoomDescription.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (int) direction;
                    return hashCode;
                }
            }

            public static bool operator ==(CacheKey left, CacheKey right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(CacheKey left, CacheKey right)
            {
                return !left.Equals(right);
            }

            #endregion
        }

        private readonly struct CorridorSelector : IEquatable<CorridorSelector>
        {
            private readonly TNode room1;

            private readonly TNode room2;

            public CorridorSelector(TNode room1, TNode room2)
            {
                this.room1 = room1;
                this.room2 = room2;
            }

            #region Equals

            public bool Equals(CorridorSelector other)
            {
                return EqualityComparer<TNode>.Default.Equals(room1, other.room1) &&
                       EqualityComparer<TNode>.Default.Equals(room2, other.room2);
            }

            public override bool Equals(object obj)
            {
                return obj is CorridorSelector other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (EqualityComparer<TNode>.Default.GetHashCode(room1) * 397) ^
                           EqualityComparer<TNode>.Default.GetHashCode(room2);
                }
            }

            public static bool operator ==(CorridorSelector left, CorridorSelector right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(CorridorSelector left, CorridorSelector right)
            {
                return !left.Equals(right);
            }

            #endregion
        }
    }
}