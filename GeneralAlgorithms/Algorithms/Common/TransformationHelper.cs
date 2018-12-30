namespace GeneralAlgorithms.Algorithms.Common
{
	using System;
	using DataStructures.Common;

	public static class TransformationHelper
	{
		public static Transformation[] GetAllTransforamtion()
		{
			return (Transformation[]) Enum.GetValues(typeof(Transformation));
		}
	}
}