using System;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace Edgar.Legacy.GeneralAlgorithms.Algorithms.Common
{
    public static class TransformationHelper
	{
		public static Transformation[] GetAllTransformations()
		{
			return (Transformation[]) Enum.GetValues(typeof(Transformation));
		}
	}
}