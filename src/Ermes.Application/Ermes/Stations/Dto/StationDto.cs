using Ermes.Dto.Spatial;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Stations.Dto
{
    public class StationDto
    {
        public string Id { get; set; }
        public string Type { get; } = "Station";
        public string Name { get; set; }
        public string Owner { get; set; }
        public string Brand { get; set; }
        public string StationType { get; set; }
        public PointPosition Location { get; set; }
        public decimal Altitude { get; set; }
        public List<SensorDto> Sensors { get; set; }
    }
}
