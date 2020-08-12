using System;
using System.Collections.Generic;
using Edgar.Legacy.Core.Doors.Interfaces;
using Edgar.Legacy.Core.Doors.ManualMode;
using Edgar.Legacy.Core.Doors.SimpleMode;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;

namespace Edgar.Legacy.Core.Doors
{
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

		private static readonly object LockObject = new object();

		/// <summary>
		/// Instance of a default door handler. Currently supports OverlapMode and SpecificPositionsMode.
		/// </summary>
		public static DoorHandler DefaultHandler
		{
			get
			{
                lock (LockObject)
                {
                    if (_defaultHandler != null)
                        return _defaultHandler;

                    _defaultHandler = new DoorHandler();
                    _defaultHandler.RegisterHandler(typeof(SimpleDoorMode), new SimpleModeHandler());
                    _defaultHandler.RegisterHandler(typeof(ManualDoorMode), new ManualDoorModeHandler());

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
		public List<DoorLine> GetDoorPositions(PolygonGrid2D polygon, IDoorMode doorMode)
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