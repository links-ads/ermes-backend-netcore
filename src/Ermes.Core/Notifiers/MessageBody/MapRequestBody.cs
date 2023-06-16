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
        public int datatype_id { get; set; }
        public string title { get; set; }
    }

    public class MapRequestPostEventMonitoringBody : MapRequestBody
    {
    }

    public class MapRequestFireAndBurnedAreaBody : MapRequestBody
    {
        public int resolution { get; set; }
        public int frequency { get; set; }
    }

    public class MapRequestWildFireSimulationBody : MapRequestBody
    {
        public string description { get; set; }
        public bool do_spotting { get; set; }
        public decimal probabilityRange { get; set; }
        public int time_limit { get; set; }
        public List<BoundaryConditionBody> boundary_conditions { get; set; }
    }

    public class BoundaryConditionBody {
        public int time { get; set; }
        public int w_dir { get; set; }
        public int w_speed { get; set; }
        public int moisture { get; set; }
        public Dictionary<string, string> fireBreak { get; set; }
    }

}
