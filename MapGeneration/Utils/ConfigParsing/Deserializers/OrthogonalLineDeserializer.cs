namespace MapGeneration.Utils.ConfigParsing.Deserializers
{
	using System;
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Common;
	using YamlDotNet.Core;
	using YamlDotNet.Serialization;

	/// <summary>
	/// A class to deserialize List of IntVector2 into an OrthogonalLine.
	/// </summary>
	/// <exception cref="InvalidOperationException">Thrown where there are not exactly two elements in the list or when the underlying element could not be parsed into a List of IntVector2.</exception>
	public class OrthogonalLineDeserializer : INodeDeserializer
	{
		public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
		{
			if (expectedType != typeof(OrthogonalLine))
			{
				value = null;
				return false;
			}

			var valueObject = nestedObjectDeserializer(reader, typeof(List<IntVector2>));

			if (valueObject == null)
				throw new ParsingException($"Given element could not be parsed into {nameof(OrthogonalLine)}.");

			var intVector2List = (List<IntVector2>) valueObject;

			if (intVector2List.Count != 2)
				throw new ParsingException($"Given element could not be parsed into {nameof(OrthogonalLine)}. There must be exactly 2 elements of type {nameof(IntVector2)} in the array.");

			value = new OrthogonalLine(intVector2List[0], intVector2List[1]);
			return true;
		}
	}
}