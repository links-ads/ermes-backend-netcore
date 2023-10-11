using Ermes.Enums;
using System;
using System.Collections.Generic;

namespace Ermes.Consumers.RabbitMq
{
    public class RabbitMqAlertInfo
    {
        public RabbitMqAlertInfo()
        {

        }
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
        public List<RabbitMqAlertArea> Area { get; set; }
    }
}
