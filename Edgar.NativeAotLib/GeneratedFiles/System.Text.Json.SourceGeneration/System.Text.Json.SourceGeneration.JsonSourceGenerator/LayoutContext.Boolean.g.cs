﻿// <auto-generated/>

#nullable enable annotations
#nullable disable warnings

// Suppress warnings about [Obsolete] member usage in generated code.
#pragma warning disable CS0612, CS0618

namespace Edgar.NativeAotLib
{
    public partial class LayoutContext
    {
        private global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<bool>? _Boolean;
        
        /// <summary>
        /// Defines the source generated JSON serialization contract metadata for a given type.
        /// </summary>
        public global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<bool> Boolean
        {
            get => _Boolean ??= (global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<bool>)Options.GetTypeInfo(typeof(bool));
        }
        
        private global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<bool> Create_Boolean(global::System.Text.Json.JsonSerializerOptions options)
        {
            if (!TryGetTypeInfoForRuntimeCustomConverter<bool>(options, out global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<bool> jsonTypeInfo))
            {
                jsonTypeInfo = global::System.Text.Json.Serialization.Metadata.JsonMetadataServices.CreateValueInfo<bool>(options, global::System.Text.Json.Serialization.Metadata.JsonMetadataServices.BooleanConverter);
            }
        
            jsonTypeInfo.OriginatingResolver = this;
            return jsonTypeInfo;
        }
    }
}
