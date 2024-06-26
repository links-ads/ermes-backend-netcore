﻿using Abp.AutoMapper;
using Ermes.Dto.Spatial;
using Ermes.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Ermes.Reports.Dto
{
    public class ReportNotificationDto
    {
        public int Id { get; set; }
        public HazardType Hazard { get; set; }
        public GeneralStatus Status { get; set; }
        public PointPosition Location { get; set; }
        public DateTime Timestamp { get; set; }
        public string Address { get; set; }
        public List<string> MediaURIs { get; set; }
        public List<ReportExtensionData> ExtensionData { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public List<ReportTarget> Targets { get; set; }
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
        public int? OrganizationId { get; set; }
        public SourceDeviceType Source { get; set; }
        public bool IsEditable { get; set; }
    }
}
