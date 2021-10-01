using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Layers.Dto
{
    public class LayerSubGroupDto
    {
        public string SubGroupKey { get; set; }
        public string SubGroup { get; set; }
        public List<LayerDto> Layers { get; set; }
    }
}
