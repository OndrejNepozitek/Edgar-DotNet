namespace GeneralAlgorithms.Algorithms.Common
{
	using System.Runtime.InteropServices;

	/// <summary>
	/// Wrapper of the Boost library that is written in C++.
	/// </summary>
	public static class BoostWrapper
	{
		/// <summary>
		/// Gets faces of a planar drawing of a given undirected graph with integer vertices.
		/// </summary>
		/// <param name="edges">Foreach i, i-th and (i+1)-th elements represent an edge of a given graph.</param>
		/// <param name="edgesCount">Total number of edges.</param>
		/// <param name="verticesCount">Total number of vertices.</param>
		/// <param name="faces">Faces of the graph. Must be allocated by the caller.</param>
		/// <param name="facesBorders">Where are the borders of returned faces in the faces array.</param>
		/// <param name="facesCount">How many faces were found.</param>
		/// <returns>Whether the call was successful</returns>
		[DllImport("BoostWrapper", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool GetFaces(
			[MarshalAs(UnmanagedType.LPArray)] int[] edges,
			int edgesCount,
			int verticesCount,
			[Out] int[] faces,
			[Out] int[] facesBorders,
			out int facesCount
		);

		/// <summary>
		/// Chekcs whether a given graph is planar.
		/// </summary>
		/// <param name="edges">Foreach i, i-th and (i+1)-th elements represent an edge of a given graph.</param>
		/// <param name="edgesCount">Total number of edges.</param>
		/// <param name="verticesCount">Total number of vertices.</param>
		/// <returns></returns>
		[DllImport("BoostWrapper", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool IsPlanar(
			[MarshalAs(UnmanagedType.LPArray)] int[] edges,
			int edgesCount,
			int verticesCount
		);
	}
}