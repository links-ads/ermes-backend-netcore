using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.SensorService.Model
{
    public class SensorServiceStation
    {
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }
        public Guid Id2 { get { 
                return new Guid(Id);
            } 
        }
        public string Key { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public SensorServiceStationLocation Location { get; set; }
        public decimal Altitude { get; set; }
        public List<SensorServiceSensor> Sensors { get; set; }
    }

    public class SensorServiceStationLocation
    {
        public string Type { get; set; } = "Point";
        public decimal[] Coordinates { get; set; }
    }
}
