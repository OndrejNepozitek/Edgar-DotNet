namespace MapGeneration.Core.ConfigurationSpaces
{
	using System;
	using System.Collections.Generic;
	using Doors;
	using GeneralAlgorithms.DataStructures.Common;

	/// <summary>
	/// Configuration space of a pair of polygons.
	/// </summary>
	public class ConfigurationSpace
	{
		public List<OrthogonalLine> Lines;

		public List<Tuple<OrthogonalLine, DoorLine>> ReverseDoors;
	}
}