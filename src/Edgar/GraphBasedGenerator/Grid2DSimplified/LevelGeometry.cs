using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.GraphBasedGenerator.Grid2D.Internal;
using Edgar.Legacy.Core.Doors;
using Edgar.Legacy.Core.MapLayouts;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;
using Edgar.Legacy.Utils.Interfaces;

namespace Edgar.GraphBasedGenerator.Grid2DSimplified
{
    public class LevelGeometry<TRoom> : IRandomInjectable
    {
        
        private readonly ConfigurationSpacesGenerator configurationSpacesGenerator;
        private readonly LazyConfigurationSpaces configurationSpaces;
        private readonly SimpleLayoutConverter<TRoom> layoutConverter;
        private Random random;

        private readonly FastGridPolygonGeometry<SimpleConfiguration<TRoom>, TRoom> gridPolygonGeometry;
        private readonly PolygonOverlap polygonOverlap;

        public LevelGeometry()
        {
            polygonOverlap = new PolygonOverlap();
            gridPolygonGeometry = new FastGridPolygonGeometry<SimpleConfiguration<TRoom>, TRoom>();

            var lineIntersection = new OrthogonalLineIntersection();
            configurationSpacesGenerator = new ConfigurationSpacesGenerator(
                polygonOverlap,
                DoorHandler.DefaultHandler,
                lineIntersection,
                new GridPolygonUtils()
                );
            configurationSpaces = new LazyConfigurationSpaces(configurationSpacesGenerator, lineIntersection);
            layoutConverter = new SimpleLayoutConverter<TRoom>(configurationSpaces);
        }

        // TODO: should this be here?
        public LayoutGrid2D<TRoom> ConvertLayout(SimpleLayout<TRoom, SimpleConfiguration<TRoom>> layout)
        {
            return layoutConverter.Convert(layout, true);
        }

        public bool IsValid(SimpleLayout<TRoom, SimpleConfiguration<TRoom>> layout)
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

        public bool IsValid(TRoom room, SimpleLayout<TRoom, SimpleConfiguration<TRoom>> layout)
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

                    //if (!gridPolygonGeometry.DoHaveMinimumDistance(configuration, otherConfiguration, 1))
                    //{
                    //    return false;
                    //}

                    // TODO: very ugly
                    if (polygonOverlap.DoTouch(configuration.RoomShape.RoomShape, configuration.Position,
                        otherConfiguration.RoomShape.RoomShape, otherConfiguration.Position))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool DoRoomsOverlap(SimpleConfiguration<TRoom> configuration1, SimpleConfiguration<TRoom> configuration2)
        {
            return DoRoomsOverlap(configuration1.RoomShape.RoomShape, configuration1.Position, configuration2.RoomShape.RoomShape,
                configuration2.Position);
        }

        public bool DoRoomsOverlap(PolygonGrid2D polygon1, Vector2Int position1, PolygonGrid2D polygon2, Vector2Int position2)
        {
            return polygonOverlap.DoOverlap(polygon1, position1, polygon2, position2);
        }

        public List<RoomTemplateInstanceGrid2D> GetRoomTemplateInstances(RoomTemplateGrid2D RoomTemplateGrid2D)
        {
            return configurationSpacesGenerator.GetRoomTemplateInstances(RoomTemplateGrid2D);
        }

        public List<OrthogonalLineGrid2D> GetConnectionPositions(RoomTemplateInstanceGrid2D roomTemplateInstance, List<SimpleConfiguration<TRoom>> neighborConfigurations)
        {
            return configurationSpaces.GetMaximumIntersection(
                GetConfiguration(new SimpleConfiguration<TRoom>()
                {
                    RoomShape = roomTemplateInstance,
                }),
                neighborConfigurations.Select(x => GetConfiguration(x)).ToList(),
                neighborConfigurations.Count,
                out _
            )?.Lines?.ToList() ?? new List<OrthogonalLineGrid2D>();
        }

        public List<OrthogonalLineGrid2D> GetConnectionPositions(RoomTemplateInstanceGrid2D roomTemplateInstance, List<ConnectionTarget<TRoom>> neighborConfigurations)
        {
            return configurationSpaces.GetMaximumIntersection(
                GetConfiguration(new SimpleConfiguration<TRoom>()
                {
                    RoomShape = roomTemplateInstance
                }),
                neighborConfigurations.Select(x => GetConfiguration(x.Configuration, x.Corridors)).ToList(),
                neighborConfigurations.Count,
                out _
            )?.Lines?.ToList() ?? new List<OrthogonalLineGrid2D>();
        }



        public void InjectRandomGenerator(Random random)
        {
            this.random = random;
            configurationSpaces.InjectRandomGenerator(random);
            layoutConverter.InjectRandomGenerator(random);
        }

        // TODO: how to handle this?
        private LazyConfigurationSpaces.Configuration GetConfiguration(SimpleConfiguration<TRoom> configuration, List<RoomTemplateInstanceGrid2D> corridors = null)
        {
            return new LazyConfigurationSpaces.Configuration(configuration.Position, configuration.RoomShape, corridors);
        }
    }
}