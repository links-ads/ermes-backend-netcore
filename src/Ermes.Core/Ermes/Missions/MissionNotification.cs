using Ermes.Enums;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Missions
{
    public class MissionNotification
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public NpgsqlRange<DateTime> Duration { get; set; }
        //Field that maps CurrentStatus props, managed in CustomDtoMapper
        //In this way, ReportNotificatioDto and MissionNotificationDto share the same prop name for this field
        public MissionStatusType Status { get; set; }
        public long? CoordinatorPersonId { get; set; }
        public int? CoordinatorTeamId { get; set; }
        public int? OrganizationId { get; set; }
        public string Notes { get; set; }
    }
}
