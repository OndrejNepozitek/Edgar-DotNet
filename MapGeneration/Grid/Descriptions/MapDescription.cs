namespace MapGeneration.Grid.Descriptions
{
	using System;
	using System.Collections.Generic;

	public class MapDescription<TNode>
	{
		public void SetMinimumCommonLength(int length)
		{
			
		}

		public void AddRoomShapes(List<RoomDescription> shapes, bool rotate = true)
		{
			
		}

		public void AddRoomShape(RoomDescription shape, bool rotate = true, int probability = 1)
		{

		}

		public void AddRoom(TNode node, List<RoomDescription> shapes = null, bool rotate = true)
		{
			
		}

		public IReadOnlyCollection<RoomDescription> GetRooms()
		{
			throw new NotImplementedException();
		}

		public void AddEdge(TNode from, TNode to, bool useCorridor = false)
		{
			if (useCorridor)
			{
				throw new NotImplementedException();
			}
		}
	}
}