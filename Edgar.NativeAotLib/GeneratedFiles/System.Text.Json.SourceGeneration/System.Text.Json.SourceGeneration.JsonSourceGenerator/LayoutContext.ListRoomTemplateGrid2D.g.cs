﻿// <auto-generated/>

#nullable enable annotations
#nullable disable warnings

// Suppress warnings about [Obsolete] member usage in generated code.
#pragma warning disable CS0612, CS0618

namespace Edgar.NativeAotLib
{
    public partial class LayoutContext
    {
        private global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<global::System.Collections.Generic.List<global::Edgar.GraphBasedGenerator.Grid2D.RoomTemplateGrid2D>>? _ListRoomTemplateGrid2D;
        
        /// <summary>
        /// Defines the source generated JSON serialization contract metadata for a given type.
        /// </summary>
        public global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<global::System.Collections.Generic.List<global::Edgar.GraphBasedGenerator.Grid2D.RoomTemplateGrid2D>> ListRoomTemplateGrid2D
        {
            get => _ListRoomTemplateGrid2D ??= (global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<global::System.Collections.Generic.List<global::Edgar.GraphBasedGenerator.Grid2D.RoomTemplateGrid2D>>)Options.GetTypeInfo(typeof(global::System.Collections.Generic.List<global::Edgar.GraphBasedGenerator.Grid2D.RoomTemplateGrid2D>));
        }
        
        private global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<global::System.Collections.Generic.List<global::Edgar.GraphBasedGenerator.Grid2D.RoomTemplateGrid2D>> Create_ListRoomTemplateGrid2D(global::System.Text.Json.JsonSerializerOptions options)
        {
            if (!TryGetTypeInfoForRuntimeCustomConverter<global::System.Collections.Generic.List<global::Edgar.GraphBasedGenerator.Grid2D.RoomTemplateGrid2D>>(options, out global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<global::System.Collections.Generic.List<global::Edgar.GraphBasedGenerator.Grid2D.RoomTemplateGrid2D>> jsonTypeInfo))
            {
                var info = new global::System.Text.Json.Serialization.Metadata.JsonCollectionInfoValues<global::System.Collections.Generic.List<global::Edgar.GraphBasedGenerator.Grid2D.RoomTemplateGrid2D>>
                {
                    ObjectCreator = () => new global::System.Collections.Generic.List<global::Edgar.GraphBasedGenerator.Grid2D.RoomTemplateGrid2D>(),
                    SerializeHandler = ListRoomTemplateGrid2DSerializeHandler
                };
                
                jsonTypeInfo = global::System.Text.Json.Serialization.Metadata.JsonMetadataServices.CreateListInfo<global::System.Collections.Generic.List<global::Edgar.GraphBasedGenerator.Grid2D.RoomTemplateGrid2D>, global::Edgar.GraphBasedGenerator.Grid2D.RoomTemplateGrid2D>(options, info);
                jsonTypeInfo.NumberHandling = null;
            }
        
            jsonTypeInfo.OriginatingResolver = this;
            return jsonTypeInfo;
        }

        // Intentionally not a static method because we create a delegate to it. Invoking delegates to instance
        // methods is almost as fast as virtual calls. Static methods need to go through a shuffle thunk.
        private void ListRoomTemplateGrid2DSerializeHandler(global::System.Text.Json.Utf8JsonWriter writer, global::System.Collections.Generic.List<global::Edgar.GraphBasedGenerator.Grid2D.RoomTemplateGrid2D>? value)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }
            
            writer.WriteStartArray();

            for (int i = 0; i < value.Count; i++)
            {
                RoomTemplateGrid2DSerializeHandler(writer, value[i]);
            }

            writer.WriteEndArray();
        }
    }
}
