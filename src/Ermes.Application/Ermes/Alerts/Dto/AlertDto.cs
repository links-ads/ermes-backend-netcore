using Castle.MicroKernel.SubSystems.Conversion;
using Ermes.Enums;
using Ermes.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Alerts.Dto
{
    public class AlertDto
    {
        public string Identifier { get; set; }
        public string Sender { get; set; }
        public DateTime Sent { get; set; }
        public CapStatusType Status { get; set; }
        public CapMsgType MsgType { get; set; }
        public string Source { get; set; }
        public CapScopeType Scope { get; set; }
        public string Code { get; set; }
        public string Note { get; set; }
        public string References { get; set; }
        public bool IsARecommendation { get; set; }
        public List<CapInfoDto> Info { get; set; }
    }
}
