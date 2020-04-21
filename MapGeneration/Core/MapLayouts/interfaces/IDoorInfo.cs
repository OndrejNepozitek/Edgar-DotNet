namespace MapGeneration.Core.MapLayouts.Interfaces
{
	using GeneralAlgorithms.DataStructures.Common;

    /// <summary>
    /// Represents door information.
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
	public interface IDoorInfo<TNode>
	{
		TNode Node { get; }

		OrthogonalLine DoorLine { get; }
	}
}