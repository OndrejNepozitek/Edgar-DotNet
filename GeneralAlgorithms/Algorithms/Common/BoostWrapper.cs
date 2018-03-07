namespace GeneralAlgorithms.Algorithms.Common
{
	using System.Runtime.InteropServices;

	public static class BoostWrapper
	{
		[DllImport("BoostWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool GetFaces(
			[MarshalAs(UnmanagedType.LPArray)] int[] edges,
			int edgesCount,
			int verticesCount,
			[Out] int[] faces,
			[Out] int[] facesBorders,
			out int facesCount
		);

		[DllImport("BoostWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool IsPlanar(
			[MarshalAs(UnmanagedType.LPArray)] int[] edges,
			int edgesCount,
			int verticesCount
		);
	}
}