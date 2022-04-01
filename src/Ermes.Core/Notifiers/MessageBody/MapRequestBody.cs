using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Notifiers.MessageBody
{
    public class MapRequestBody
    {
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public Geometry geometry { get; set; }
        public string request_code { get; set; }
        public int frequency { get; set; }
        public int datatype_id { get; set; }
    }
}
