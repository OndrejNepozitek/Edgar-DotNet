namespace MapGeneration.Interfaces.Core.LayoutConverters
{
	using GeneralAlgorithms.DataStructures.Common;
	using MapDescriptions;

	/// <summary>
	/// Represents a type that can create nodes of a given generic type.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface ICorridorNodesCreator<T>
	{
		/// <summary>
		/// Adds corridor rooms to a given mapping.
		/// </summary>
		/// <param name="mapDescription"></param>
		/// <param name="mapping"></param>
		void AddCorridorsToMapping(ICorridorMapDescription<int> mapDescription, TwoWayDictionary<T, int> mapping);
	}
}