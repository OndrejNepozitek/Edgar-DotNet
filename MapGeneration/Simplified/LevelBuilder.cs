using System;
using System.Collections.Generic;
using System.Linq;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Utils;
using MapGeneration.Utils.Interfaces;

namespace MapGeneration.Simplified
{
    public class LevelBuilder<TRoom> : IRandomInjectable
    {
        private readonly LevelGeometry<TRoom> levelGeometry;
        private Random random;

        public LevelBuilder(LevelGeometry<TRoom> levelGeometry)
        {
            this.levelGeometry = levelGeometry;
        }

        public void InjectRandomGenerator(Random random)
        {
            this.random = random;
        }

         public bool TryConnectRoomToNeighbors(SimpleLayout<TRoom> layout, TRoom room, RoomTemplateInstance roomTemplate, List<ConnectionTarget<TRoom>> neighborConfigurations, int maximumAttempts = -1)
        {
            // Find all possible connection points
            var connectionPositions = levelGeometry.GetConnectionPositions(roomTemplate, neighborConfigurations)
                .SelectMany(x => x.GetPoints())
                .ToList();

            // Shuffle connection positions to make sure that we do not always pick a position that is on the left side of a room
            connectionPositions.Shuffle(random);

            // Set maximum attempts to the number of connection positions if it was set to -1
            maximumAttempts = maximumAttempts == -1 ? connectionPositions.Count : maximumAttempts;

            // Set maximum attempts to the number of connection positions if the original number was larger - it does not make sense to try a position more that once
            maximumAttempts = Math.Min(maximumAttempts, connectionPositions.Count);

            // Begin a transaction so we can easily rollback if we do not find a valid position
            using (var transaction = layout.BeginChanges())
            {
                for (int i = 0; i < maximumAttempts; i++)
                {
                    var position = connectionPositions[i];
                    layout.SetConfiguration(room, new SimpleConfiguration<TRoom>(room, position, roomTemplate));

                    // Commit the transaction if the layout is valid
                    if (levelGeometry.IsValid(room, layout))
                    {
                        transaction.Commit();
                        return true;
                    }
                }
            }

            return false;
        }

        public bool TryConnectRoomToNeighbors(SimpleLayout<TRoom> layout, TRoom room, RoomTemplateInstance roomTemplate, List<RoomTemplateInstance> corridorRoomTemplates, int maximumAttempts = -1)
        {
            if (layout.IsCorridor(room))
            {
                throw new ArgumentException($"The room {room} is a corridor", nameof(room));
            }

            var connectionTargets = new List<ConnectionTarget<TRoom>>();
            var neighbors = layout.Graph.GetNeighbours(room);

            // Go through all the neighbors and check if they are connected via corridors or not.
            // If they are connected via corridors, we want to use given corridor room templates to connect them.
            foreach (var neighbor in neighbors)
            {
                var configuration = layout.GetConfiguration(neighbor);

                // We can check if two rooms are connected via corridors by checking if there is an edge between the two rooms in the graph with corridors.
                // If there is not an edge, that means that there is a corridor room between them.
                var isViaCorridor = !layout.GraphWithCorridors.HasEdge(room, neighbor);

                if (isViaCorridor && (corridorRoomTemplates == null || corridorRoomTemplates.Count == 0))
                {
                    throw new ArgumentException("Corridor room templates must be provided if any of the neighbors are connected via corridor", nameof(corridorRoomTemplates));
                }

                connectionTargets.Add(new ConnectionTarget<TRoom>(configuration, isViaCorridor ? corridorRoomTemplates : null));
            }

            return TryConnectRoomToNeighbors(layout, room, roomTemplate, connectionTargets, maximumAttempts);
        }

        public bool TryAddCorridorRoom(SimpleLayout<TRoom> layout, TRoom room, RoomTemplateInstance roomTemplate, int maximumAttempts = -1)
        {
            // TODO: uncomment
            //if (!layout.IsCorridor(room))
            //{
            //    throw new ArgumentException($"The room {room} is not a corridor", nameof(room));
            //}

            var connectionTargets = new List<ConnectionTarget<TRoom>>();
            var neighbors = layout.GraphWithCorridors.GetNeighbours(room);

            foreach (var neighbor in neighbors)
            {
                var configuration = layout.GetConfiguration(neighbor);
                connectionTargets.Add(new ConnectionTarget<TRoom>(configuration));
            }

            return TryConnectRoomToNeighbors(layout, room, roomTemplate, connectionTargets, maximumAttempts);
        }

        public bool TryAddCorridorRoom(SimpleLayout<TRoom> layout, TRoom room, List<RoomTemplateInstance> roomTemplates, int maximumAttempts = -1)
        {
            // TODO: uncomment
            //if (!layout.IsCorridor(room))
            //{
            //    throw new ArgumentException($"The room {room} is not a corridor", nameof(room));
            //}

            // Shuffle room templates so that we do not always use the same corridor
            roomTemplates = roomTemplates.ToList();
            roomTemplates.Shuffle(random);

            foreach (var corridorRoomTemplate in roomTemplates)
            {
                if (TryAddCorridorRoom(layout, room, corridorRoomTemplate, maximumAttempts))
                {
                    return true;
                }
            }

            return false;
        }

        public bool TryAddCorridors(SimpleLayout<TRoom> layout, TRoom room, List<RoomTemplateInstance> corridorRoomTemplates, int maximumAttempts = -1)
        {
            if (layout.IsCorridor(room))
            {
                throw new ArgumentException($"The room {room} is a corridor.", nameof(room));
            }

            var corridorNeighbors = layout
                .GraphWithCorridors
                .GetNeighbours(room)
                .Where(layout.IsCorridor)
                .ToList();

            corridorRoomTemplates = corridorRoomTemplates.ToList();

            foreach (var neighbor in corridorNeighbors)
            {
                // Shuffle room templates so that we do not always use the same corridor
                corridorRoomTemplates.Shuffle(random);

                var foundValid = false;

                foreach (var corridorRoomTemplate in corridorRoomTemplates)
                {
                    if (TryAddCorridorRoom(layout, neighbor, corridorRoomTemplate, maximumAttempts))
                    {
                        foundValid = true;
                        break;
                    }
                }

                if (foundValid == false)
                {
                    return false;
                }
            }

            return true;
        }
    }
}