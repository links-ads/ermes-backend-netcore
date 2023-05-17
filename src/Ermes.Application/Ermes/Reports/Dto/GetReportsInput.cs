using Ermes.Dto.Datatable;
using Ermes.Dto.Spatial;
using Ermes.Enums;
using Ermes.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Reports.Dto
{
    public class GetReportsInput : DTPagedSortedAndFilteredInputDto, IDateRangeFilter, IBBoxFilter
    {
        public List<HazardType> Hazards { get; set; }
        public List<GeneralStatus> Status { get; set; }
        public List<ReportContentType> Contents { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        //kept for retro-compatibility in the SDKs
        public int ReportRequestId { get; set; }
        public bool FilterByCreator { get; set; }
        public PointPosition NorthEastBoundary { get; set; }
        public PointPosition SouthWestBoundary { get; set; }
        public VisibilityType Visibility { get; set; }
    }
}
