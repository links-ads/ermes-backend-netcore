using Abp.SocialMedia.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Social.Dto
{
    public class GetTweetStatisticsInput
    {
        public GetTweetStatisticsInput()
        {
            Filters = new TweetStatFilters();
        }
        public TweetStatFilters Filters { get; set; }
    }
}
