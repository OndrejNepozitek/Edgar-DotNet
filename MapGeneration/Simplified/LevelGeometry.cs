using System;
using System.Collections.Generic;
using System.Linq;
using GeneralAlgorithms.Algorithms.Common;
using GeneralAlgorithms.Algorithms.Polygons;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.ConfigurationSpaces;
using MapGeneration.Core.Doors;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.MapLayouts;
using MapGeneration.Simplified.ConfigurationSpaces;
using MapGeneration.Utils;
using MapGeneration.Utils.Interfaces;

namespace MapGeneration.Simplified
{
    public class LevelGeometry<TRoom> : IRandomInjectable
    {
        private readonly CachedPolygonOverlap polygonOverlap;
        private readonly ConfigurationSpacesGenerator configurationSpacesGenerator;
        private readonly LazyConfigurationSpaces configurationSpaces;
        private readonly SimpleLayoutConverter<TRoom> layoutConverter;
        private Random random;

        public LevelGeometry()
        {
            polygonOverlap = new CachedPolygonOverlap();

            var lineIntersection = new OrthogonalLineIntersection();
            configurationSpacesGenerator = new ConfigurationSpacesGenerator(
                polygonOverlap,
                DoorHandler.DefaultHandler,
                lineIntersection,
                new GridPolygonUtils()
                );
            configurationSpaces = new LazyConfigurationSpaces(lineIntersection, configurationSpacesGenerator);
            layoutConverter = new SimpleLayoutConverter<TRoom>(configurationSpaces);
        }

        // TODO: should this be here?
        public MapLayout<TRoom> ConvertLayout(SimpleLayout<TRoom> layout)
        {
            return layoutConverter.Convert(layout, true);
        }

        public bool IsValid(SimpleLayout<TRoom> layout)
        {
            foreach (var configuration in layout.GetAllConfigurations())
            {
                if (!IsValid(configuration.Room, layout))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsValid(TRoom room, SimpleLayout<TRoom> layout)
        {
            var configuration = layout.GetConfiguration(room);

            foreach (var otherConfiguration in layout.GetAllConfigurations())
            {
                var otherRoom = otherConfiguration.Room;

                if (room.Equals(otherRoom))
                {
                    continue;
                }

                if (layout.GraphWithCorridors.HasEdge(room, otherRoom))
                {
                    if (!configurationSpaces.HaveValidPosition(GetConfiguration(configuration), GetConfiguration(otherConfiguration)))
                    {
                        return false;
                    }
                }
                else
                {
                    if (DoRoomsOverlap(configuration, otherConfiguration))
                    {
                        return false;
                    }

                    // TODO: very ugly
                    if (polygonOverlap.DoTouch(configuration.RoomTemplateInstance.RoomShape, configuration.Position,
                        otherConfiguration.RoomTemplateInstance.RoomShape, otherConfiguration.Position))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool DoRoomsOverlap(SimpleConfiguration<TRoom> configuration1, SimpleConfiguration<TRoom> configuration2)
        {
            return DoRoomsOverlap(configuration1.RoomTemplateInstance.RoomShape, configuration1.Position,
                configuration2.RoomTemplateInstance.RoomShape, configuration2.Position);
        }

        public bool DoRoomsOverlap(GridPolygon polygon1, IntVector2 position1, GridPolygon polygon2, IntVector2 position2)
        {
            return polygonOverlap.DoOverlap(polygon1, position1, polygon2, position2);
        }

        public List<RoomTemplateInstance> GetRoomTemplateInstances(RoomTemplate roomTemplate)
        {
            return configurationSpacesGenerator.GetRoomTemplateInstances(roomTemplate);
        }

        public List<OrthogonalLine> GetConnectionPositions(RoomTemplateInstance roomTemplateInstance, List<SimpleConfiguration<TRoom>> neighborConfigurations)
        {
            return configurationSpaces.GetMaximumIntersection(
                GetConfiguration(new SimpleConfiguration<TRoom>(default, new IntVector2(), roomTemplateInstance)),
                neighborConfigurations.Select(x => GetConfiguration(x)).ToList(),
                neighborConfigurations.Count,
                out _
            ) ?? new List<OrthogonalLine>();
        }

        public List<OrthogonalLine> GetConnectionPositions(RoomTemplateInstance roomTemplateInstance, List<ConnectionTarget<TRoom>> neighborConfigurations)
        {
            return configurationSpaces.GetMaximumIntersection(
                GetConfiguration(new SimpleConfiguration<TRoom>(default, new IntVector2(), roomTemplateInstance)),
                neighborConfigurations.Select(x => GetConfiguration(x.Configuration, x.Corridors)).ToList(),
                neighborConfigurations.Count,
                out _
            ) ?? new List<OrthogonalLine>();
        }

       

        public void InjectRandomGenerator(Random random)
        {
            this.random = random;
            configurationSpaces.InjectRandomGenerator(random);
            layoutConverter.InjectRandomGenerator(random);
        }

        // TODO: how to handle this?
        private LazyConfigurationSpaces.Configuration GetConfiguration(SimpleConfiguration<TRoom> configuration, List<RoomTemplateInstance> corridors = null)
        {
            return new LazyConfigurationSpaces.Configuration(configuration.Position, configuration.RoomTemplateInstance, corridors);
        }
    }
}