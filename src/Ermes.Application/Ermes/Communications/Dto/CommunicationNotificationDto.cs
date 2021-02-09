using Ermes.Dto;
using Ermes.Dto.Spatial;
using Ermes.Enums;
using Ermes.Notifiers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Communications.Dto
{
    public class CommunicationNotificationDto
    {
        public int Id { get; set; }
        public int? OrganizationId { get; set; }
        public string Message { get; set; }
        public RangeDto<DateTime> Duration { get; set; }
        public PointPosition Centroid { get; set; }
    }
}
