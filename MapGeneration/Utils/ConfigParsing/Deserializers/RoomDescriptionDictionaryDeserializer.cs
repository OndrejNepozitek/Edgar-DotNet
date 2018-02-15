namespace MapGeneration.Utils.ConfigParsing.Deserializers
{
	using System;
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Common;
	using YamlDotNet.Core;
	using YamlDotNet.Serialization;

	public class RoomDescriptionDictionaryDeserializer : INodeDeserializer
	{
		public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
		{
			throw new NotImplementedException();
		}
	}
}