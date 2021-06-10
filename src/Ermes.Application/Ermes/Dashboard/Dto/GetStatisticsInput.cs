using Ermes.Dto.Spatial;
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
    }
}
