using Ermes.Enums;
using Ermes.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Alerts.Dto
{
    public class CapInfoDto
    {
        public string Language { get; set; }
        public CapCategoryType Category { get; set; }
        public string Event { get; set; }
        public CapResponseType ResponseType { get; set; }
        public CapUrgencyType Urgency { get; set; }
        public CapSeverityType Severity { get; set; }
        public CapCertaintyType Certainty { get; set; }
        public DateTime Expires { get; set; }
        public string Headline { get; set; }
        public string Description { get; set; }
        public List<CapAreaDto> Area { get; set; }
    }
}
