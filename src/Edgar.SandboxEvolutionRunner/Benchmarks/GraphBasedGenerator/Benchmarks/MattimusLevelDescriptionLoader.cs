using System;
using System.Collections.Generic;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common;
using Edgar.GraphBasedGenerator.Grid2D;
using SandboxEvolutionRunner.Utils;

namespace Edgar.SandboxEvolutionRunner.Benchmarks.GraphBasedGenerator.Benchmarks
{
    public class MattimusLevelDescriptionLoader<TRoom> : LevelDescriptionLoader<TRoom>
    {
        public MattimusLevelDescriptionLoader(RoomTemplatesSet roomTemplatesSet, Vector2Int scale,
            RoomTemplateRepeatMode repeatMode = RoomTemplateRepeatMode.AllowRepeat,
            Func<int, TRoom> getCorridorNameFunc = null) : base(roomTemplatesSet, scale, repeatMode,
            getCorridorNameFunc)
        {
        }

        protected override List<RoomTemplateGrid2D> GetMediumRoomTemplates()
        {
            return new List<RoomTemplateGrid2D>()
            {
                new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(16),
                    GetDoorMode(new List<Vector2Int>() {new Vector2Int(0, 0)}),
                    allowedTransformations: TransformationGrid2DHelper.GetAll()),
                new RoomTemplateGrid2D(PolygonGrid2D.GetRectangle(32, 16), GetDoorMode(new List<Vector2Int>()
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                }), allowedTransformations: TransformationGrid2DHelper.GetAll()),
                new RoomTemplateGrid2D(PolygonGrid2D.GetRectangle(32, 32), GetDoorMode(new List<Vector2Int>()
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                    new Vector2Int(1, 1),
                    new Vector2Int(0, 1),
                }), allowedTransformations: TransformationGrid2DHelper.GetAll()),
            };
        }

        private ManualDoorModeGrid2D GetDoorMode(List<Vector2Int> usedPlaces)
        {
            var doors = new List<DoorGrid2D>();
            var directions = new List<Vector2Int>()
            {
                new Vector2Int(0, 1),
                new Vector2Int(0, -1),
                new Vector2Int(1, 0),
                new Vector2Int(-1, 0),
            };
            var gridSize = new Vector2Int(16, 16);

            foreach (var usedPlace in usedPlaces)
            {
                var offset = new Vector2Int(usedPlace.X * gridSize.X, usedPlace.Y * gridSize.Y);

                foreach (var direction in directions)
                {
                    var newPlace = usedPlace + direction;

                    if (!usedPlaces.Contains(newPlace))
                    {
                        if (direction == new Vector2Int(-1, 0))
                        {
                            var from = new Vector2Int(0, 7);
                            var to = new Vector2Int(0, 8);
                            doors.Add(new DoorGrid2D(from + offset, to + offset));
                        }
                        else if (direction == new Vector2Int(1, 0))
                        {
                            var from = new Vector2Int(gridSize.X, 7);
                            var to = new Vector2Int(gridSize.X, 8);
                            doors.Add(new DoorGrid2D(from + offset, to + offset));
                        }
                        else if (direction == new Vector2Int(0, 1))
                        {
                            var from = new Vector2Int(7, gridSize.Y);
                            var to = new Vector2Int(8, gridSize.Y);
                            doors.Add(new DoorGrid2D(from + offset, to + offset));
                        }
                        else if (direction == new Vector2Int(0, -1))
                        {
                            var from = new Vector2Int(7, 0);
                            var to = new Vector2Int(8, 0);
                            doors.Add(new DoorGrid2D(from + offset, to + offset));
                        }
                    }
                }
            }

            return new ManualDoorModeGrid2D(doors);
        }

        protected override List<RoomTemplateGrid2D> GetSmallRoomTemplates()
        {
            return new List<RoomTemplateGrid2D>();
        }
    }
}