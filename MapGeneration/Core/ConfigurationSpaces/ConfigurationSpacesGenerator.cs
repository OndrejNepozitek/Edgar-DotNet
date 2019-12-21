using System;
using System.Collections.Generic;
using System.Linq;
using GeneralAlgorithms.Algorithms.Common;
using GeneralAlgorithms.Algorithms.Polygons;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.Doors;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Interfaces.Core.Configuration;
using MapGeneration.Interfaces.Core.ConfigurationSpaces;
using MapGeneration.Interfaces.Core.Doors;

namespace MapGeneration.Core.ConfigurationSpaces
{
    public class ConfigurationSpacesGenerator
    {
        private readonly IPolygonOverlap<GridPolygon> polygonOverlap;
        private readonly IDoorHandler doorHandler;
        private readonly ILineIntersection<OrthogonalLine> lineIntersection;
        private readonly IPolygonUtils<GridPolygon> polygonUtils;

        public ConfigurationSpacesGenerator(IPolygonOverlap<GridPolygon> polygonOverlap, IDoorHandler doorHandler, ILineIntersection<OrthogonalLine> lineIntersection, IPolygonUtils<GridPolygon> polygonUtils)
        {
            this.polygonOverlap = polygonOverlap;
            this.doorHandler = doorHandler;
            this.lineIntersection = lineIntersection;
            this.polygonUtils = polygonUtils;
        }

        public ConfigurationSpaces<TConfiguration> Generate<TNode, TConfiguration>(MapDescription<TNode> mapDescription, List<int> offsets = null)
            where TConfiguration : IConfiguration<IntAlias<GridPolygon>>
        {
            throw new NotImplementedException();
        }
        
		/// <summary>
		/// Computes configuration space of given two polygons.
		/// </summary>
		/// <param name="polygon"></param>
		/// <param name="doorsMode"></param>
		/// <param name="fixedCenter"></param>
		/// <param name="fixedDoorsMode"></param>
		/// <param name="offsets"></param>
		/// <returns></returns>
		public ConfigurationSpace GetConfigurationSpace(GridPolygon polygon, IDoorMode doorsMode, GridPolygon fixedCenter,
			IDoorMode fixedDoorsMode, List<int> offsets = null)
		{
            throw new NotImplementedException();
        }

		public List<RoomTemplateInstance> GetRoomTemplateInstances(RoomTemplate roomTemplate)
        {
            var result = new List<RoomTemplateInstance>();
            var doorLines = doorHandler.GetDoorPositions(roomTemplate.Shape, roomTemplate.DoorsMode);
            var shape = roomTemplate.Shape;

            foreach (var transformation in roomTemplate.AllowedTransformations)
            {
                var transformedShape = shape.Transform(transformation);
                var smallestPoint = transformedShape.BoundingRectangle.A;

                // Both the shape and doors are moved so the polygon is in the first quadrant and touches axes
                transformedShape = transformedShape + (-1 * smallestPoint);
                transformedShape = polygonUtils.NormalizePolygon(transformedShape);
                var transformedDoorLines = doorLines
                    .Select(x => DoorUtils.TransformDoorLine(x, transformation))
                    .Select(x => new DoorLine(x.Line + (-1 * smallestPoint), x.Length))
                    .Cast<IDoorLine>()
                    .ToList();

                // Check if we already have the same room shape (together with door lines)
                var sameRoomShapeFound = false;
                foreach (var roomInfo in result)
                {
                    if (roomInfo.RoomShape.Equals(transformedShape) &&
                        roomInfo.DoorLines.SequenceEqualWithoutOrder(transformedDoorLines))
                    {
                        roomInfo.Transformations.Add(transformation);

                        sameRoomShapeFound = true;
                        break;
                    }
                }

                if (sameRoomShapeFound)
                    continue;

                result.Add(new RoomTemplateInstance(roomTemplate, transformedShape, transformation, transformedDoorLines));
            }

            return result;
		}
    }
}