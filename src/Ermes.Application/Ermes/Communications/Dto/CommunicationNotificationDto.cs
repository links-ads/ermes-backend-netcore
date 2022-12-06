using Ermes.Dto;
using Ermes.Dto.Spatial;
using Ermes.Enums;
using System;
using System.Collections.Generic;

namespace Ermes.Communications.Dto
{
    public class CommunicationNotificationDto
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public RangeDto<DateTime> Duration { get; set; }
        public PointPosition Centroid { get; set; }
        public CommunicationScopeType Scope { get; set; }
        public CommunicationRestrictionType Restriction { get; set; }
        public List<int> OrganizationReceiverIds { get; set; }

    }
}
