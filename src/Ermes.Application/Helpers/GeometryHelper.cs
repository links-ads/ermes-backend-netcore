using Ermes.Dto.Spatial;
using NetTopologySuite.Geometries;
using System;

namespace Ermes.Helpers
{
    public static class GeometryHelper
    {
        public static Polygon GetPolygonFromBoundaries(PointPosition southWestBoundary, PointPosition northEastBoundary)
        {
            Coordinate ne = new Coordinate(northEastBoundary.Longitude, northEastBoundary.Latitude);
            Coordinate se = new Coordinate(northEastBoundary.Longitude, southWestBoundary.Latitude);
            Coordinate sw = new Coordinate(southWestBoundary.Longitude, southWestBoundary.Latitude);
            Coordinate nw = new Coordinate(southWestBoundary.Longitude, northEastBoundary.Latitude);
            return new Polygon(new LinearRing(new Coordinate[] { nw, ne, se, sw, nw }));
        }

        public static Point GetPointFromCoordinates(decimal[] coordinates)
        {
            double[] doubleArray = Array.ConvertAll(coordinates, x => (double)x);
            return new Point(new Coordinate() { X = doubleArray[1], Y = doubleArray[0] });
        }
    }
}
