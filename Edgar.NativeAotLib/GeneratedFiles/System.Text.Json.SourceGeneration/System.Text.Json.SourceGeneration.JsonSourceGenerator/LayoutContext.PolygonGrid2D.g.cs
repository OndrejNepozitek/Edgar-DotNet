﻿// <auto-generated/>

#nullable enable annotations
#nullable disable warnings

// Suppress warnings about [Obsolete] member usage in generated code.
#pragma warning disable CS0612, CS0618

namespace Edgar.NativeAotLib
{
    public partial class LayoutContext
    {
        private global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<global::Edgar.Geometry.PolygonGrid2D>? _PolygonGrid2D;
        
        /// <summary>
        /// Defines the source generated JSON serialization contract metadata for a given type.
        /// </summary>
        public global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<global::Edgar.Geometry.PolygonGrid2D> PolygonGrid2D
        {
            get => _PolygonGrid2D ??= (global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<global::Edgar.Geometry.PolygonGrid2D>)Options.GetTypeInfo(typeof(global::Edgar.Geometry.PolygonGrid2D));
        }
        
        private global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<global::Edgar.Geometry.PolygonGrid2D> Create_PolygonGrid2D(global::System.Text.Json.JsonSerializerOptions options)
        {
            if (!TryGetTypeInfoForRuntimeCustomConverter<global::Edgar.Geometry.PolygonGrid2D>(options, out global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<global::Edgar.Geometry.PolygonGrid2D> jsonTypeInfo))
            {
                var objectInfo = new global::System.Text.Json.Serialization.Metadata.JsonObjectInfoValues<global::Edgar.Geometry.PolygonGrid2D>
                {
                    ObjectCreator = () => new global::Edgar.Geometry.PolygonGrid2D(),
                    ObjectWithParameterizedConstructorCreator = null,
                    PropertyMetadataInitializer = _ => PolygonGrid2DPropInit(options),
                    ConstructorParameterMetadataInitializer = null,
                    SerializeHandler = PolygonGrid2DSerializeHandler
                };
                
                jsonTypeInfo = global::System.Text.Json.Serialization.Metadata.JsonMetadataServices.CreateObjectInfo<global::Edgar.Geometry.PolygonGrid2D>(options, objectInfo);
                jsonTypeInfo.NumberHandling = null;
            }
        
            jsonTypeInfo.OriginatingResolver = this;
            return jsonTypeInfo;
        }

        private static global::System.Text.Json.Serialization.Metadata.JsonPropertyInfo[] PolygonGrid2DPropInit(global::System.Text.Json.JsonSerializerOptions options)
        {
            var properties = new global::System.Text.Json.Serialization.Metadata.JsonPropertyInfo[2];

            var info0 = new global::System.Text.Json.Serialization.Metadata.JsonPropertyInfoValues<global::Edgar.Geometry.RectangleGrid2D>
            {
                IsProperty = true,
                IsPublic = true,
                IsVirtual = false,
                DeclaringType = typeof(global::Edgar.Geometry.PolygonGrid2D),
                Converter = null,
                Getter = null,
                Setter = null,
                IgnoreCondition = global::System.Text.Json.Serialization.JsonIgnoreCondition.Always,
                HasJsonInclude = false,
                IsExtensionData = false,
                NumberHandling = null,
                PropertyName = "BoundingRectangle",
                JsonPropertyName = null
            };
            
            properties[0] = global::System.Text.Json.Serialization.Metadata.JsonMetadataServices.CreatePropertyInfo<global::Edgar.Geometry.RectangleGrid2D>(options, info0);

            var info1 = new global::System.Text.Json.Serialization.Metadata.JsonPropertyInfoValues<global::System.Collections.Generic.List<global::Edgar.Geometry.Vector2Int>>
            {
                IsProperty = false,
                IsPublic = true,
                IsVirtual = false,
                DeclaringType = typeof(global::Edgar.Geometry.PolygonGrid2D),
                Converter = null,
                Getter = static obj => ((global::Edgar.Geometry.PolygonGrid2D)obj).points,
                Setter = static (obj, value) => ((global::Edgar.Geometry.PolygonGrid2D)obj).points = value!,
                IgnoreCondition = null,
                HasJsonInclude = false,
                IsExtensionData = false,
                NumberHandling = null,
                PropertyName = "points",
                JsonPropertyName = null
            };
            
            properties[1] = global::System.Text.Json.Serialization.Metadata.JsonMetadataServices.CreatePropertyInfo<global::System.Collections.Generic.List<global::Edgar.Geometry.Vector2Int>>(options, info1);

            return properties;
        }

        // Intentionally not a static method because we create a delegate to it. Invoking delegates to instance
        // methods is almost as fast as virtual calls. Static methods need to go through a shuffle thunk.
        private void PolygonGrid2DSerializeHandler(global::System.Text.Json.Utf8JsonWriter writer, global::Edgar.Geometry.PolygonGrid2D? value)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }
            
            writer.WriteStartObject();


            writer.WriteEndObject();
        }
    }
}
