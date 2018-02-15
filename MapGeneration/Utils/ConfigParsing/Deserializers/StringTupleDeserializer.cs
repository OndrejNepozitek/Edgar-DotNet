namespace MapGeneration.Utils.ConfigParsing.Deserializers
{
	using System;
	using System.Collections.Generic;
	using YamlDotNet.Core;
	using YamlDotNet.Serialization;

	public class StringTupleDeserializer : INodeDeserializer
	{
		public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
		{
			if (expectedType != typeof(Tuple<string, string>))
			{
				value = null;
				return false;
			}

			var valueObject = nestedObjectDeserializer(reader, typeof(List<string>));

			if (valueObject == null)
				throw new InvalidOperationException();

			var stringList = (List<string>)valueObject;

			if (stringList.Count != 2)
				throw new InvalidOperationException();

			value = new Tuple<string, string>(stringList[0], stringList[1]);
			return true;
		}
	}
}