using Ermes.Dto.Spatial;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Dto
{
    public class GetEntityByIdOutput<T>
    {
        public FeatureDto<T> Feature { get; set; }
    }
}
