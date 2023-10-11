using Abp.Runtime.Validation;
using Ermes.Enums;
using Ermes.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Stations.Dto
{
    public class GetMeasuresByStationAndSensorInput : IDateRangeFilter, ICustomValidate
    {
        public string StationId { get; set; }
        public string SensorId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }


        public void AddValidationErrors(CustomValidationContext context)
        {
            if (StationId == "")
                context.Results.Add(new ValidationResult("StationId is required"));
            if(SensorId == "")
                context.Results.Add(new ValidationResult("SensorId is required"));
        }
    }
}
