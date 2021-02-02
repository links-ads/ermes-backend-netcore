using Ermes.Dto.Spatial;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Ermes.Missions.Dto
{
    public class CreateOrUpdateMissionInput
    {
        [Required]
        public FeatureDto<MissionDto> Feature { get; set; }
    }
}
