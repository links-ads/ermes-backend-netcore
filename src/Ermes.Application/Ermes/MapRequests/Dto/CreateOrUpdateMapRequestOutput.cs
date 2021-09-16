using Ermes.Dto.Spatial;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.MapRequests.Dto
{
    public class CreateOrUpdateMapRequestOutput
    {
        public FeatureDto<MapRequestDto> Feature { get; set; }
    }
}
