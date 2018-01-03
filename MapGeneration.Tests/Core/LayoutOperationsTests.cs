namespace MapGeneration.Tests.Core
{
	using System.Linq;
	using GeneralAlgorithms.DataStructures.Graphs;
	using GeneralAlgorithms.DataStructures.Polygons;
	using MapGeneration.Core;
	using MapGeneration.Core.ConfigurationSpaces;
	using MapGeneration.Core.Interfaces;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class LayoutOperationsTests
	{
		private LayoutOperations<int, Layout, Configuration, IntAlias<GridPolygon>> layoutOperations;
		private Mock<IConfigurationSpaces<int, IntAlias<GridPolygon>, Configuration>> configurationSpaces;

		[SetUp]
		public void SetUp()
		{
			configurationSpaces = new Mock<IConfigurationSpaces<int, IntAlias<GridPolygon>, Configuration>>();
			layoutOperations = new LayoutOperations<int, Layout, Configuration, IntAlias<GridPolygon>>(configurationSpaces.Object);
		}

		[Test]
		public void RecomputeValidity_AllValid()
		{
			// Make all configuration pairs valid
			configurationSpaces
				.Setup(x => x.HaveValidPosition(It.IsAny<Configuration>(), It.IsAny<Configuration>()))
				.Returns(true);

			// Make graph with 4 vertices
			var graph = new FastGraph<int>(4);
			Enumerable.Range(0, 4).ToList().ForEach(x => graph.AddVertex(x));

			graph.AddEdge(0, 2);
			graph.AddEdge(0, 3);
			graph.AddEdge(1, 2);
			graph.AddEdge(2, 3);

			// Create a layout with a default configuration for every vertex
			var layout = new Layout(graph);
			graph.Vertices.ToList().ForEach(x => layout.SetConfiguration(x, new Configuration()));
			layoutOperations.RecomputeValidityVectors(layout);

			foreach (var vertex in graph.Vertices)
			{
				layout.GetConfiguration(vertex, out var configuration);
				var validityVector = configuration.ValidityVector;

				for (var i = 0; i < validityVector.Length; i++)
				{
					Assert.IsFalse(validityVector[i]);
				}
			}
		}

		[Test]
		public void RecomputeValidity_AllInvalid()
		{
			// Make all configuration pairs invalid
			configurationSpaces
				.Setup(x => x.HaveValidPosition(It.IsAny<Configuration>(), It.IsAny<Configuration>()))
				.Returns(false);

			// Make graph with 4 vertices
			var graph = new FastGraph<int>(4);
			Enumerable.Range(0, 4).ToList().ForEach(x => graph.AddVertex(x));

			graph.AddEdge(0, 2);
			graph.AddEdge(0, 3);
			graph.AddEdge(1, 2);
			graph.AddEdge(2, 3);

			// Create a layout with a default configuration for every vertex
			var layout = new Layout(graph);
			graph.Vertices.ToList().ForEach(x => layout.SetConfiguration(x, new Configuration()));
			layoutOperations.RecomputeValidityVectors(layout);

			foreach (var vertex in graph.Vertices)
			{
				layout.GetConfiguration(vertex, out var configuration);
				var validityVector = configuration.ValidityVector;
				var neighboursCount = graph.GetNeighbours(vertex).Count();

				for (var i = 0; i < neighboursCount; i++)
				{
					Assert.IsTrue(validityVector[i]);
				}

				for (var i = neighboursCount; i < validityVector.Length; i++)
				{
					Assert.IsFalse(validityVector[i]);
				}
			}
		}
	}
}