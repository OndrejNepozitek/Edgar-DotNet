namespace MapGeneration.Interfaces.Utils
{
	using System.Collections.Generic;
	using System.IO;
	using Core;
	using Core.MapLayouts;

	public interface ISerializer<TNode>
	{
		void Serialize(IMapLayout<TNode> layout, StreamWriter writer);

		void Serialize(IList<IMapLayout<TNode>> layouts, StreamWriter writer);
	}
}