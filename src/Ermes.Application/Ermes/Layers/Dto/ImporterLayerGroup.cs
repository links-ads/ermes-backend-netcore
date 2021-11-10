using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Layers.Dto
{
    public class ImporterLayerGroup
    {
        public int DataType_Id { get; set; }
        public List<LayerDetailsDto> Details { get; set; }
    }
}
