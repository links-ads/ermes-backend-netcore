using Ermes.Reports.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Stations.Dto
{
    public class MeasureDto
    {
        public string Id { get; set; }
        public string SensorId { get; set; }
        public string StationId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Measure { get; set; }
        public object Metadata { get; set; }
    }
}
