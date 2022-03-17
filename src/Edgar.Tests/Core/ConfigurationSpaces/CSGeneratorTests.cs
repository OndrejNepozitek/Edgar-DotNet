﻿namespace MapGeneration.Tests.Core.ConfigurationSpaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class CSGeneratorTests
    {
        //private ConfigurationSpacesGeneratorOld generatorOld;

        //[SetUp]
        //public void SetUp()
        //{
        //	generatorOld = new ConfigurationSpacesGeneratorOld(new PolygonOverlap(), DoorHandler.DefaultHandler,
        //		new OrthogonalLineIntersection(), new GridPolygonUtils());
        //}

        //[Test]
        //public void GetConfigurationSpace_Squares()
        //{
        //	var p1 = GridPolygon.GetSquare(3);
        //	var p2 = GridPolygon.GetSquare(5);

        //	var configurationSpace = generatorOld.GetConfigurationSpace(p1, new OverlapMode(1, 0), p2, new OverlapMode(1, 0));
        //	var expectedPoints = new List<IntVector2>();
        //	var actualPoints = configurationSpace.Lines.Select(x => x.GetPoints()).SelectMany(x => x).ToList();

        //	{
        //		// Top side of fixed
        //		var points = new OrthogonalLine(new IntVector2(-2, 5), new IntVector2(4, 5)).GetPoints();
        //		expectedPoints.AddRange(points);
        //		Assert.AreEqual(points.Count, actualPoints.Intersect(points).Count());
        //	}

        //	{
        //		// Bottom side of fixed
        //		var points = new OrthogonalLine(new IntVector2(-2, -3), new IntVector2(4, -3)).GetPoints();
        //		expectedPoints.AddRange(points);
        //		Assert.AreEqual(points.Count, actualPoints.Intersect(points).Count());
        //	}

        //	{
        //		// Right side of fixed
        //		var points = new OrthogonalLine(new IntVector2(5, 4), new IntVector2(5, -2)).GetPoints();
        //		expectedPoints.AddRange(points);
        //		Assert.AreEqual(points.Count, actualPoints.Intersect(points).Count());
        //	}

        //	{
        //		// Left side of fixed
        //		var points = new OrthogonalLine(new IntVector2(-3, -2), new IntVector2(-3, 4)).GetPoints();
        //		expectedPoints.AddRange(points);
        //		Assert.AreEqual(points.Count, actualPoints.Intersect(points).Count());
        //	}

        //	Assert.AreEqual(expectedPoints.Distinct().Count(), actualPoints.Count);
        //}

        //[Test]
        //      [Ignore("Fix later")]
        //      public void GetConfigurationSpace_MovedSquares()
        //{
        //	// Check if the generator works even when the polygons are not normalized
        //	Assert.IsTrue(false);
        //}

        //[Test]
        //public void GetConfigurationSpace_OverlapOne()
        //{
        //	var p1 = GridPolygon.GetSquare(3);
        //	var p2 = GridPolygon.GetSquare(5);

        //	var configurationSpace = generatorOld.GetConfigurationSpace(p1, new OverlapMode(1, 1), p2, new OverlapMode(1, 1));
        //	var expectedPoints = new List<IntVector2>();
        //	var actualPoints = configurationSpace.Lines.Select(x => x.GetPoints()).SelectMany(x => x).ToList();

        //	{
        //		// Top side of fixed
        //		var points = new OrthogonalLine(new IntVector2(0, 5), new IntVector2(2, 5)).GetPoints();
        //		expectedPoints.AddRange(points);
        //		Assert.AreEqual(points.Count, actualPoints.Intersect(points).Count());
        //	}

        //	{
        //		// Bottom side of fixed
        //		var points = new OrthogonalLine(new IntVector2(0, -3), new IntVector2(2, -3)).GetPoints();
        //		expectedPoints.AddRange(points);
        //		Assert.AreEqual(points.Count, actualPoints.Intersect(points).Count());
        //	}

        //	{
        //		// Right side of fixed
        //		var points = new OrthogonalLine(new IntVector2(5, 2), new IntVector2(5, 0)).GetPoints();
        //		expectedPoints.AddRange(points);
        //		Assert.AreEqual(points.Count, actualPoints.Intersect(points).Count());
        //	}

        //	{
        //		// Left side of fixed
        //		var points = new OrthogonalLine(new IntVector2(-3, 0), new IntVector2(-3, 2)).GetPoints();
        //		expectedPoints.AddRange(points);
        //		Assert.AreEqual(points.Count, actualPoints.Intersect(points).Count());
        //	}

        //	Assert.AreEqual(expectedPoints.Distinct().Count(), actualPoints.Count);
        //}

        //[Test]
        //public void GetConfigurationSpace_DifferentOverlaps()
        //{
        //	var p1 = GridPolygon.GetSquare(3);
        //	var p2 = GridPolygon.GetSquare(5);

        //	var configurationSpace = generatorOld.GetConfigurationSpace(p1, new OverlapMode(1, 0), p2, new OverlapMode(1, 1));
        //	var expectedPoints = new List<IntVector2>();
        //	var actualPoints = configurationSpace.Lines.Select(x => x.GetPoints()).SelectMany(x => x).ToList();

        //	{
        //		// Top side of fixed
        //		var points = new OrthogonalLine(new IntVector2(-1, 5), new IntVector2(3, 5)).GetPoints();
        //		expectedPoints.AddRange(points);
        //		Assert.AreEqual(points.Count, actualPoints.Intersect(points).Count());
        //	}

        //	{
        //		// Bottom side of fixed
        //		var points = new OrthogonalLine(new IntVector2(-1, -3), new IntVector2(3, -3)).GetPoints();
        //		expectedPoints.AddRange(points);
        //		Assert.AreEqual(points.Count, actualPoints.Intersect(points).Count());
        //	}

        //	{
        //		// Right side of fixed
        //		var points = new OrthogonalLine(new IntVector2(5, 3), new IntVector2(5, -1)).GetPoints();
        //		expectedPoints.AddRange(points);
        //		Assert.AreEqual(points.Count, actualPoints.Intersect(points).Count());
        //	}

        //	{
        //		// Left side of fixed
        //		var points = new OrthogonalLine(new IntVector2(-3, -1), new IntVector2(-3, 3)).GetPoints();
        //		expectedPoints.AddRange(points);
        //		Assert.AreEqual(points.Count, actualPoints.Intersect(points).Count());
        //	}

        //	Assert.AreEqual(expectedPoints.Distinct().Count(), actualPoints.Count);
        //}

        //[Test]
        //      [Ignore("Fix later")]
        //public void Generate_BasicTest()
        //{
        //	// This test cannot check if the generated configuration spaces are valid
        //	var mapDescription = new MapDescriptionOld<int>();
        //	var squareRoom = new RoomTemplate(GridPolygon.GetSquare(3), new OverlapMode(1, 0));
        //	var rectangleRoom = new RoomTemplate(GridPolygon.GetRectangle(4, 5), new OverlapMode(1, 1));

        //	mapDescription.AddRoomShapes(squareRoom);
        //	mapDescription.AddRoomShapes(rectangleRoom, probability: 0.5d);

        //	mapDescription.AddRoom(0);
        //	mapDescription.AddRoom(1);
        //	mapDescription.AddPassage(0, 1);

        //	mapDescription.AddRoomShapes(1, rectangleRoom, new List<Transformation>() {Transformation.Identity});

        //	// var configurationSpaces = generator.Generate(mapDescription);
        //	Assert.IsTrue(false); // TODO: repair

        //	//Assert.AreEqual(3, configurationSpaces.GetShapesForNode(0).Count);
        //	//Assert.AreEqual(1, configurationSpaces.GetShapesForNode(1).Count);
        //}

        //[Test]
        //public void Generate_NodeWithoutShape()
        //{
        //	var mapDescription = new MapDescriptionOld<int>();
        //	mapDescription.AddRoom(0);

        //	Assert.Throws<ArgumentException>(
        //		() => generatorOld.Generate<int, IConfiguration<IntAlias<GridPolygon>, int>>(mapDescription));
        //}
    }
}