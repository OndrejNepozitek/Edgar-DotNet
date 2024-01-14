using System.Text.Json;
using System.Text.Json.Serialization;
using Edgar.GraphBasedGenerator.Grid2D;

namespace Edgar.NativeAotLib
{
    public static class SystemTextJsonUtils
    {
        public static string SerializeToJson(LevelDescriptionGrid2D<int> levelDescription)
        {
            var options = new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true,
                TypeInfoResolver = LevelDescriptionContext.Default,
                IncludeFields = true,
            };

            var json = JsonSerializer.Serialize(levelDescription, options);

            return json;
        }

        public static string SerializeToJson(LayoutGrid2D<int> layout)
        {
            var options = new JsonSerializerOptions()
            {
                ReferenceHandler = null,
                WriteIndented = true,
                TypeInfoResolver = LayoutContext.Default,
                IncludeFields = true,
            };

            var json = JsonSerializer.Serialize(layout, options);

            return json;
        }

        public static LevelDescriptionGrid2D<int> DeserializeFromJson(string json)
        {
            var options = new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true,
                TypeInfoResolver = LevelDescriptionContext.Default,
                IncludeFields = true,
            };

            var levelDescription = JsonSerializer.Deserialize<LevelDescriptionGrid2D<int>>(json, options);

            return levelDescription;
        }
    }

    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(LevelDescriptionGrid2D<int>))]
    public partial class LevelDescriptionContext : JsonSerializerContext {

    }

    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(LayoutGrid2D<int>))]
    public partial class LayoutContext : JsonSerializerContext
    {

    }
}