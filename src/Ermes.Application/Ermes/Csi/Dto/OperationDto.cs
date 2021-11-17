using Abp.AutoMapper;
using Ermes.Enums;
using Ermes.Operations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Csi.Dto
{
    [AutoMap(typeof(Operation))]
    public class OperationDto
    {
        public long PersonId { get; set; }
        public VolterOperationType Type { get; set; }
        public int PersonLegacyId { get; set; }
        public int OperationLegacyId { get; set; }
        public string Request { get; set; }
        public VolterResponse Response { get; set; }
    }
}
