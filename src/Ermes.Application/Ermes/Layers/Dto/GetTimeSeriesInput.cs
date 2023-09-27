using Abp.Runtime.Validation;
using Ermes.Helpers;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Ermes.Layers.Dto
{
    public class GetTimeSeriesInput : ICustomValidate
    {
        [Required]
        public string DatatypeId { get; set; }
        [Required]
        public string Point { get; set; }
        [Required]
        public string Crs { get; set; }
        public string RequestCode { get; set; }
        public string LayerName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            var geo = ErmesCommon.GetGeometryFromWKT(Point);
            if (geo == null || geo.GeometryType != AppConsts.GEOMETRY_POINT)
            {
                context.Results.Add(new ValidationResult("Point must be a valid WKT string"));
            }
        }
    }
}
