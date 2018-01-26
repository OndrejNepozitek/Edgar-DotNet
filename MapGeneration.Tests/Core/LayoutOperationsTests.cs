namespace MapGeneration.Tests.Core
{
	using System.Linq;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Graphs;
	using GeneralAlgorithms.DataStructures.Polygons;
	using MapGeneration.Core;
	using MapGeneration.Core.Interfaces;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class LayoutOperationsTests
	{
		private LayoutOperations<int, Layout, Configuration, IntAlias<GridPolygon>> layoutOperations;
		private Mock<IConfigurationSpaces<int, IntAlias<GridPolygon>, Configuration>> configurationSpaces;
		private IPolygonOverlap polygonOverlap;

		[SetUp]
		public void SetUp()
		{
			configurationSpaces = new Mock<IConfigurationSpaces<int, IntAlias<GridPolygon>, Configuration>>();
			polygonOverlap = new PolygonOverlap();
			layoutOperations = new LayoutOperations<int, Layout, Configuration, IntAlias<GridPolygon>>(configurationSpaces.Object, polygonOverlap);
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

		[Test]
		public void UpdateLayout_AllInvalid()
		{
			// TODO: Both RecomputeValidityVectors and RecomputeEnergy must work for this test to make any sense

			// Make all configuration pairs valid
			configurationSpaces
				.Setup(x => x.HaveValidPosition(It.IsAny<Configuration>(), It.IsAny<Configuration>()))
				.Returns(false);

			// Make graph with 3 vertices
			var graph = new FastGraph<int>(3);
			Enumerable.Range(0, 3).ToList().ForEach(x => graph.AddVertex(x));

			graph.AddEdge(0, 1);
			graph.AddEdge(1, 2);
			graph.AddEdge(0, 2);

			// Create a layout and recompute validity vectors and energies
			var layout = new Layout(graph);
			layout.SetConfiguration(0, new Configuration(new IntAlias<GridPolygon>(0, GridPolygon.GetSquare(3)), new IntVector2(0, 0), new SimpleBitVector32(), new EnergyData()));
			layout.SetConfiguration(1, new Configuration(new IntAlias<GridPolygon>(1, GridPolygon.GetSquare(3)), new IntVector2(2, -1), new SimpleBitVector32(), new EnergyData()));
			layout.SetConfiguration(2, new Configuration(new IntAlias<GridPolygon>(2, GridPolygon.GetRectangle(6, 3)), new IntVector2(2, 1), new SimpleBitVector32(), new EnergyData()));
			layoutOperations.RecomputeValidityVectors(layout);
			layoutOperations.RecomputeEnergy(layout);

			// Make a new configuration for vertex 2
			var newConfiguration = new Configuration(new IntAlias<GridPolygon>(3, GridPolygon.GetRectangle(6, 3)),
				new IntVector2(4, 1), new SimpleBitVector32(), new EnergyData());

			var changedLayout = layoutOperations.UpdateLayout(layout.Clone(), 2, newConfiguration);

			// Create the expected output
			var expectedLayout = layout.Clone();
			expectedLayout.SetConfiguration(2, newConfiguration);
			layoutOperations.RecomputeValidityVectors(expectedLayout);
			layoutOperations.RecomputeEnergy(expectedLayout);

			foreach (var vertex in graph.Vertices)
			{
				changedLayout.GetConfiguration(vertex, out var actualConfiguration);
				expectedLayout.GetConfiguration(vertex, out var expectedConfiguration);

				Assert.AreEqual(expectedConfiguration, actualConfiguration);
			}
		}
	}
}