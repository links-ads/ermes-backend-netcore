using Abp.Runtime.Validation;
using Ermes.Dto;
using Ermes.Dto.Spatial;
using Ermes.Enums;
using Ermes.Organizations.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ermes.MapRequests.Dto
{
    public class MapRequestDto: ICustomValidate
    {
        public int Id { get; set; }
        public string Type { get; } = "MapRequest";
        public MapRequestType MapRequestType { get; set; }
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
        public List<BoundaryCondition> BoundaryConditions { get; set; }
        public string Description { get; set; }
        public bool DoSpotting { get; set; }
        public decimal ProbabilityRange { get; set; }
        public int TimeLimit { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (Frequency > 30)
                context.Results.Add(new ValidationResult("Frequancy must be <= 30"));
            if (Resolution < 10 || Resolution > 60)
                context.Results.Add(new ValidationResult("Resolution must be >= 10 and <= 60"));
            if (DataTypeIds == null || DataTypeIds.Count == 0)
                context.Results.Add(new ValidationResult("At least one DatatypeId must be associated to the map request"));
            if(ProbabilityRange > 100)
                context.Results.Add(new ValidationResult("ProbabilityRange must be <= 100"));
            if(BoundaryConditions != null && BoundaryConditions.Count > TimeLimit)
                context.Results.Add(new ValidationResult("Mismatch between TimeLimit value and BoundaryConditions length"));
            if(Duration == null || (Duration.UpperBound.Subtract(Duration.LowerBound).Days > 30))
                context.Results.Add(new ValidationResult("Max time span allowed is 30 days"));
        }
    }
}
