using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ermes.Web.Converters
{
    public class GeometryJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            GeoJsonWriter geoJsonWriter = new GeoJsonWriter();
            if (value is Polygon || value is MultiPolygon || value is FeatureCollection)
            {
                geoJsonWriter.Write(value, writer);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            GeoJsonReader geoJsonReader = new GeoJsonReader();
            if (objectType == typeof(Polygon))
            {
                var a = geoJsonReader.Read<Polygon>(reader.Value.ToString());
                return a;
            }
            else if (objectType == typeof(MultiPolygon))
            {
                var a = geoJsonReader.Read<MultiPolygon>(reader.Value.ToString());
                return a;
            }
            else if (objectType == typeof(Geometry))
            {
                var a = geoJsonReader.Read<Geometry>(reader.Value.ToString());
                return a;
            }
            else if (objectType == typeof(FeatureCollection))
            {
                var a = geoJsonReader.Read<FeatureCollection>(reader.Value.ToString());
                return a;
            }

            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(MultiPolygon) || objectType == typeof(Polygon) || objectType == typeof(Geometry) || objectType == typeof(FeatureCollection);
        }
    }
}
