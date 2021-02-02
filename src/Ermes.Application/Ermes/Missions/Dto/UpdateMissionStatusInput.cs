using Ermes.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace Ermes.Missions.Dto
{
    public class UpdateMissionStatusInput
    {
        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public MissionStatusType Status { get; set; }
        public int MissionId { get; set; }
    }
}
