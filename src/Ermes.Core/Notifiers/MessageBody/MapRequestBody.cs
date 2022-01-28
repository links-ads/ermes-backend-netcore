using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Notifiers.MessageBody
{
    public class MapRequestBody
    {
        public string hazard { get; set; }
        public DateTime delineation_time_start { get; set; }
        public DateTime delineation_time_end { get; set; }
        public Geometry geometry { get; set; }
        public string request_code { get; set; }
    }
}
