﻿using Ermes.Dto;
using Ermes.Dto.Spatial;
using Ermes.Enums;
using Ermes.Organizations.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Communications.Dto
{
    public class CommunicationDto
    {
        public int Id { get; set; }
        [Required]
        public string Message { get; set; }
        public RangeDto<DateTime> Duration { get; set; }
        public PointPosition Centroid { get; set; }
        public string OrganizationName { get; set; }
        public CommunicationScopeType Scope { get; set; }
        public CommunicationRestrictionType Restriction { get; set; }
        [NotMapped]
        public List<int> OrganizationIdList { get; set; }
    }
}
