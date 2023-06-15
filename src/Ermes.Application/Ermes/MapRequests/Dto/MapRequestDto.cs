using Ermes.Dto;
using Ermes.Dto.Spatial;
using Ermes.Enums;
using Ermes.Organizations.Dto;
using System;
using System.Collections.Generic;

namespace Ermes.MapRequests.Dto
{
    public class MapRequestDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public RangeDto<DateTime> Duration { get; set; }
        public PointPosition Centroid { get; set; }
        public int Frequency { get; set; }
        public List<int> DataTypeIds { get; set; }
        public MapRequestStatusType Status { get; set; }
        public OrganizationDto Organization { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string DisplayName
        {
            get
            {
                return Username ?? Email;
            }
        }
        public int Resolution { get; set; } = 10;
        public List<MapRequestLayerDto> MapRequestLayers { get; set; }
        public string Type { get; set; }
        public List<BoundaryCondition> BoundaryConditions { get; set; }
        public string Description { get; set; }
        public bool DoSpotting { get; set; }
        public decimal ProbabilityRange { get; set; }
        public int TimeLimit { get; set; }
    }
}
