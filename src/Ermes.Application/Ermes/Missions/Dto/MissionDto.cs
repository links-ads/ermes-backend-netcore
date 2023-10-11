using Abp.AutoMapper;
using Ermes.Dto;
using Ermes.Dto.Spatial;
using Ermes.Enums;
using Ermes.Organizations.Dto;
using Ermes.Reports.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ermes.Missions.Dto
{
    [AutoMap(typeof(Mission))]
    public class MissionDto
    {
        public int Id { get; set; }
        public string Type { get; } = "Mission";
        [Required]
        [StringLength(Mission.MAX_TITLE_LENGTH)]
        public string Title { get; set; }
        [StringLength(Mission.MAX_DESCRIPTION_LENGTH)]
        public string Description { get; set; }
        public RangeDto<DateTime> Duration { get; set; }
        public MissionStatusType CurrentStatus { get; set; }
        public long? CoordinatorPersonId { get; set; }
        public int? CoordinatorTeamId { get; set; }
        public int OrganizationId { get; set; }
        public OrganizationDto Organization { get; set; }
        [StringLength(Mission.MAX_NOTES_LENGTH)]
        public string Notes { get; set; }
        public PointPosition Centroid { get; set; }
        public List<ReportDto> Reports { get; set; }
    }
}
