using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GeneralAlgorithms.Algorithms.Common;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Graphs;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.Doors.DoorModes;
using MapGeneration.Core.LayoutGenerators.Interfaces;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.MapLayouts;
using MapGeneration.Utils;
using MapGeneration.Utils.Interfaces;

namespace MapGeneration.Simplified
{
    public class SimpleDungeonGenerator :  IRandomInjectable, ICancellable, IObservableGenerator<MapLayout<int>>
    {
        private Random random = new Random(0);
        private CancellationToken? cancellationToken;

        public void InjectRandomGenerator(Random random)
        {
            this.random = random;
        }

        public void SetCancellationToken(CancellationToken? cancellationToken)
        {
            this.cancellationToken = cancellationToken;
        }

        public MapLayout<int> GenerateLayout()
        {
            var levelGeometry = new LevelGeometry<int>();
            levelGeometry.InjectRandomGenerator(random);

            var roomTemplates = new List<RoomTemplate>()
            {
                //new RoomTemplate(GridPolygon.GetSquare(10), new SimpleDoorMode(2, 2)),
                //new RoomTemplate(GridPolygon.GetRectangle(10, 15), new SimpleDoorMode(2, 2), allowedTransformations: TransformationHelper.GetAllTransformations().ToList()),
            };

            roomTemplates.Add(new RoomTemplate(GridPolygon.GetSquare(6), new SimpleDoorMode(2, 1), name: "Square 6x6"));
            roomTemplates.Add(new RoomTemplate(GridPolygon.GetSquare(8), new SimpleDoorMode(2, 1), name: "Square 8x8"));
            roomTemplates.Add(new RoomTemplate(GridPolygon.GetRectangle(6, 8), new SimpleDoorMode(2, 1), name: "Rectangle 6x8"));
            roomTemplates.Add(new RoomTemplate(GridPolygon.GetSquare(12), new SimpleDoorMode(2, 2), name: "Square 12x12"));
            roomTemplates.Add(new RoomTemplate(GridPolygon.GetSquare(14), new SimpleDoorMode(2, 2), name: "Square 14x14"));
            roomTemplates.Add(new RoomTemplate(GridPolygon.GetRectangle(10, 14), new SimpleDoorMode(2, 2), name: "Rectangle 10x14"));
            roomTemplates.Add(new RoomTemplate(GridPolygon.GetRectangle(12, 15), new SimpleDoorMode(2, 2), name: "Rectangle 12x15"));

            var roomTemplateInstances = roomTemplates
                .SelectMany(levelGeometry.GetRoomTemplateInstances)
                .ToList();

            var layout = new SimpleLayout<int>();

            // TODO: weird that we have to both set the configuration and the graph vertex
            layout.Graph.AddVertex(0);
            layout.SetConfiguration(0, new SimpleConfiguration<int>(0, new IntVector2(0, 0), roomTemplateInstances[0]));

            while (layout.Graph.VerticesCount < 500)
            {
                if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
                {
                    return null;
                }

                var room = layout.Graph.VerticesCount;

                // TODO: how to handle properly
                var graphBackup = new UndirectedAdjacencyListGraph<int>((UndirectedAdjacencyListGraph<int>)layout.Graph);

                layout.Graph.AddVertex(room);
                layout.Graph.AddEdge(room, random.Next(layout.Graph.VerticesCount - 1));

                var roomTemplate = roomTemplateInstances.GetRandom(random);
                var neighborConfigurations = layout.GetNeighborConfigurations(room);

                if (!(neighborConfigurations[0].Position.X > 0 && neighborConfigurations[0].Position.Y > 0 || neighborConfigurations[0].Position.X < 0 && neighborConfigurations[0].Position.Y < 0))
                {
                    roomTemplate = roomTemplateInstances[0];
                }
                else
                {
                    roomTemplate = roomTemplateInstances[random.Next(roomTemplateInstances.Count / 2, roomTemplateInstances.Count)];
                }

                var possiblePositionLines = levelGeometry.GetAvailableRoomPositions(roomTemplate, neighborConfigurations);

                // TODO: maybe prepare a helper for this?
                var possiblePositions = possiblePositionLines.SelectMany(x => x.GetPoints()).ToList();

                var hasCorrectPosition = false;

                for (int j = 0; j < 100; j++)
                {
                    var position = possiblePositions.GetRandom(random);

                    layout.SetConfiguration(room, new SimpleConfiguration<int>(room, position, roomTemplate));

                    if (levelGeometry.IsValid(room, layout))
                    {
                        hasCorrectPosition = true;
                        OnPartialValid?.Invoke(levelGeometry.ConvertLayout(layout));
                        break;
                    }
                    else
                    {
                        layout.RemoveConfiguration(room);
                    }
                }

                if (!hasCorrectPosition)
                {
                    layout.Graph = graphBackup;
                    //throw new InvalidOperationException();
                }
            }

            OnValid?.Invoke(levelGeometry.ConvertLayout(layout));

            return levelGeometry.ConvertLayout(layout);
        }

        public event Action<MapLayout<int>> OnPerturbed;
        public event Action<MapLayout<int>> OnPartialValid;
        public event Action<MapLayout<int>> OnValid;
    }
}