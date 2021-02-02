using Abp.Timing;
using Ermes.Dto;
using Ermes.Dto.Datatable;
using Ermes.Dto.Spatial;
using Ermes.Enums;
using Ermes.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Missions.Dto
{
    public class GetMissionsInput : DTPagedSortedAndFilteredInputDto, IDateRangeFilter, IBBoxFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<MissionStatusType> Status { get; set; }
        public PointPosition NorthEastBoundary { get; set; }
        public PointPosition SouthWestBoundary { get; set; }
    }
}
