using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Stations.Dto
{
    public class SensorDto
    {
        public string Id { get; set; }
        public string StationId { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Unit { get; set; }
        public List<MeasureDto> Measurements { get; set; }
    }
}
