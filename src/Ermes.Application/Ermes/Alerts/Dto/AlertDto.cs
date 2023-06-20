using Abp.Application.Services.Dto;
using Ermes.Dto.Spatial;
using Ermes.Enums;
using System;
using System.Collections.Generic;

namespace Ermes.Alerts.Dto
{
    public class AlertDto: EntityDto
    {
        public string Identifier { get; set; }
        public string Type { get; } = "Alert";
        public string Sender { get; set; }
        public DateTime Sent { get; set; }
        public CapStatusType Status { get; set; }
        public CapMsgType MsgType { get; set; }
        public string Source { get; set; }
        public CapScopeType Scope { get; set; }
        public string Code { get; set; }
        public string Note { get; set; }
        public string References { get; set; }
        public string Restriction { get; set; }
        public string Region { get; set; }
        public bool IsARecommendation { get; set; }
        public PointPosition Centroid { get; set; }
        public List<CapInfoDto> Info { get; set; }
    }
}
