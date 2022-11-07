using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.MapRequests.Dto
{
    public class MapRequestLayerErrorDto
    {
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
