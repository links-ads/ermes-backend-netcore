using Ermes.Dto.Spatial;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Communications.Dto
{
    public class CreateOrUpdateCommunicationOutput
    {
        public FeatureDto<CommunicationDto> Feature { get; set; }
    }
}
