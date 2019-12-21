namespace GeneralAlgorithms.Algorithms.Common
{
	using System;
	using DataStructures.Common;

	public static class TransformationHelper
	{
		public static Transformation[] GetAllTransformations()
		{
			return (Transformation[]) Enum.GetValues(typeof(Transformation));
		}
	}
}