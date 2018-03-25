namespace MapGeneration.Core.LayoutConverters.CorridorNodesCreators
{
	using System;
	using System.Collections.Generic;
	using Interfaces.Core.LayoutConverters;

	/// <summary>
	/// Factory of corridor nodes creators.
	/// </summary>
	public class CorridorNodesCreatorFactory
	{
		/// <summary>
		/// Default instance of the factory.
		/// </summary>
		public static CorridorNodesCreatorFactory Default { get; }

		private readonly Dictionary<Type, object> creators = new Dictionary<Type, object>();

		static CorridorNodesCreatorFactory()
		{
			Default = new CorridorNodesCreatorFactory();
			Default.AddCreator(new IntCorridorNodesCreator());
			Default.AddCreator(new StringCorridorNodesCreator());
		}

		/// <summary>
		/// Registers a creator for a given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="creator"></param>
		public void AddCreator<T>(ICorridorNodesCreator<T> creator)
		{
			creators.Add(typeof(T), creator);
		}

		/// <summary>
		/// Gets a creator for a given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public ICorridorNodesCreator<T> GetCreator<T>()
		{
			if (creators.TryGetValue(typeof(T), out var creator))
			{
				return (ICorridorNodesCreator<T>) creator;
			}

			throw new ArgumentException("Creator for a given type was not found.");
		}
	}
}