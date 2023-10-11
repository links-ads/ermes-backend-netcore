using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.SensorService.Model
{
    public class SensorServiceSensor
    {
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }
        public string StationId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public List<SensorServiceMeasure> Measurements { get; set; }
    }
}
