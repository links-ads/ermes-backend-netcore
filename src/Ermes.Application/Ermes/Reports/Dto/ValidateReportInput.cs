using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Reports.Dto
{
    public class ValidateReportInput
    {
        public int ReportId { get; set; }
        public bool IsValid { get; set; }
        public string RejectionNote { get; set; }
    }
}
