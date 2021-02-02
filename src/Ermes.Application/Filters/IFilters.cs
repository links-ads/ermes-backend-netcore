using Ermes.Dto.Spatial;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Filters
{
    public interface IDateRangeFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public interface IBBoxFilter
    {
        public PointPosition NorthEastBoundary { get; set; }
        public PointPosition SouthWestBoundary { get; set; }
    }
}
