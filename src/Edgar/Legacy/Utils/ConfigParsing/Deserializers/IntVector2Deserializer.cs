using System;
using System.Collections.Generic;
using Edgar.Geometry;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Edgar.Legacy.Utils.ConfigParsing.Deserializers
{
    /// <summary>
	/// A class to deserialize List of ints into IntVector2 or IntVector2?
	/// </summary>
	/// <exception cref="InvalidOperationException">Thrown where there are not exactly two elements in the list.</exception>
	public class IntVector2Deserializer : INodeDeserializer
	{
		public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
		{
			if (expectedType != typeof(Vector2Int) && expectedType != typeof(Vector2Int?))
			{
				value = null;
				return false;
			}

			var valueObject = nestedObjectDeserializer(reader, typeof(List<int>));

			if (valueObject == null)
				throw new ParsingException($"Given element could not be parsed into {nameof(List<int>)}");

			var intList = (List<int>) valueObject;

			if (intList.Count != 2)
				throw new ParsingException($"Given element could not be parsed into {nameof(Vector2Int)}. There must be exactly 2 elements of type {nameof(Int32)} in the array.");

			value = new Vector2Int(intList[0], intList[1]);
			return true;
		}
	}
}