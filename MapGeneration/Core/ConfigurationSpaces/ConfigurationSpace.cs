namespace MapGeneration.Core.ConfigurationSpaces
{
	using System;
	using System.Collections.Generic;
	using Doors;
	using GeneralAlgorithms.DataStructures.Common;

	public class ConfigurationSpace
	{
		public List<OrthogonalLine> Lines;

		public List<Tuple<OrthogonalLine, DoorLine>> ReverseDoors;
	}
}