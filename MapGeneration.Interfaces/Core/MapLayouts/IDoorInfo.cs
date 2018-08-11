namespace MapGeneration.Interfaces.Core.MapLayouts
{
	using GeneralAlgorithms.DataStructures.Common;

	/// <summary>
	/// Represents door information.
	/// </summary>
	/// <typeparam name="TNode"></typeparam>
	public interface IDoorInfo<out TNode>
	{
		TNode Node { get; }

		OrthogonalLine DoorLine { get; }
	}
}