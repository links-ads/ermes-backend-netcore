using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;

namespace Ermes.Consumers.RabbitMq
{
    public class RabbitMqAlertArea
    {
        public string AreaDesc { get; set; }
        [JsonIgnore]
        public Geometry FullGeometry
        {
            get
            {
                var reader = new GeoJsonReader();
                return reader.Read<Geometry>(Geometry);
            }
            set { }
        }
        public string Geometry { get; set; }
    }
}
