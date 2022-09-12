using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.MapRequests.Dto
{
    public class MapRequestLayerDto
    {
        public int LayerDataTypeId { get; set; }
        public string MapRequestCode { get; set; }
        public LayerImportStatusType Status { get; set; }
        public string ErrorMessage { get; set; }
    }
}
