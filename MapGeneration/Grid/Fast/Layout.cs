namespace MapGeneration.Grid.Fast
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces;

	// TODO: should it be struct or not?
	public class Layout : ILayout<int, GridPolygon, IntVector2>
	{
		private readonly Configuration?[] vertices;

		public Layout(int verticesCount)
		{
			vertices = new Configuration?[verticesCount];
		}

		private Layout(Configuration?[] vertices)
		{
			this.vertices = (Configuration?[]) vertices.Clone();
		}

		public bool GetConfiguration(int vertex, out Configuration configuration)
		{
			if (vertices[vertex].HasValue)
			{
				configuration = vertices[vertex].Value;
				return true;
			}

			configuration = default(Configuration);
			return false;
		}

		// TODO: unsafe
		public IConfiguration<GridPolygon, IntVector2> GetConfiguration(int node)
		{
			throw new NotImplementedException();
		}

		IEnumerable<IConfiguration<GridPolygon, IntVector2>> ILayout<int, GridPolygon, IntVector2>.GetConfigurations()
		{
			return vertices.Where(x => x.HasValue).Select(x => (IConfiguration<GridPolygon, IntVector2>) x.Value);
		}

		public IEnumerable<IRoom<int, GridPolygon, IntVector2>> GetRooms()
		{
			return Enumerable.Range(0, vertices.Length).Where(x => vertices[x].HasValue)
				.Select(x => new GridRoom<int>(x, vertices[x].Value));
		}

		public Configuration?[] GetConfigurations()
		{
			return vertices;
		}

		public void SetConfiguration(int vertex, Configuration configuration)
		{
			vertices[vertex] = configuration;
		}

		public bool AreConfigurationsValid()
		{
			return vertices.Where(x => x.HasValue).All(x => x.Value.IsValid());
		}

		public float GetEnergy()
		{
			return vertices.Where(x => x.HasValue).Sum(x => x.Value.Energy);
		}

		// TODO: should it be here?
		public float GetDifference(Layout other)
		{
			var diff = 0f;

			for (var i = 0; i < vertices.Length; i++)
			{
				if (GetConfiguration(i, out var c1) && other.GetConfiguration(i, out var c2))
				{
					diff += (float)Math.Pow(
						IntVector2.ManhattanDistance(c1.Polygon.BoundingRectangle.Center + c1.Position,
							c2.Polygon.BoundingRectangle.Center + c2.Position), 2);
				}
			}

			return diff;
		}

		public Layout Clone()
		{
			return new Layout(vertices);
		}
	}
}