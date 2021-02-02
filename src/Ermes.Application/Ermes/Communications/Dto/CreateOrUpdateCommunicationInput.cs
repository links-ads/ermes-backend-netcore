using Ermes.Dto.Spatial;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Communications.Dto
{
    public class CreateOrUpdateCommunicationInput
    {
        [Required]
        public FeatureDto<CommunicationDto> Feature { get; set; }
    }
}
