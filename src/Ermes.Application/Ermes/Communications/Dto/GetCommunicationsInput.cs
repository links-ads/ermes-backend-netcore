﻿using Ermes.Dto.Datatable;
using Ermes.Dto.Spatial;
using Ermes.Enums;
using Ermes.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Communications.Dto
{
    public class GetCommunicationsInput : DTPagedSortedAndFilteredInputDto, IDateRangeFilter, IBBoxFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public PointPosition NorthEastBoundary { get; set; }
        public PointPosition SouthWestBoundary { get; set; }
        public List<CommunicationRestrictionType> Restrictions { get; set; }
        public List<CommunicationScopeType> Scopes { get; set; }
    }
}
