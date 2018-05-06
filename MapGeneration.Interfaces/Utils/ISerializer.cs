namespace MapGeneration.Interfaces.Utils
{
	using System.Collections.Generic;
	using System.IO;
	using Core.MapLayouts;

	/// <summary>
	/// Represents types that are able to serialize IMapLayout.
	/// </summary>
	/// <typeparam name="TNode"></typeparam>
	public interface ISerializer<TNode>
	{
		void Serialize(IMapLayout<TNode> layout, StreamWriter writer);

		void Serialize(IList<IMapLayout<TNode>> layouts, StreamWriter writer);
	}
}