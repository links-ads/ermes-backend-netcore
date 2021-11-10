using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Layers.Dto
{
    public class LayerDto
    {
        public int DataTypeId { get; set; }
        public string Group { get; set; }
        //Key value for Group, not influenced by localization
        public string GroupKey { get; set; }
        public string SubGroup { get; set; }
        //Key value for SubGroup, not influenced by localization
        public string SubGroupKey { get; set; }
        public string Name { get; set; }
        public string PartnerName { get; set; }
        public List<LayerDetailsDto> Details { get; set; }
    }
}
