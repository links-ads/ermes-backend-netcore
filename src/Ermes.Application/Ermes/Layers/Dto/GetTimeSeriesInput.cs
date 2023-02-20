using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Layers.Dto
{
    public class GetTimeSeriesInput
    {
        [Required]
        public string DatatypeId { get; set; }
        [Required]
        public string Point { get; set; } 
        public string RequestCode { get; set; }
        public string LayerName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
