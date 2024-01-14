﻿// <auto-generated/>

#nullable enable annotations
#nullable disable warnings

// Suppress warnings about [Obsolete] member usage in generated code.
#pragma warning disable CS0612, CS0618

namespace Edgar.NativeAotLib
{
    public partial class LevelDescriptionContext
    {
        private global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<global::System.Collections.Generic.List<global::Edgar.Geometry.Vector2Int>>? _ListVector2Int;
        
        /// <summary>
        /// Defines the source generated JSON serialization contract metadata for a given type.
        /// </summary>
        public global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<global::System.Collections.Generic.List<global::Edgar.Geometry.Vector2Int>> ListVector2Int
        {
            get => _ListVector2Int ??= (global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<global::System.Collections.Generic.List<global::Edgar.Geometry.Vector2Int>>)Options.GetTypeInfo(typeof(global::System.Collections.Generic.List<global::Edgar.Geometry.Vector2Int>));
        }
        
        private global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<global::System.Collections.Generic.List<global::Edgar.Geometry.Vector2Int>> Create_ListVector2Int(global::System.Text.Json.JsonSerializerOptions options)
        {
            if (!TryGetTypeInfoForRuntimeCustomConverter<global::System.Collections.Generic.List<global::Edgar.Geometry.Vector2Int>>(options, out global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<global::System.Collections.Generic.List<global::Edgar.Geometry.Vector2Int>> jsonTypeInfo))
            {
                var info = new global::System.Text.Json.Serialization.Metadata.JsonCollectionInfoValues<global::System.Collections.Generic.List<global::Edgar.Geometry.Vector2Int>>
                {
                    ObjectCreator = () => new global::System.Collections.Generic.List<global::Edgar.Geometry.Vector2Int>(),
                    SerializeHandler = ListVector2IntSerializeHandler
                };
                
                jsonTypeInfo = global::System.Text.Json.Serialization.Metadata.JsonMetadataServices.CreateListInfo<global::System.Collections.Generic.List<global::Edgar.Geometry.Vector2Int>, global::Edgar.Geometry.Vector2Int>(options, info);
                jsonTypeInfo.NumberHandling = null;
            }
        
            jsonTypeInfo.OriginatingResolver = this;
            return jsonTypeInfo;
        }

        // Intentionally not a static method because we create a delegate to it. Invoking delegates to instance
        // methods is almost as fast as virtual calls. Static methods need to go through a shuffle thunk.
        private void ListVector2IntSerializeHandler(global::System.Text.Json.Utf8JsonWriter writer, global::System.Collections.Generic.List<global::Edgar.Geometry.Vector2Int>? value)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }
            
            writer.WriteStartArray();

            for (int i = 0; i < value.Count; i++)
            {
                Vector2IntSerializeHandler(writer, value[i]);
            }

            writer.WriteEndArray();
        }
    }
}
