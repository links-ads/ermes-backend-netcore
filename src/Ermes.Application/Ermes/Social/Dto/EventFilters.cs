using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Social.Dto
{
    public class EventFilters : SocialBaseFilters, ISocialPaginationInput
    {
        public int? Page { get; set; }
        public int? Limit { get; set; }
    }
}
