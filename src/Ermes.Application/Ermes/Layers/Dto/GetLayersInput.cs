using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Layers.Dto
{
    public class GetLayersInput
    {
        public List<string> DataTypeIds { get; set; }
        public string Bbox { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string MapRequestCode { get; set; }
    }
}
