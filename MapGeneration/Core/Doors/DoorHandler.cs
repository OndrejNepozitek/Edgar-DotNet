namespace MapGeneration.Core.Doors
{
	using System;
	using System.Collections.Generic;
	using DoorHandlers;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces;

	public class DoorHandler : IDoorHandler
	{
		private readonly Dictionary<Type, IDoorHandler> handlers = new Dictionary<Type, IDoorHandler>();

		private static DoorHandler _defaultHandler;

		public static DoorHandler DefaultHandler
		{
			get
			{
				if (_defaultHandler != null)
					return _defaultHandler;

				_defaultHandler = new DoorHandler();
				_defaultHandler.RegisterHandler(typeof(OverlapModeHandler), new OverlapModeHandler());

				return _defaultHandler;
			}
		}

		public List<OrthogonalLine> GetDoorPositions(GridPolygon polygon, IDoorMode doorMode)
		{
			if (handlers.TryGetValue(doorMode.GetType(), out var handler))
			{
				return handler.GetDoorPositions(polygon, doorMode);
			}

			throw new InvalidOperationException("Handler not found");
		}

		public void RegisterHandler(Type type, IDoorHandler handler)
		{
			handlers.Add(type, handler);
		}
	}
}