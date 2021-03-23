using Abp.SocialMedia.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Social.Dto
{
    public class GetEventStatisticsInput
    {
        public GetEventStatisticsInput()
        {
            Filters = new SocialBaseFilters();
        }
        public SocialBaseFilters Filters { get; set; }
    }
}
