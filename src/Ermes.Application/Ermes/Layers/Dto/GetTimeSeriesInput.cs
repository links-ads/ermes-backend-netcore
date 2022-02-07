using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Layers.Dto
{
    public class GetTimeSeriesInput
    {
        public string DataTypeId { get; set; }
        public string Point { get; set; }
        public string Attribute { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
    }
}
