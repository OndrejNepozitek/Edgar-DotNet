namespace MapGeneration.Core.Doors
{
	using System;
	using System.Collections.Generic;
	using DoorHandlers;
	using DoorModes;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces.Core.Doors;

	/// <inheritdoc />
	/// <summary>
	/// Door handler to be used in a context where multiple door modes are used.
	/// </summary>
	/// <remarks>
	/// This door handler does not have any logic itself - it just calls registered
	/// door handlers based on given door modes.
	/// </remarks>
	public class DoorHandler : IDoorHandler
	{
		private readonly Dictionary<Type, IDoorHandler> handlers = new Dictionary<Type, IDoorHandler>();

		private static DoorHandler _defaultHandler;

		private static object lockObject = new object();

		/// <summary>
		/// Instance of a default door handler. Currently supports OverlapMode and SpecificPositionsMode.
		/// </summary>
		public static DoorHandler DefaultHandler
		{
			get
			{
                lock (lockObject)
                {
                    if (_defaultHandler != null)
                        return _defaultHandler;

                    _defaultHandler = new DoorHandler();
                    _defaultHandler.RegisterHandler(typeof(OverlapMode), new OverlapModeHandler());
                    _defaultHandler.RegisterHandler(typeof(SpecificPositionsMode), new SpecificPositionsModeHandler());

                    return _defaultHandler;
                }
            }
		}

		/// <inheritdoc />
		/// <remarks>
		/// Gets door positions by by returning an output of a registered door handler.
		/// </remarks>
		/// <param name="polygon"></param>
		/// <param name="doorMode"></param>
		/// <returns></returns>
		public List<IDoorLine> GetDoorPositions(GridPolygon polygon, IDoorMode doorMode)
		{
			if (handlers.TryGetValue(doorMode.GetType(), out var handler))
			{
				return handler.GetDoorPositions(polygon, doorMode);
			}

			throw new InvalidOperationException("Handler not found");
		}

		/// <summary>
		/// Register a door handler for a given type of door mode.
		/// </summary>
		/// <param name="doorModeType"></param>
		/// <param name="handler"></param>
		public void RegisterHandler(Type doorModeType, IDoorHandler handler)
		{
			handlers.Add(doorModeType, handler);
		}
	}
}