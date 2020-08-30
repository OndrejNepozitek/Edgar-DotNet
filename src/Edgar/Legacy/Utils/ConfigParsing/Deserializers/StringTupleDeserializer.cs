using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Edgar.Legacy.Utils.ConfigParsing.Deserializers
{
    /// <summary>
	/// A class to deserialize List of string into Tuple with two strings
	/// </summary>
	/// <exception cref="InvalidOperationException">Thrown where there are not exactly two elements in the list.</exception>
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
				throw new ParsingException($"Given element could not be parsed into {nameof(List<string>)}");

			var stringList = (List<string>)valueObject;

			if (stringList.Count != 2)
				throw new ParsingException($"Given element could not be parsed into {nameof(Tuple<string, string>)}. There must be exactly 2 elements of type {nameof(String)} in the array.");

			value = new Tuple<string, string>(stringList[0], stringList[1]);
			return true;
		}
	}
}