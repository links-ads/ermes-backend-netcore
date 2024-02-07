using Abp.AutoMapper;
using Ermes.Dto.Spatial;
using Ermes.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ermes.Reports.Dto
{
    public class ReportDto
    {
        public int Id { get; set; }
        public string Type { get; } = "Report";
        public HazardType Hazard { get; set; }
        public GeneralStatus Status { get; set; }
        public ReportContentType Content { get; set; }
        public PointPosition Location { get; set; }
        public DateTime Timestamp { get; set; }
        public List<MediaURIDto> MediaURIs { get; set; }
        public List<ReportExtensionData> ExtensionData { get; set; }
        [StringLength(Report.MAX_DESCRIPTION_LENGTH)]
        public string Description { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string DisplayName
        {
            get
            {
                return Username ?? Email;
            }
        }
        public string OrganizationName { get; set; }
        public SourceDeviceType Source { get; set; }
        public bool IsEditable { get; set; }
        [JsonIgnore]
        public long CreatorId { get; set; }
        public int? RelativeMissionId { get; set; }
        public List<ReportTag> Tags { get; set; }
        public List<ReportAdultInfo> AdultInfo { get; set; }
        public bool IsPublic { get; set; }
        public int Points { get; set; }
        public int? LevelId { get; set; }
        public string LevelName { get; set; }
        public int EarnedPoints { get; set; }
        public bool CanBeValidated { get; set; }
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
        public List<ReportValidationDto> Validations { get; set; }
        public bool Read { get; set; }
    }
}
