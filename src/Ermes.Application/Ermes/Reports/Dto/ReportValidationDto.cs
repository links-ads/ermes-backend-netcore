using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Reports.Dto
{
    [AutoMap(typeof(ReportValidation))]
    public class ReportValidationDto
    {
        public int ReportId { get; set; }
        public int PersonId { get; set; }
        public bool IsValid { get; set; }
        public string RejectionNote { get; set; }
    }
}
