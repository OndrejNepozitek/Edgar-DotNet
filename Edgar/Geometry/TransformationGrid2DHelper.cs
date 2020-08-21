using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace Edgar.Geometry
{
    public static class TransformationGrid2DHelper
	{
		public static TransformationGrid2D[] GetAllTransformationsOld()
		{
			return (TransformationGrid2D[]) Enum.GetValues(typeof(TransformationGrid2D));
		}

        public static List<TransformationGrid2D> GetAll()
        {
            return ((TransformationGrid2D[]) Enum.GetValues(typeof(TransformationGrid2D))).ToList();
        }

        public static List<TransformationGrid2D> GetRotations(bool includeIdentity = true)
        {
            return new List<TransformationGrid2D>()
            {
                TransformationGrid2D.Identity,
                TransformationGrid2D.Rotate90,
                TransformationGrid2D.Rotate180,
                TransformationGrid2D.Rotate270
            };
        }
	}
}