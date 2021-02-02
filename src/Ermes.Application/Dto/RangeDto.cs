using Abp.AutoMapper;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Dto
{
    [AutoMap(typeof(NpgsqlRange<DateTime>))]
    public class RangeDto<T>
    {
        public T LowerBound { get; set; }
        public T UpperBound { get; set; }
        public bool LowerBoundIsInclusive { get; set; }
        public bool UpperBoundIsInclusive { get; set; }
    }
}
