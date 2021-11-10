using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Layers.Dto
{
    public class ImporterLayerGroup
    {
        public int DataTypeId { get; set; }
        public List<LayerDetailsDto> Details { get; set; }
    }
}
