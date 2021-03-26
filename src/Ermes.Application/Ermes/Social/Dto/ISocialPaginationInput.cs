using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Social.Dto
{
    public interface ISocialPaginationInput
    {
        public int? Page { get; set; }
        public int? Limit { get; set; }
    }
}
