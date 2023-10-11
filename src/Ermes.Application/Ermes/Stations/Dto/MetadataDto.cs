using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Stations.Dto
{
    public class MetadataDto
    {
        public MeasureDetection Detection { get; set; }
    }

    public class MeasureDetection
    {
        public bool not_available { get; set; }
        public bool fire { get; set; }
        public bool smoke { get; set; }
    }
}