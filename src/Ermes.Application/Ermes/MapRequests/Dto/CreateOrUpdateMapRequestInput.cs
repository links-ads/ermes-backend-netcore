using Ermes.Dto.Spatial;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.MapRequests.Dto
{
    public class CreateOrUpdateMapRequestInput
    {
        [Required]
        public FeatureDto<MapRequestDto> Feature { get; set; }
    }
}
