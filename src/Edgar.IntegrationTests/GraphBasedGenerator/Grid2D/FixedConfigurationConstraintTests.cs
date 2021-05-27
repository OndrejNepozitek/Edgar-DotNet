using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common;
using Edgar.GraphBasedGenerator.Common.Doors;
using Edgar.GraphBasedGenerator.Grid2D;
using NUnit.Framework;

namespace Edgar.IntegrationTests.GraphBasedGenerator.Grid2D
{
    [TestFixture]
    public class FixedConfigurationConstraintTests
    {

        [Test]
        public void FixedPosition_Path_OneRoom_StartOfChain()
        {
            var roomTemplate1 = new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(5), new SimpleDoorModeGrid2D(1, 0));

            var levelDescription = new LevelDescriptionGrid2D<int>();

            levelDescription.AddRoom(0, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(1, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(2, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(3, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddConnection(0, 1);
            levelDescription.AddConnection(1, 2);
            levelDescription.AddConnection(2, 3);

            levelDescription.Constraints = new List<IGeneratorConstraintGrid2D<int>>()
            {
                new FixedConfigurationConstraint<int>()
                {
                    Room = 0,
                    Position = new Vector2Int(20, 20),
                    RoomTemplate = roomTemplate1,
                }
            };

            var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription, new GraphBasedGeneratorConfiguration<int>()
            {
                EarlyStopIfTimeExceeded = TimeSpan.FromSeconds(2),
            });
            generator.InjectRandomGenerator(new Random(0));

            var layout = generator.GenerateLayout();

            Assert.That(layout, Is.Not.Null, "Layout not generated");

            {
                // Room 0

                var room = layout.Rooms.Single(x => x.Room == 0);

                Assert.That(room.Position, Is.EqualTo(new Vector2Int(20, 20)));
            }
        }

        [Test]
        public void FixedPosition_Path_OneRoom_DifferentTransformation_FromTwoPossible()
        {
            var roomTemplate1 = new RoomTemplateGrid2D(PolygonGrid2D.GetRectangle(5, 10), new SimpleDoorModeGrid2D(1, 0), allowedTransformations: new List<TransformationGrid2D>()
                {
                    TransformationGrid2D.Identity,
                    TransformationGrid2D.Rotate90
                });

            var levelDescription = new LevelDescriptionGrid2D<int>();

            levelDescription.AddRoom(0, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(1, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(2, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(3, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddConnection(0, 1);
            levelDescription.AddConnection(1, 2);
            levelDescription.AddConnection(2, 3);

            levelDescription.Constraints = new List<IGeneratorConstraintGrid2D<int>>()
            {
                new FixedConfigurationConstraint<int>()
                {
                    Room = 0,
                    Position = new Vector2Int(20, 20),
                    RoomTemplate = roomTemplate1,
                    Transformation = TransformationGrid2D.Rotate90,
                }
            };

            var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription, new GraphBasedGeneratorConfiguration<int>()
            {
                EarlyStopIfTimeExceeded = TimeSpan.FromSeconds(2),
            });
            generator.InjectRandomGenerator(new Random(0));

            var layout = generator.GenerateLayout();

            Assert.That(layout, Is.Not.Null, "Layout not generated");

            {
                // Room 0

                var room = layout.Rooms.Single(x => x.Room == 0);

                Assert.That(room.Position, Is.EqualTo(new Vector2Int(20, 20)));
                Assert.That(room.Transformation, Is.EqualTo(TransformationGrid2D.Rotate90));
            }
        }

        [Test]
        public void FixedPosition_Path_OneRoom_DifferentTransformation_FromAllPossible()
        {
            var roomTemplate1 = new RoomTemplateGrid2D(PolygonGrid2D.GetRectangle(5, 10), new SimpleDoorModeGrid2D(1, 0), allowedTransformations: TransformationGrid2DHelper.GetAll());

            var levelDescription = new LevelDescriptionGrid2D<int>();

            levelDescription.AddRoom(0, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(1, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(2, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(3, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddConnection(0, 1);
            levelDescription.AddConnection(1, 2);
            levelDescription.AddConnection(2, 3);

            levelDescription.Constraints = new List<IGeneratorConstraintGrid2D<int>>()
            {
                new FixedConfigurationConstraint<int>()
                {
                    Room = 0,
                    Position = new Vector2Int(20, 20),
                    RoomTemplate = roomTemplate1,
                    Transformation = TransformationGrid2D.Rotate90,
                }
            };

            var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription, new GraphBasedGeneratorConfiguration<int>()
            {
                EarlyStopIfTimeExceeded = TimeSpan.FromSeconds(2),
            });
            generator.InjectRandomGenerator(new Random(0));

            var layout = generator.GenerateLayout();

            Assert.That(layout, Is.Not.Null, "Layout not generated");

            {
                // Room 0

                var room = layout.Rooms.Single(x => x.Room == 0);

                Assert.That(room.Transformation, Is.EqualTo(TransformationGrid2D.Rotate90));
                Assert.That(room.Position, Is.EqualTo(new Vector2Int(20, 20)));
            }
        }

        [Test]
        public void FixedShapeAndPosition_Path_OneRoom_StartOfChain_NotNormalizedRoomTemplate()
        {
            var roomTemplate1 = new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(5) + new Vector2Int(5, 10), new SimpleDoorModeGrid2D(1, 0));

            var levelDescription = new LevelDescriptionGrid2D<int>();

            levelDescription.AddRoom(0, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(1, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(2, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(3, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddConnection(0, 1);
            levelDescription.AddConnection(1, 2);
            levelDescription.AddConnection(2, 3);

            levelDescription.Constraints = new List<IGeneratorConstraintGrid2D<int>>()
            {
                new FixedConfigurationConstraint<int>()
                {
                    Room = 0,
                    Position = new Vector2Int(20, 20),
                    RoomTemplate = roomTemplate1,
                }
            };

            var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription, new GraphBasedGeneratorConfiguration<int>()
            {
                EarlyStopIfTimeExceeded = TimeSpan.FromSeconds(2),
            });
            generator.InjectRandomGenerator(new Random(0));

            var layout = generator.GenerateLayout();

            Assert.That(layout, Is.Not.Null, "Layout not generated");

            {
                // Room 0

                var room = layout.Rooms.Single(x => x.Room == 0);

                Assert.That(room.Position, Is.EqualTo(new Vector2Int(20, 20)));
            }
        }

        [Test]
        public void FixedPosition_Path_OneRoom_InsideOfChain()
        {
            var roomTemplate1 = new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(5), new SimpleDoorModeGrid2D(1, 0));

            var levelDescription = new LevelDescriptionGrid2D<int>();

            levelDescription.AddRoom(0, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(1, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(2, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(3, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddConnection(0, 1);
            levelDescription.AddConnection(1, 2);
            levelDescription.AddConnection(2, 3);

            levelDescription.Constraints = new List<IGeneratorConstraintGrid2D<int>>()
            {
                new FixedConfigurationConstraint<int>()
                {
                    Room = 1,
                    Position = new Vector2Int(20, 20),
                    RoomTemplate = roomTemplate1,
                }
            };

            var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription, new GraphBasedGeneratorConfiguration<int>()
            {
                EarlyStopIfTimeExceeded = TimeSpan.FromSeconds(2),
            });
            generator.InjectRandomGenerator(new Random(0));

            var layout = generator.GenerateLayout();

            Assert.That(layout, Is.Not.Null, "Layout not generated");

            {
                // Room 1

                var room = layout.Rooms.Single(x => x.Room == 1);

                Assert.That(room.Position, Is.EqualTo(new Vector2Int(20, 20)));
            }
        }

        [Test]
        public void FixedPosition_Path_TwoRooms_BothEnds()
        {
            var roomTemplate1 = new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(5), new SimpleDoorModeGrid2D(1, 0));

            var levelDescription = new LevelDescriptionGrid2D<int>();

            levelDescription.AddRoom(0, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(1, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(2, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(3, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddConnection(0, 1);
            levelDescription.AddConnection(1, 2);
            levelDescription.AddConnection(2, 3);

            levelDescription.Constraints = new List<IGeneratorConstraintGrid2D<int>>()
            {
                new FixedConfigurationConstraint<int>()
                {
                    Room = 0,
                    Position = new Vector2Int(0, 0),
                    RoomTemplate = roomTemplate1,
                },
                new FixedConfigurationConstraint<int>()
                {
                    Room = 3,
                    Position = new Vector2Int(10, -5),
                    RoomTemplate = roomTemplate1,
                },
            };

            var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription, new GraphBasedGeneratorConfiguration<int>()
            {
                EarlyStopIfTimeExceeded = TimeSpan.FromSeconds(2),
            });
            generator.InjectRandomGenerator(new Random(0));

            var layout = generator.GenerateLayout();

            Assert.That(layout, Is.Not.Null, "Layout not generated");

            {
                // Room 0

                var room = layout.Rooms.Single(x => x.Room == 0);

                Assert.That(room.Position, Is.EqualTo(new Vector2Int(0, 0)));
            }

            {
                // Room 3

                var room = layout.Rooms.Single(x => x.Room == 3);

                Assert.That(room.Position, Is.EqualTo(new Vector2Int(10, -5)));
            }
        }

        [Test]
        public void FixedShapeAndPosition_Path_TwoRooms_BothEnds()
        {
            var roomTemplate1 = new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(5), new SimpleDoorModeGrid2D(1, 0));
            var roomTemplate2 = new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(6), new SimpleDoorModeGrid2D(1, 0));
            var roomTemplate3 = new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(7), new SimpleDoorModeGrid2D(1, 0));
            var roomTemplates = new List<RoomTemplateGrid2D>()
            {
                roomTemplate1, roomTemplate2, roomTemplate3
            };

            var levelDescription = new LevelDescriptionGrid2D<int>();

            levelDescription.AddRoom(0, new RoomDescriptionGrid2D(false, roomTemplates));
            levelDescription.AddRoom(1, new RoomDescriptionGrid2D(false, roomTemplates));
            levelDescription.AddRoom(2, new RoomDescriptionGrid2D(false, roomTemplates));
            levelDescription.AddRoom(3, new RoomDescriptionGrid2D(false, roomTemplates));
            levelDescription.AddConnection(0, 1);
            levelDescription.AddConnection(1, 2);
            levelDescription.AddConnection(2, 3);

            levelDescription.Constraints = new List<IGeneratorConstraintGrid2D<int>>()
            {
                new FixedConfigurationConstraint<int>()
                {
                    Room = 0,
                    Position = new Vector2Int(0, 0),
                    RoomTemplate = roomTemplate1,
                },
                new FixedConfigurationConstraint<int>()
                {
                    Room = 3,
                    Position = new Vector2Int(10, -5),
                    RoomTemplate = roomTemplate3,
                },
            };

            var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription, new GraphBasedGeneratorConfiguration<int>()
            {
                EarlyStopIfTimeExceeded = TimeSpan.FromSeconds(2),
            });
            generator.InjectRandomGenerator(new Random(0));

            var layout = generator.GenerateLayout();

            Assert.That(layout, Is.Not.Null, "Layout not generated");

            {
                // Room 0

                var room = layout.Rooms.Single(x => x.Room == 0);

                Assert.That(room.Position, Is.EqualTo(new Vector2Int(0, 0)));
                Assert.That(room.RoomTemplate, Is.EqualTo(roomTemplate1));
            }

            {
                // Room 3

                var room = layout.Rooms.Single(x => x.Room == 3);

                Assert.That(room.Position, Is.EqualTo(new Vector2Int(10, -5)));
                Assert.That(room.RoomTemplate, Is.EqualTo(roomTemplate3));
            }
        }

        [Test]
        public void FixedPositionAndShape_Path_TwoRooms_BothEnds()
        {
            var roomTemplate1 = new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(5), new SimpleDoorModeGrid2D(1, 0));

            var levelDescription = new LevelDescriptionGrid2D<int>();

            levelDescription.AddRoom(0, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(1, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(2, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(3, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddConnection(0, 1);
            levelDescription.AddConnection(1, 2);
            levelDescription.AddConnection(2, 3);

            levelDescription.Constraints = new List<IGeneratorConstraintGrid2D<int>>()
            {
                new FixedConfigurationConstraint<int>()
                {
                    Room = 0,
                    Position = new Vector2Int(0, 0),
                    RoomTemplate = roomTemplate1,
                },
                new FixedConfigurationConstraint<int>()
                {
                    Room = 3,
                    Position = new Vector2Int(10, -5),
                    RoomTemplate = roomTemplate1,
                },
            };

            var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription, new GraphBasedGeneratorConfiguration<int>()
            {
                EarlyStopIfTimeExceeded = TimeSpan.FromSeconds(2),
            });
            generator.InjectRandomGenerator(new Random(0));

            var layout = generator.GenerateLayout();

            Assert.That(layout, Is.Not.Null, "Layout not generated");

            {
                // Room 0

                var room = layout.Rooms.Single(x => x.Room == 0);

                Assert.That(room.Position, Is.EqualTo(new Vector2Int(0, 0)));
            }

            {
                // Room 3

                var room = layout.Rooms.Single(x => x.Room == 3);

                Assert.That(room.Position, Is.EqualTo(new Vector2Int(10, -5)));
            }
        }

        [Test]
        public void FixedPositionAndShape_PathExtended_TwoRooms_BothEnds()
        {
            var roomTemplate1 = new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(5), new SimpleDoorModeGrid2D(1, 0));

            var levelDescription = new LevelDescriptionGrid2D<int>();

            levelDescription.AddRoom(0, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(1, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(2, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(3, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddConnection(0, 1);
            levelDescription.AddConnection(1, 2);
            levelDescription.AddConnection(2, 3);

            // Extended path
            levelDescription.AddRoom(4, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddConnection(3, 4);

            levelDescription.Constraints = new List<IGeneratorConstraintGrid2D<int>>()
            {
                new FixedConfigurationConstraint<int>()
                {
                    Room = 0,
                    Position = new Vector2Int(0, 0),
                    RoomTemplate = roomTemplate1,
                },
                new FixedConfigurationConstraint<int>()
                {
                    Room = 3,
                    Position = new Vector2Int(10, -5),
                    RoomTemplate = roomTemplate1,
                },
                new FixedConfigurationConstraint<int>()
                {
                    Room = 4,
                    Position = new Vector2Int(15, -5),
                    RoomTemplate = roomTemplate1,
                },
            };

            var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription, new GraphBasedGeneratorConfiguration<int>()
            {
                EarlyStopIfTimeExceeded = TimeSpan.FromSeconds(2),
            });
            generator.InjectRandomGenerator(new Random(0));

            var layout = generator.GenerateLayout();

            Assert.That(layout, Is.Not.Null, "Layout not generated");

            {
                // Room 0

                var room = layout.Rooms.Single(x => x.Room == 0);

                Assert.That(room.Position, Is.EqualTo(new Vector2Int(0, 0)));
            }

            {
                // Room 3

                var room = layout.Rooms.Single(x => x.Room == 3);

                Assert.That(room.Position, Is.EqualTo(new Vector2Int(10, -5)));
            }

            {
                // Room 4

                var room = layout.Rooms.Single(x => x.Room == 4);

                Assert.That(room.Position, Is.EqualTo(new Vector2Int(15, -5)));
            }
        }

        [Test]
        public void FixedPosition_Cycle_OneRoom()
        {
            var roomTemplate1 = new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(5), new SimpleDoorModeGrid2D(1, 0));

            var levelDescription = new LevelDescriptionGrid2D<int>();

            levelDescription.AddRoom(0, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(1, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(2, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(3, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddConnection(0, 1);
            levelDescription.AddConnection(1, 2);
            levelDescription.AddConnection(2, 3);
            levelDescription.AddConnection(3, 0);

            levelDescription.Constraints = new List<IGeneratorConstraintGrid2D<int>>()
            {
                new FixedConfigurationConstraint<int>()
                {
                    Room = 1,
                    Position = new Vector2Int(20, 20),
                    RoomTemplate = roomTemplate1,
                }
            };

            var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription, new GraphBasedGeneratorConfiguration<int>()
            {
                EarlyStopIfTimeExceeded = TimeSpan.FromSeconds(2),
            });
            generator.InjectRandomGenerator(new Random(0));

            var layout = generator.GenerateLayout();

            Assert.That(layout, Is.Not.Null, "Layout not generated");

            {
                // Room 1

                var room = layout.Rooms.Single(x => x.Room == 1);

                Assert.That(room.Position, Is.EqualTo(new Vector2Int(20, 20)));
            }
        }

        [Test]
        public void FixedPosition_Path_InvalidCorridor()
        {
            var roomTemplate1 = new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(5), new SimpleDoorModeGrid2D(1, 0));

            var levelDescription = new LevelDescriptionGrid2D<int>();

            levelDescription.AddRoom(0, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(1, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(2, new RoomDescriptionGrid2D(true, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(3, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(4, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddConnection(0, 1);
            levelDescription.AddConnection(1, 2);
            levelDescription.AddConnection(2, 3);
            levelDescription.AddConnection(3, 4);

            levelDescription.Constraints = new List<IGeneratorConstraintGrid2D<int>>()
            {
                new FixedConfigurationConstraint<int>()
                {
                    Room = 2,
                    Position = new Vector2Int(20, 20),
                    RoomTemplate = roomTemplate1,
                }
            };

            Assert.Throws<InvalidOperationException>(() =>
            {
                var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription,
                    new GraphBasedGeneratorConfiguration<int>()
                    {
                        EarlyStopIfTimeExceeded = TimeSpan.FromSeconds(2),
                    });
            });
        }

        [Test]
        public void FixedPosition_Path_CorridorAndNeighbors()
        {
            var roomTemplate1 = new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(5), new SimpleDoorModeGrid2D(1, 0));

            var levelDescription = new LevelDescriptionGrid2D<int>();

            levelDescription.AddRoom(0, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(1, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(2, new RoomDescriptionGrid2D(true, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(3, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(4, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddConnection(0, 1);
            levelDescription.AddConnection(1, 2);
            levelDescription.AddConnection(2, 3);
            levelDescription.AddConnection(3, 4);

            levelDescription.Constraints = new List<IGeneratorConstraintGrid2D<int>>()
            {
                new FixedConfigurationConstraint<int>()
                {
                    Room = 1,
                    Position = new Vector2Int(15, 20),
                    RoomTemplate = roomTemplate1,
                },
                new FixedConfigurationConstraint<int>()
                {
                    Room = 2,
                    Position = new Vector2Int(20, 20),
                    RoomTemplate = roomTemplate1,
                },
                new FixedConfigurationConstraint<int>()
                {
                    Room = 3,
                    Position = new Vector2Int(25, 20),
                    RoomTemplate = roomTemplate1,
                },
            };

            var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription, new GraphBasedGeneratorConfiguration<int>()
            {
                EarlyStopIfTimeExceeded = TimeSpan.FromSeconds(2),
            });
            generator.InjectRandomGenerator(new Random(0));

            var layout = generator.GenerateLayout();

            Assert.That(layout, Is.Not.Null, "Layout not generated");

            {
                // Room 1

                var room = layout.Rooms.Single(x => x.Room == 1);

                Assert.That(room.Position, Is.EqualTo(new Vector2Int(15, 20)));
            }

            {
                // Room 2

                var room = layout.Rooms.Single(x => x.Room == 2);

                Assert.That(room.Position, Is.EqualTo(new Vector2Int(20, 20)));
            }

            {
                // Room 3

                var room = layout.Rooms.Single(x => x.Room == 3);

                Assert.That(room.Position, Is.EqualTo(new Vector2Int(25, 20)));
            }
        }
    }
}