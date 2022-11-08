using Abp.AutoMapper;
using Ermes.Enums;
using System.Collections.Generic;

namespace Ermes.MapRequests.Dto
{
    public class MapRequestLayerDto
    {
        public int LayerDataTypeId { get; set; }
        public string MapRequestCode { get; set; }
        public LayerImportStatusType Status { get; set; }
        public int ReceivedUpdates { get; set; }
        public List<MapRequestLayerErrorDto> ErrorMessages { get; set; }
    }
}
