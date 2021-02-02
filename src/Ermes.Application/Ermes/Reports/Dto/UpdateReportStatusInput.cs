using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Reports.Dto
{
    public class UpdateReportStatusInput
    {
        public int ReportId { get; set; }
        public GeneralStatus Status { get; set; }
    }
}
