using System;

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
