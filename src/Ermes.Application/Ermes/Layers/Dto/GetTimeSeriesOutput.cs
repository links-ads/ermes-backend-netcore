using Ermes.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public TimeSeriesVariableType Type
        {
            get
            {
                return Values != null && Values.Count > 0 ?
                        decimal.TryParse(Values.First().Value, out _) ? TimeSeriesVariableType.Number : Values.First().Value == "yes" || Values.First().Value == "no" || Boolean.TryParse(Values.First().Value, out _) ? TimeSeriesVariableType.Boolean : TimeSeriesVariableType.String :
                        TimeSeriesVariableType.Unknown;

            }
        }
        public List<TimeSeriesValueDto> Values { get; set; }

    }

    public class TimeSeriesValueDto
    {
        public DateTime DateTime { get; set; }
        public string Value { get; set; }

    }
}
