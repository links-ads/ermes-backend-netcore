using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Dto.Spatial
{
    public class FeatureDto<T>
    {
        [JsonIgnore]
        public Geometry FullGeometry { get {
                var reader = new GeoJsonReader();
                return reader.Read<Geometry>(Geometry);
            } set { } }

        public string Geometry { get; set; }
        [Required]
        public T Properties { get; set; }
    }

    public class PointPosition
    {
        public PointPosition()
        {

        }

        public PointPosition(double lon, double lat)
        {
            Latitude = lat;
            Longitude = lon;
        }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
