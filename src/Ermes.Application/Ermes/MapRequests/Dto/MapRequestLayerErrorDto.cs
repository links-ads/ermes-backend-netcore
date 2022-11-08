using Abp.AutoMapper;
using System;

namespace Ermes.MapRequests.Dto
{
    public class MapRequestLayerErrorDto
    {
        public string Message { get; set; }
        public DateTime AcquisitionDate { get; set; }
    }
}
