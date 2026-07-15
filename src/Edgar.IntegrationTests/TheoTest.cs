using System.Collections.Generic;
using System.IO;
using System.Linq;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.Legacy.Utils;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Edgar.IntegrationTests
{
    [TestFixture]
    public class TheoTest
    {
        public record Room
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        [Test]
        public void Run()
        {
            var levelDescription = LevelDescriptionGrid2D<Room>.LoadFromJson(
                "C:\\Users\\nepozitek\\Downloads\\Desert_LevelGenerator_raw.json");

            //var roomTemplates = new List<RoomTemplateGrid2D>();
            //var corridorRoomTemplates = new List<RoomTemplateGrid2D>();
            //var corridorRoomDescription = default(RoomDescriptionGrid2D);

            //foreach (var file in files)
            //{
            //    var content = File.ReadAllText(file);
            //    content = content.Replace("EdgarSingleFile", "Edgar");
            //    var roomDescription = JsonConvert.DeserializeObject<RoomDescriptionGrid2D>(content, new JsonSerializerSettings
            //    {
            //        TypeNameHandling = TypeNameHandling.Auto
            //    });

            //    var roomTemplatesList = roomDescription.IsCorridor ? corridorRoomTemplates : roomTemplates;

            //    foreach (var roomTemplate in roomDescription.RoomTemplates)
            //    {
            //        if (roomTemplatesList.All(x => x.Name != roomTemplate.Name))
            //        {
            //            roomTemplatesList.Add(roomTemplate);
            //        }
            //    }

            //    if (roomDescription.IsCorridor)
            //    {
            //        corridorRoomDescription = roomDescription;
            //    }
            //}

            //var levelDescription = new LevelDescriptionGrid2D<int>();
            //for (int i = 0; i < 10; i++)
            //{
            //    levelDescription.AddRoom(i, new RoomDescriptionGrid2D(false, roomTemplates));

            //    if (i > 0)
            //    {
            //        var corridorRoom = 100 + i;
            //        levelDescription.AddRoom(corridorRoom, corridorRoomDescription);
            //        levelDescription.AddConnection(i - 1, corridorRoom);
            //        levelDescription.AddConnection(i, corridorRoom);
            //    }
            //}

            var generator = new GraphBasedGeneratorGrid2D<Room>(levelDescription);

            for (int i = 0; i < 100; i++)
            {
                var layout = generator.GenerateLayout();
                var s = 1;
            }

        }
    }
}