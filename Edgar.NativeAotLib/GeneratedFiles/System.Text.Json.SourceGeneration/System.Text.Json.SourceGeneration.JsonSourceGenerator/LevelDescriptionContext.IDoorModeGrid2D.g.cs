﻿// <auto-generated/>

#nullable enable annotations
#nullable disable warnings

// Suppress warnings about [Obsolete] member usage in generated code.
#pragma warning disable CS0612, CS0618

namespace Edgar.NativeAotLib
{
    public partial class LevelDescriptionContext
    {
        private global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<global::Edgar.GraphBasedGenerator.Grid2D.IDoorModeGrid2D>? _IDoorModeGrid2D;
        
        /// <summary>
        /// Defines the source generated JSON serialization contract metadata for a given type.
        /// </summary>
        public global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<global::Edgar.GraphBasedGenerator.Grid2D.IDoorModeGrid2D> IDoorModeGrid2D
        {
            get => _IDoorModeGrid2D ??= (global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<global::Edgar.GraphBasedGenerator.Grid2D.IDoorModeGrid2D>)Options.GetTypeInfo(typeof(global::Edgar.GraphBasedGenerator.Grid2D.IDoorModeGrid2D));
        }
        
        private global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<global::Edgar.GraphBasedGenerator.Grid2D.IDoorModeGrid2D> Create_IDoorModeGrid2D(global::System.Text.Json.JsonSerializerOptions options)
        {
            if (!TryGetTypeInfoForRuntimeCustomConverter<global::Edgar.GraphBasedGenerator.Grid2D.IDoorModeGrid2D>(options, out global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<global::Edgar.GraphBasedGenerator.Grid2D.IDoorModeGrid2D> jsonTypeInfo))
            {
                var objectInfo = new global::System.Text.Json.Serialization.Metadata.JsonObjectInfoValues<global::Edgar.GraphBasedGenerator.Grid2D.IDoorModeGrid2D>
                {
                    ObjectCreator = null,
                    ObjectWithParameterizedConstructorCreator = null,
                    PropertyMetadataInitializer = _ => IDoorModeGrid2DPropInit(options),
                    ConstructorParameterMetadataInitializer = null,
                    SerializeHandler = null
                };
                
                jsonTypeInfo = global::System.Text.Json.Serialization.Metadata.JsonMetadataServices.CreateObjectInfo<global::Edgar.GraphBasedGenerator.Grid2D.IDoorModeGrid2D>(options, objectInfo);
                jsonTypeInfo.NumberHandling = null;
            }
        
            jsonTypeInfo.OriginatingResolver = this;
            return jsonTypeInfo;
        }

        private static global::System.Text.Json.Serialization.Metadata.JsonPropertyInfo[] IDoorModeGrid2DPropInit(global::System.Text.Json.JsonSerializerOptions options)
        {
            var properties = new global::System.Text.Json.Serialization.Metadata.JsonPropertyInfo[0];

            return properties;
        }
    }
}
