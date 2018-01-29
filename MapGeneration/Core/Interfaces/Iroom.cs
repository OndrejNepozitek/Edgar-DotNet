namespace MapGeneration.Core.Interfaces
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;

	public interface IRoom<out TNode>
	{
		TNode Node { get; }

		GridPolygon Shape { get;  }

		IntVector2 Position { get; }

		IList<OrthogonalLine> Doors { get; }
	}
}