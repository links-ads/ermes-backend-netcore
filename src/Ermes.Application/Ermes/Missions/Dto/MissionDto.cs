using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Ermes.Dto;
using Ermes.Dto.Spatial;
using Ermes.Enums;
using Ermes.Organizations.Dto;
using Ermes.Profile.Dto;
using Ermes.Reports;
using Ermes.Reports.Dto;
using Microsoft.VisualBasic.CompilerServices;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Missions.Dto
{
    [AutoMap(typeof(Mission))]
    public class MissionDto
    {
        public int Id { get; set; }
        public string Type { get; } = "Mission";
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public RangeDto<DateTime> Duration { get; set; }
        public MissionStatusType CurrentStatus { get; set; }
        public long? CoordinatorPersonId { get; set; }
        public int? CoordinatorTeamId { get; set; }
        public int OrganizationId { get; set; }
        public OrganizationDto Organization { get; set; }
        public string Notes { get; set; }
        public PointPosition Centroid { get; set; }
        public List<ReportDto> Reports { get; set; }
    }
}
