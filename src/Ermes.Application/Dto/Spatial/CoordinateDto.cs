using Abp.AutoMapper;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Dto.Spatial
{
    [JsonArray]
    [AutoMap(typeof(Coordinate))]
    public class CoordinateDto
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
}
