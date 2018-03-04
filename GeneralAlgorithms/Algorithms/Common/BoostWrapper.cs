namespace GeneralAlgorithms.Algorithms.Common
{
	using System;
	using System.Runtime.InteropServices;

	public static class BoostWrapper
	{
		[DllImport("BoostWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool GetFaces(
			[MarshalAs(UnmanagedType.LPArray)] int[] edges,
			int edgesCount,
			int verticesCount,
			[Out] int[] faces,
			[Out] int[] facesBorders,
			out int facesCount
		);
	}
}