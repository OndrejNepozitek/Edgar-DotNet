namespace MapGeneration.Grid
{
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces;
	public class GridRoom<TNode> : IRoom<TNode, GridPolygon, IntVector2>
	{
		public TNode Node { get; }
		public IConfiguration<GridPolygon, IntVector2> Configuration { get; }

		public GridRoom(TNode node, IConfiguration<GridPolygon, IntVector2> configuration)
		{
			Node = node;
			Configuration = configuration;
		}
	}
}