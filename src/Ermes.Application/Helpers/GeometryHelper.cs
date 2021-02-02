using Ermes.Dto.Spatial;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

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

    }
}
