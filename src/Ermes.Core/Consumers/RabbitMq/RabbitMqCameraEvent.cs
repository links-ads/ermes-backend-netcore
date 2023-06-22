using Newtonsoft.Json;
using System;

namespace Ermes.Consumers.RabbitMq
{
    public class RabbitMqCameraEvent
    {
        public DateTime Timestamp { get; set; }
        public string Link { get; set; }
        public Camera Camera { get; set; }
        public CameraDetection Detection { get; set; }
        [JsonProperty(propertyName: "class_of_fire")]
        public ClassOfFire ClassOfFire { get; set; }
        [JsonProperty(propertyName: "fire_location")]
        public FireLocation FireLocation { get; set; }
    }

    public class Camera
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        [JsonProperty(propertyName: "cam_direction")]
        public string CamDirection { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal Altitude { get; set; }
    }

    public class CameraDetection
    {
        [JsonProperty(propertyName: "not_available")]
        public bool NotAvailable { get; set; }
        public bool Smoke { get; set; }
        public bool Fire { get; set; }
    }

    public class ClassOfFire
    {
        [JsonProperty(propertyName: "not_available")]
        public bool NotAvailable { get; set; }
        [JsonProperty(propertyName: "class_1")]
        public bool Class1 { get; set; }
        [JsonProperty(propertyName: "class_2")]
        public bool Class2 { get; set; }
        [JsonProperty(propertyName: "class_3")]
        public bool Class3 { get; set; }
    }

    public class FireLocation
    {
        [JsonProperty(propertyName: "not_available")]
        public bool NotAvailable { get; set; }
        public decimal? Direction { get; set; }
        public decimal? Distance { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }

}
