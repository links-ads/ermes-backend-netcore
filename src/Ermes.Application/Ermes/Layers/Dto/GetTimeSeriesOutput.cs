using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Ermes.Layers.Dto
{
    public class GetTimeSeriesOutput
    {
        public List<TimeSeriesVariableDto> Variables { get; set; }

    }

    public class TimeSeriesVariableDto
    {
        [JsonProperty("var_name")]
        public string VariableName { get; set; }
        public List<TimeSeriesValueDto> Values { get; set; }
    }

    public class TimeSeriesValueDto
    {
        public DateTime DateTime { get; set; }
        public decimal Value { get; set; }
    }
}
