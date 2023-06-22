using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.SensorService.Model
{
    public class SensorServiceMeasure
    {
        [JsonProperty(PropertyName = "_id")]
        public Guid Id { get; set; }
        public Guid StationId { get; set; }
        public Guid SenssorId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string Measure { get; set; }
        public object Metadata { get; set; }
    }
}
