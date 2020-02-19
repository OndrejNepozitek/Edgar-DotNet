using System.Collections.Generic;
using System.IO;
using MapGeneration.Core.MapLayouts;

namespace MapGeneration.Utils.Interfaces
{
    /// <summary>
	/// Represents types that are able to serialize MapLayout.
	/// </summary>
	/// <typeparam name="TNode"></typeparam>
	public interface ISerializer<TNode>
	{
		void Serialize(MapLayout<TNode> layout, StreamWriter writer);

		void Serialize(IList<MapLayout<TNode>> layouts, StreamWriter writer);
	}
}