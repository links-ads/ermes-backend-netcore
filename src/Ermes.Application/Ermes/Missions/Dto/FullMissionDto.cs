using NetTopologySuite.Features;
using NetTopologySuite.IO;
using Newtonsoft.Json;

namespace Ermes.Missions.Dto
{
    public class FullMissionDto : MissionDto
    {
        [JsonIgnore]
        public Feature Feature { get {
                var reader = new GeoJsonReader();
                return reader.Read<Feature>(AreaOfInterest);
            } set { } }
        public string AreaOfInterest { get; set; }
    }
}
