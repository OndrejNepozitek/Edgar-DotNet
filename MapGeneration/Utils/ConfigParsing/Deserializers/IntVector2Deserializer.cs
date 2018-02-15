namespace MapGeneration.Utils.ConfigParsing.Deserializers
{
	using System;
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Common;
	using YamlDotNet.Core;
	using YamlDotNet.Serialization;

	public class IntVector2Deserializer : INodeDeserializer
	{
		public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
		{
			if (expectedType != typeof(IntVector2))
			{
				value = null;
				return false;
			}

			var valueObject = nestedObjectDeserializer(reader, typeof(List<int>));

			if (valueObject == null)
				throw new InvalidOperationException();

			var intList = (List<int>) valueObject;

			if (intList.Count != 2)
				throw new InvalidOperationException();

			value = new IntVector2(intList[0], intList[1]);
			return true;
		}
	}
}