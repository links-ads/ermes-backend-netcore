using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Layers.Dto
{
    public class LayerGroupDto
    {
        public string GroupKey { get; set; }
        public string Group { get; set; }
        public List<LayerSubGroupDto> SubGroups { get; set; }
    }
}
