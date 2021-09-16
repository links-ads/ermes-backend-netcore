﻿using Ermes.Dto;
using Ermes.Dto.Spatial;
using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.MapRequests.Dto
{
    public class MapRequestDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public RangeDto<DateTime> Duration { get; set; }
        public PointPosition Centroid { get; set; }
        public HazardType Hazard { get; set; }
        public LayerType Layer { get; set; }
        public int Frequency { get; set; }
        public int DataTypeId { get; set; }
        public MapRequestStatusType Status { get; set; }
        public string ErrorMessage { get; set; }
    }
}