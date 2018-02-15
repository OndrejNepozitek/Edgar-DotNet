namespace MapGeneration.Utils.ConfigParsing.Deserializers
{
	using System;
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Common;
	using YamlDotNet.Core;
	using YamlDotNet.Serialization;

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
				throw new InvalidOperationException();

			var intVector2List = (List<IntVector2>) valueObject;

			if (intVector2List.Count != 2)
				throw new InvalidOperationException();

			value = new OrthogonalLine(intVector2List[0], intVector2List[1]);
			return true;
		}
	}
}