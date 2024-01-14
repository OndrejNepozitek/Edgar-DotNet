using Edgar.GraphBasedGenerator.Grid2D;
using System.Text.Json.Serialization;
using System.Text.Json;
using Edgar.Legacy.Core.MapLayouts;
using System.Collections.Generic;

namespace Edgar.Utils
{
    public static class JsonUtils2
    {
        public static string SerializeToJson(LevelDescriptionGrid2D<int> levelDescription)
        {
            var options = new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true,
                IncludeFields = true,
            };

            var json = JsonSerializer.Serialize(levelDescription, options);

            return json;
        }

        public static LayoutGrid2DModel<int> DeserializeFromJson(string json)
        {
            var options = new JsonSerializerOptions()
            {
                ReferenceHandler = null,
                WriteIndented = true,
                IncludeFields = true,
            };

            var layout = JsonSerializer.Deserialize<LayoutGrid2DModel<int>>(json, options);

            return layout;
        }

        public class LayoutGrid2DModel<TRoom>
        {
            public List<LayoutRoomGrid2D<TRoom>> Rooms { get; set; }
        }
    }
}