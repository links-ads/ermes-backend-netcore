using Ermes.Dto.Datatable;
using Ermes.Dto.Spatial;
using Ermes.Enums;
using Ermes.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Alerts.Dto
{
    public class GetAlertsInput: DTPagedSortedAndFilteredInputDto, IDateRangeFilter, IBBoxFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public PointPosition NorthEastBoundary { get; set; }
        public PointPosition SouthWestBoundary { get; set; }
        public List<CapStatusType> Status { get; set; }
        public List<CapMsgType> MsgTypes { get; set; }
        public List<CapScopeType> Scopes { get; set; }
    }
}
