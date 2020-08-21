using System;
using System.Collections.Generic;
using Edgar.Geometry;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Edgar.Legacy.Utils.ConfigParsing.Deserializers
{
    /// <summary>
	/// A class to deserialize List of IntVector2 into an OrthogonalLine.
	/// </summary>
	/// <exception cref="InvalidOperationException">Thrown where there are not exactly two elements in the list or when the underlying element could not be parsed into a List of IntVector2.</exception>
	public class OrthogonalLineDeserializer : INodeDeserializer
	{
		public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
		{
			if (expectedType != typeof(OrthogonalLineGrid2D))
			{
				value = null;
				return false;
			}

			var valueObject = nestedObjectDeserializer(reader, typeof(List<Vector2Int>));

			if (valueObject == null)
				throw new ParsingException($"Given element could not be parsed into {nameof(OrthogonalLineGrid2D)}.");

			var intVector2List = (List<Vector2Int>) valueObject;

			if (intVector2List.Count != 2)
				throw new ParsingException($"Given element could not be parsed into {nameof(OrthogonalLineGrid2D)}. There must be exactly 2 elements of type {nameof(Vector2Int)} in the array.");

			value = new OrthogonalLineGrid2D(intVector2List[0], intVector2List[1]);
			return true;
		}
	}
}