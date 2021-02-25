using Abp.AutoMapper;
using Ermes.Dto;
using Ermes.Dto.Datatable;
using Ermes.Dto.Spatial;
using Ermes.Enums;
using Ermes.Notifiers;
using Ermes.ReportRequests;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Reports.Dto
{
    public class ReportRequestNotificationDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public RangeDto<DateTime> Duration { get; set; }
        public int? OrganizationId { get; set; }
        public List<int> SelectedCategories { get; set; }
        public PointPosition Centroid { get; set; }
    }
}
