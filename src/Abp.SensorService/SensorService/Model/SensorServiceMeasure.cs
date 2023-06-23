using Newtonsoft.Json;
using System;

namespace Abp.SensorService.Model
{
    public class SensorServiceMeasure
    {
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }
        public string StationId { get; set; }
        public string SensorId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string Measure { get; set; }
        public object Metadata { get; set; }
    }
}
