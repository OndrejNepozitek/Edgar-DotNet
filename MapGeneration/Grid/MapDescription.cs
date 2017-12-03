namespace MapGeneration.Grid
{
	using System.Collections.Generic;

	public class MapDescription<TNode, TPolygon>
	{
		public void SetMinimumCommonLength(int length)
		{
			
		}

		public void AddRoomShapes(List<TPolygon> shapes, bool rotate = true)
		{
			
		}

		public void AddRoomShape(TPolygon shape, bool rotate = true, int probability = 1)
		{

		}

		public void AddRoom(TNode node, List<TPolygon> shapes = null, bool rotate = true)
		{
			
		}

		public void AddEdge(TNode from, TNode to, bool useCorridor = false)
		{
			
		}
	}
}