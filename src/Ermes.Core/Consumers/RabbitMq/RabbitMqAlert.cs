using Ermes.Enums;
using System;
using System.Collections.Generic;

namespace Ermes.Consumers.RabbitMq
{
    public class RabbitMqAlert
    {
        public string Identifier { get; set; }
        public string Sender { get; set; }
        public DateTime Sent { get; set; }
        public CapStatusType Status { get; set; }
        public CapMsgType MsgType { get; set; }
        public string Source { get; set; }
        public CapScopeType Scope { get; set; }
        public string Region { get; set; }
        public string AreaID { get; set; }
        public string Restriction { get; set; }
        public List<RabbitMqAlertInfo> Info { get; set; }
    }
}
