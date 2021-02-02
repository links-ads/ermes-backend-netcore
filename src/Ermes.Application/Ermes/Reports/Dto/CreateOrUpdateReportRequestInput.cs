using Ermes.Dto.Spatial;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Reports.Dto
{
    public class CreateOrUpdateReportRequestInput
    {
        [Required]
        public FeatureDto<ReportRequestDto> Feature { get; set; }
    }
}
