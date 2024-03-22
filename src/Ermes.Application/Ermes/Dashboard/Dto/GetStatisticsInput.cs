using Ermes.Dto.Spatial;
using Ermes.Enums;
using Ermes.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Dashboard.Dto
{
    public class GetStatisticsInput : IDateRangeFilter, IBBoxFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public PointPosition NorthEastBoundary { get; set; }
        public PointPosition SouthWestBoundary { get; set; }

        public List<ActionStatusType> ActionStatusTypes { get; set; }
        public List<int> TeamIds { get; set; }

        public List<HazardType> ReportHazards { get; set; }
        public VisibilityType ReportVisibility { get; set; }

        public List<MissionStatusType> MissionStatus { get; set; }

        public List<CommunicationRestrictionType> CommunicationRestrictions { get; set; }
        public List<CommunicationScopeType> CommunicationScopes { get; set; }

        public List<MapRequestStatusType> MapRequestStatus { get; set; }
        public List<MapRequestType> MapRequestTypes { get; set; }
    }
}
