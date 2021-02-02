
using Ermes.Dto.Datatable;
using Ermes.Dto.Spatial;
using Ermes.Enums;
using Ermes.Filters;
using System;
using System.Collections.Generic;

namespace Ermes.Actions.Dto
{
    public class GetActionsInput : DTPagedSortedAndFilteredInputDto, IDateRangeFilter, IBBoxFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<ActionStatusType> StatusTypes { get; set; }
        public List<int> ActivityIds { get; set; }
        public PointPosition NorthEastBoundary { get; set; }
        public PointPosition SouthWestBoundary { get; set; }
    }
}
