using Abp.AutoMapper;
using Ermes.Dto.Spatial;
using Ermes.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Ermes.Reports.Dto
{
    public class ReportDto
    {
        public int Id { get; set; }
        public HazardType Hazard { get; set; }
        public GeneralStatus Status { get; set; }
        public ReportContentType Content { get; set; }
        public PointPosition Location { get; set; }
        public DateTime Timestamp { get; set; }
        public List<MediaURIDto> MediaURIs { get; set; }
        public List<ReportExtensionData> ExtensionData { get; set; }
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
    }
}
