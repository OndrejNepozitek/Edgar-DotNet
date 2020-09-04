using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common;
using Edgar.GraphBasedGenerator.Common.Configurations;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.GraphBasedGenerator.Grid2D.Drawing;
using Edgar.GraphBasedGenerator.Grid2D.Internal;
using Edgar.GraphBasedGenerator.Grid2D.Internal.Corridors;
using Edgar.Graphs;
using Edgar.Legacy.Core.Doors;
using Edgar.Legacy.Core.Layouts.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;
using Edgar.Legacy.Utils.Interfaces;
using NUnit.Framework;

namespace Edgar.Tests.GraphBasedGenerator.Grid2D
{
    [TestFixture]
    public class CorridorsPathfinderTests
    {
        [Test]
        public void TwoSquares_NextToEachOther_NoObstacles()
        {
            var config = new Config(1, 1, 0);

            var square = new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(3), new SimpleDoorModeGrid2D(1, 1));
            var configurations = new List<Configuration>()
            {
                GetConfiguration(0, square, new Vector2Int(0, 0)),
                GetConfiguration(1, square, new Vector2Int(7, 0)),
            };
            var expectedResult = GetExpectedResult(
                new PolygonGrid2DBuilder()
                    .AddPoint(3, 1)
                    .AddPoint(3, 2)
                    .AddPoint(7, 2)
                    .AddPoint(7, 1)
                    .Build(),
                new DoorGrid2D(new Vector2Int(3, 1), new Vector2Int(3, 2)),
                new DoorGrid2D(new Vector2Int(7, 2), new Vector2Int(7, 1))
            );

            CheckResult(config, configurations, 0, 1,expectedResult, GetCurrentMethod());
        }

        [Test]
        public void TwoSquares_NextToEachOther_NoObstacles_WidthTwo()
        {
            var config = new Config(2, 2, 0);

            var square = new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(4), new SimpleDoorModeGrid2D(2, 1));
            var configurations = new List<Configuration>()
            {
                GetConfiguration(0, square, new Vector2Int(0, 0)),
                GetConfiguration(1, square, new Vector2Int(7, 0)),
            };
            var expectedResult = GetExpectedResult(
                new PolygonGrid2DBuilder()
                    .AddPoint(4, 1)
                    .AddPoint(4, 3)
                    .AddPoint(7, 3)
                    .AddPoint(7, 1)
                    .Build(),
                new DoorGrid2D(new Vector2Int(4, 1), new Vector2Int(4, 3)),
                new DoorGrid2D(new Vector2Int(7, 1), new Vector2Int(7, 3))
            );

            CheckResult(config, configurations, 0, 1,expectedResult, GetCurrentMethod());
        }

        [Test]
        public void TwoSquares_NextToEachOther_WithObstacle()
        {
            var config = new Config(1, 1, 0);

            var square = new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(3), new SimpleDoorModeGrid2D(1, 1));
            var rectangle = new RoomTemplateGrid2D(PolygonGrid2D.GetRectangle(2, 10), new SimpleDoorModeGrid2D(1, 1));

            var configurations = new List<Configuration>()
            {
                GetConfiguration(0, square, new Vector2Int(0, 0)),
                GetConfiguration(1, square, new Vector2Int(11, 0)),
                GetConfiguration(2, rectangle, new Vector2Int(6, -6)),
            };
            var expectedResult = GetExpectedResult(
                new PolygonGrid2DBuilder()
                    .AddPoint(2, 3)
                    .AddPoint(1, 3)
                    .AddPoint(1,5)
                    .AddPoint(13, 5)
                    .AddPoint(13, 3)
                    .AddPoint(12, 3)
                    .AddPoint(12, 4)
                    .AddPoint(2, 4)
                    .Build(),
                new DoorGrid2D(new Vector2Int(2, 3), new Vector2Int(1, 3)),
                new DoorGrid2D(new Vector2Int(13, 3), new Vector2Int(12, 3))
            );

            CheckResult(config, configurations, 0, 1,expectedResult, GetCurrentMethod());
        }

        [Test]
        public void TwoSquares_NextToEachOther_WithObstacle_WidthTwo()
        {
            var config = new Config(2, 2, 0);

            var square = new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(4), new SimpleDoorModeGrid2D(2, 1));
            var rectangle = new RoomTemplateGrid2D(PolygonGrid2D.GetRectangle(2, 10), new SimpleDoorModeGrid2D(1, 1));

            var configurations = new List<Configuration>()
            {
                GetConfiguration(0, square, new Vector2Int(0, 0)),
                GetConfiguration(1, square, new Vector2Int(14, 0)),
                GetConfiguration(2, rectangle, new Vector2Int(8, -4)),
            };
            var expectedResult = GetExpectedResult(
                new PolygonGrid2DBuilder()
                    .AddPoint(3, 4)
                    .AddPoint(1, 4)
                    .AddPoint(1,8)
                    .AddPoint(17, 8)
                    .AddPoint(17, 4)
                    .AddPoint(15, 4)
                    .AddPoint(15, 6)
                    .AddPoint(3, 6)
                    .Build(),
                new DoorGrid2D(new Vector2Int(1, 4), new Vector2Int(3, 4)),
                new DoorGrid2D(new Vector2Int(15, 4), new Vector2Int(17, 4))
            );

            CheckResult(config, configurations, 0, 1,expectedResult, GetCurrentMethod());
        }

        [Test]
        public void TwoSquares_NextToEachOther_WithObstacle_WidthTwo_HeightOne()
        {
            var config = new Config(2, 1, 0);
            var square = new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(4), new SimpleDoorModeGrid2D(2, 1));
            var rectangle = new RoomTemplateGrid2D(PolygonGrid2D.GetRectangle(2, 10), new SimpleDoorModeGrid2D(1, 1));

            var configurations = new List<Configuration>()
            {
                GetConfiguration(0, square, new Vector2Int(0, 0)),
                GetConfiguration(1, square, new Vector2Int(14, 0)),
                GetConfiguration(2, rectangle, new Vector2Int(8, -4)),
            };
            var expectedResult = GetExpectedResult(
                new PolygonGrid2DBuilder()
                    .AddPoint(3, 4)
                    .AddPoint(1, 4)
                    .AddPoint(1,7)
                    .AddPoint(17, 7)
                    .AddPoint(17, 4)
                    .AddPoint(15, 4)
                    .AddPoint(15, 6)
                    .AddPoint(3, 6)
                    .Build(),
                new DoorGrid2D(new Vector2Int(1, 4), new Vector2Int(3, 4)),
                new DoorGrid2D(new Vector2Int(15, 4), new Vector2Int(17, 4))
            );

            CheckResult(config, configurations, 0, 1,expectedResult, GetCurrentMethod());
        }

        [Test]
        public void TwoSquares_NextToEachOther_WithObstacle_MinimumRoomDistanceOne()
        {
            var config = new Config(1, 1, 0);

            var square = new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(3), new SimpleDoorModeGrid2D(1, 1));
            var rectangle = new RoomTemplateGrid2D(PolygonGrid2D.GetRectangle(2, 10), new SimpleDoorModeGrid2D(1, 1));

            var configurations = new List<Configuration>()
            {
                GetConfiguration(0, square, new Vector2Int(0, 0)),
                GetConfiguration(1, square, new Vector2Int(11, 0)),
                GetConfiguration(2, rectangle, new Vector2Int(6, -6)),
            };
            var expectedResult = GetExpectedResult(
                new PolygonGrid2DBuilder()
                    .AddPoint(2, 3)
                    .AddPoint(1, 3)
                    .AddPoint(1, 6)
                    .AddPoint(13, 6)
                    .AddPoint(13, 3)
                    .AddPoint(12, 3)
                    .AddPoint(12, 5)
                    .AddPoint(2, 5)
                    .Build(),
                new DoorGrid2D(new Vector2Int(2, 3), new Vector2Int(1, 3)),
                new DoorGrid2D(new Vector2Int(13, 3), new Vector2Int(12, 3))
            );

            CheckResult(config, configurations, 0, 1,expectedResult, GetCurrentMethod(), true);
        }

        //private void CheckResult(CorridorsPathfinder<Layout, int, Configuration> pathfinder, List<Configuration> configurations, int fromRoom, int toRoom, CorridorsPathfinder<Layout, int, Configuration>.PathfindingResult expectedResult, string name, bool addRotations = true)
        //{
        //    var rotations = addRotations ? new List<int>() {0, 90, 180, 270} : new List<int>(){0};
        //    var layout = new Layout(configurations);

        //    foreach (var rotation in rotations)
        //    {
        //        var transformedLayout = GetTransformedLayout(layout, rotation);
        //        var expectedResultRotated = expectedResult == null ? null : GetTransformedResult(expectedResult, rotation);

        //        if (expectedResult != null)
        //        {
        //            SaveImage(transformedLayout, expectedResultRotated, $"{name}_{rotation}_0_expected");
        //        }

        //        var result = pathfinder.FindPath(transformedLayout, fromRoom, toRoom);

        //        if (result != null)
        //        {
        //            SaveImage(transformedLayout, result, $"{name}_{rotation}_1_actual");
        //        }

        //        if (expectedResult == null)
        //        {
        //            Assert.That(result, Is.Null);
        //        }
        //        else
        //        {
        //            Assert.That(result, Is.Not.Null);
        //            Assert.That(result.Outline.GetPoints(), Is.EquivalentTo(expectedResultRotated.Outline.GetPoints()));
        //            Assert.That(new List<Vector2Int>() { result.DoorFrom.From, result.DoorFrom.To }, Is.EquivalentTo(new List<Vector2Int>() { expectedResultRotated.DoorFrom.From, expectedResultRotated.DoorFrom.To }));
        //            Assert.That(new List<Vector2Int>() { result.DoorTo.From, result.DoorTo.To }, Is.EquivalentTo(new List<Vector2Int>() { expectedResultRotated.DoorTo.From, expectedResultRotated.DoorTo.To }));
        //        }
        //    }
        //}

        private void CheckResult(Config config, List<Configuration> configurations, int fromRoom, int toRoom, CorridorsPathfinder<Layout, int, Configuration>.PathfindingResult expectedResult, string name, bool addRotations = true)
        {
            var rotations = addRotations ? new List<int>() {0, 90, 180, 270} : new List<int>(){0};
            var layout = new Layout(configurations);

            foreach (var rotation in rotations)
            {
                var transformedLayout = GetTransformedLayout(layout, rotation);
                var expectedResultRotated = expectedResult == null ? null : GetTransformedResult(expectedResult, rotation);

                if (expectedResult != null)
                {
                    SaveImage(transformedLayout, expectedResultRotated, $"{name}_{rotation}_0_expected");
                }

                var pathfinder = config.Rotate(rotation).GetPathfinder();
                var result = pathfinder.FindPath(transformedLayout, fromRoom, toRoom);

                if (result != null)
                {
                    SaveImage(transformedLayout, result, $"{name}_{rotation}_1_actual");
                }

                if (expectedResult == null)
                {
                    Assert.That(result, Is.Null);
                }
                else
                {
                    Assert.That(result, Is.Not.Null);
                    Assert.That(result.Outline.GetPoints(), Is.EquivalentTo(expectedResultRotated.Outline.GetPoints()));
                    Assert.That(new List<Vector2Int>() { result.DoorFrom.From, result.DoorFrom.To }, Is.EquivalentTo(new List<Vector2Int>() { expectedResultRotated.DoorFrom.From, expectedResultRotated.DoorFrom.To }));
                    Assert.That(new List<Vector2Int>() { result.DoorTo.From, result.DoorTo.To }, Is.EquivalentTo(new List<Vector2Int>() { expectedResultRotated.DoorTo.From, expectedResultRotated.DoorTo.To }));
                }
            }
        }

        private Layout GetTransformedLayout(Layout layout, int rotation)
        {
            var configurations = layout.GetAllConfigurations().Select(x =>
            {
                var outline = x.RoomShape.RoomShape;
                var rotatedOutline = (outline).Rotate(rotation);

                var roomTemplate = RotateRoomTemplate(x.RoomShape.RoomTemplate, rotation);

                return new Configuration()
                {
                    Position = x.Position.RotateAroundCenter(rotation),
                    RoomShape = new RoomTemplateInstanceGrid2D(roomTemplate,
                        roomTemplate.Outline,
                        x.RoomShape.DoorLines
                            .Select(y => new DoorLineGrid2D(y.Line.Rotate(rotation), y.Length, y.DoorSocket)).ToList(),
                        new List<TransformationGrid2D>() {TransformationGrid2D.Identity}),
                    Room = x.Room,
                };
            }).ToList();

            return new Layout(configurations);
        }

        private CorridorsPathfinder<Layout, int, Configuration>.PathfindingResult GetTransformedResult(CorridorsPathfinder<Layout, int, Configuration>.PathfindingResult expectedResult, int rotation)
        {
            var doorFrom = new DoorGrid2D(expectedResult.DoorFrom.From.RotateAroundCenter(rotation), expectedResult.DoorFrom.To.RotateAroundCenter(rotation));
            var doorTo = new DoorGrid2D(expectedResult.DoorTo.From.RotateAroundCenter(rotation), expectedResult.DoorTo.To.RotateAroundCenter(rotation));

            return new CorridorsPathfinder<Layout, int, Configuration>.PathfindingResult(expectedResult.Outline.Rotate(rotation), doorFrom, doorTo);
        }

        private RoomTemplateGrid2D RotateRoomTemplate(RoomTemplateGrid2D roomTemplate, int rotation)
        {
            var outline = roomTemplate.Outline.Rotate(rotation);
            var doors = roomTemplate.Doors;

            if (doors is ManualDoorModeGrid2D manualDoorMode)
            {
                doors = new ManualDoorModeGrid2D(manualDoorMode.Doors.Select(x => new DoorGrid2D(x.From.RotateAroundCenter(rotation), x.To.RotateAroundCenter(rotation), x.Socket)).ToList());
            }

            return new RoomTemplateGrid2D(outline, doors);
        }

        private void SaveImage(Layout layout, CorridorsPathfinder<Layout, int, Configuration>.PathfindingResult result, string name)
        {
            var roomTemplates = new List<RoomTemplateGrid2D>();
            var positions = new List<Vector2Int>();

            foreach (var configuration in layout.GetAllConfigurations())
            {
                roomTemplates.Add(configuration.RoomShape.RoomTemplate);   
                positions.Add(configuration.Position);   
            }

            var corridorRoomTemplate = new RoomTemplateGrid2D(
                outline: result.Outline,
                doors: new ManualDoorModeGrid2D(new List<DoorGrid2D>()
                {
                    result.DoorFrom,
                    result.DoorTo,
                })
            );

            roomTemplates.Add(corridorRoomTemplate);
            positions.Add(new Vector2Int());

            Directory.CreateDirectory("tests");
            var roomTemplateDrawer = new CustomDrawer();
            roomTemplateDrawer.DrawRoomTemplates(roomTemplates, new DungeonDrawerOptions()
            {
                Width = 2000,
                Height = 2000,
                ShowRoomNames = false,
                EnableHatching = false,
            }, positions, null).Save(Path.Combine("tests", $"{name}.png"));

            if (result.Costs != null)
            {
                roomTemplateDrawer.DrawRoomTemplates(roomTemplates, new DungeonDrawerOptions()
                {
                    Width = 2000,
                    Height = 2000,
                    ShowRoomNames = false,
                    EnableHatching = false,
                    PaddingPercentage = 0.3f,
                }, positions, result.Costs).Save(Path.Combine("tests", $"{name}_costs.png"));
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }

        private CorridorsPathfinder<Layout, int, Configuration>.PathfindingResult GetExpectedResult(PolygonGrid2D outline, DoorGrid2D fromDoor, DoorGrid2D toDoor)
        {
            return new CorridorsPathfinder<Layout, int, Configuration>.PathfindingResult(outline, fromDoor, toDoor);
        }

        private static Configuration GetConfiguration(int room, RoomTemplateGrid2D roomTemplate, Vector2Int position)
        {
            return new Configuration()
            {
                RoomShape = new RoomTemplateInstanceGrid2D(roomTemplate, roomTemplate.Outline, roomTemplate.Doors.GetDoors(roomTemplate.Outline), new List<TransformationGrid2D>() {TransformationGrid2D.Identity}),
                Position = position,
                Room = room,
            };
        }

        public class Config
        {
            public int CorridorWidth { get; }

            public int CorridorHeight { get; }

            public int MinimumRoomDistance { get; }

            public Config(int corridorWidth, int corridorHeight, int minimumRoomDistance)
            {
                CorridorWidth = corridorWidth;
                CorridorHeight = corridorHeight;
                MinimumRoomDistance = minimumRoomDistance;
            }

            public Config Rotate(int degrees)
            {
                if (degrees == 180 || degrees == 0)
                {
                    return this;
                }

                return new Config(CorridorHeight, CorridorWidth, MinimumRoomDistance);
            }

            public CorridorsPathfinder<Layout, int, Configuration> GetPathfinder()
            {
                return new CorridorsPathfinder<Layout, int, Configuration>(CorridorWidth, CorridorHeight, MinimumRoomDistance);
            }
        }

        public class Layout : ILayout<int, Configuration>
        {
            private readonly List<Configuration> configurations;

            public IGraph<int> Graph => throw new NotImplementedException();

            public Layout(List<Configuration> configurations)
            {
                this.configurations = configurations;
            }

            public bool GetConfiguration(int node, out Configuration configuration)
            {
                configuration = configurations.Single(x => x.Room == node);
                return true;
            }

            public void SetConfiguration(int node, Configuration configuration)
            {
                throw new System.NotImplementedException();
            }

            public void RemoveConfiguration(int node)
            {
                throw new System.NotImplementedException();
            }

            public IEnumerable<Configuration> GetAllConfigurations()
            {
                return configurations;
            }
        }

        public class Configuration : IConfiguration<RoomTemplateInstanceGrid2D, Vector2Int, int>, ISmartCloneable<Configuration>
        {
            public RoomTemplateInstanceGrid2D RoomShape { get; set; }

            public Vector2Int Position { get; set; }

            public int Room { get; set; }

            public Configuration SmartClone()
            {
                throw new System.NotImplementedException();
            }
        }

        public class CustomDrawer : RoomTemplateDrawer
        {
            public Bitmap DrawRoomTemplates(List<RoomTemplateGrid2D> roomTemplates, DungeonDrawerOptions options,
                List<Vector2Int> positions, Dictionary<Vector2Int, int> costs)
            {
            var configurations = new List<RoomTemplateConfiguration>();

            for (var i = 0; i < roomTemplates.Count; i++)
            {
                var roomTemplate = roomTemplates[i];
                configurations.Add(new RoomTemplateConfiguration(roomTemplate, positions[i]));
            }

            var outlines = configurations.Select(x => x.RoomTemplate.Outline + x.Position).ToList();
            var boundingBox = DrawingUtils.GetBoundingBox(outlines);
            var (width, height, scale) = DrawingUtils.GetSize(boundingBox, options.Width, options.Height, options.Scale, options.PaddingAbsolute, options.PaddingPercentage);
            var offset = DrawingUtils.GetOffset(boundingBox, width, height, scale);

            bitmap = new Bitmap(width, height);
            graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            using (SolidBrush brush = new SolidBrush(Color.FromArgb(248, 248, 244)))
            {
                graphics.FillRectangle(brush, 0, 0, width, height);
            }
            
            var outlinePen = new Pen(Color.FromArgb(50, 50, 50), 0.2f)
            {
                EndCap = LineCap.Round,
                StartCap = LineCap.Round
            };

            var shadePen = new Pen(Color.FromArgb(204, 206, 206), 1.3f)
            {
                EndCap = LineCap.Round,
                StartCap = LineCap.Round
            };

            graphics.TranslateTransform(offset.X, height - offset.Y);
            graphics.ScaleTransform(scale, scale);
            graphics.ScaleTransform(1, -1);

            if (options.EnableShading)
            {
                foreach (var roomTemplate in configurations)
                {
                    DrawShading(GetOutline(roomTemplate.RoomTemplate.Outline, null, roomTemplate.Position), shadePen);
                }
            }

            if (options.EnableHatching)
            {
                var hatchingUsedPoints = new List<Tuple<RectangleGrid2D, List<Vector2>>>();
                foreach (var roomTemplate in configurations)
                {
                    DrawHatching(roomTemplate.RoomTemplate.Outline + roomTemplate.Position, hatchingUsedPoints, options.HatchingClusterOffset, options.HatchingLength);
                }
            }

            foreach (var roomTemplate in configurations)
            {
                DrawRoomBackground(roomTemplate.RoomTemplate.Outline + roomTemplate.Position);
                DrawGrid(roomTemplate.RoomTemplate.Outline + roomTemplate.Position);
                DrawOutline(roomTemplate.RoomTemplate.Outline + roomTemplate.Position, GetOutline(roomTemplate.RoomTemplate.Outline, null, roomTemplate.Position), outlinePen);
                DrawDoors(roomTemplate);
            }

            foreach (var roomTemplate in configurations)
            {
                if (options.ShowRoomNames)
                {
                    DrawTextOntoPolygon(roomTemplate.RoomTemplate.Outline + roomTemplate.Position, roomTemplate.RoomTemplate.Name, options.FontSize);
                }
            }

            if (true)
            {
                var radius = 0.33f;
                graphics.DrawEllipse(outlinePen, -radius, -radius, 2 * radius, 2 * radius);
            }

            if (costs != null)
            {
                var oldTransform = graphics.Transform;
                
                using (var font = new Font("Baskerville Old Face", 0.4f, FontStyle.Regular, GraphicsUnit.Pixel))
                {
                    foreach (var pair in costs)
                    {
                        var position = pair.Key;
                        var cost = pair.Value;

                        var rect = new RectangleF(
                            -0.25f,
                            -0.25f,
                            0.75f,
                            0.75f
                        );

                        var sf = new StringFormat
                        {
                            LineAlignment = StringAlignment.Center,
                            Alignment = StringAlignment.Center
                        };

                        graphics.TranslateTransform(position.X, position.Y);
                        graphics.ScaleTransform(1, -1);
                        
                        graphics.DrawString(cost.ToString(), font, Brushes.Black, rect, sf);

                        graphics.Transform = oldTransform;
                    }
                }

                
            }


            shadePen.Dispose();
            outlinePen.Dispose();

            return bitmap;
            }
        }
    }
}