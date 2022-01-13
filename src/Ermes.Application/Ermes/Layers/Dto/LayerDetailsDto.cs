using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Layers.Dto
{
    public class LayerDetailsDto
    {
        public string Name { get; set; }
        public List<DateTime> Timestamps { get; set; }
        public DateTime Created_At { get; set; }
    }
}
