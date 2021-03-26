using Abp.SocialMedia.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Social.Dto
{
    public class GetEventsInput
    {
        public GetEventsInput()
        {
            Filters = new EventFilters();
        }
        public EventFilters Filters { get; set; }
    }
}
