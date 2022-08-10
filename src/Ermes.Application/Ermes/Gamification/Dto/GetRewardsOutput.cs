using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Gamification.Dto
{
    public class GetRewardsOutput
    {
        public List<AwardDto> Awards { get; set; }
        public List<MedalDto> Medals { get; set; }
        public List<BadgeDto> Badges  { get; set; }
    }
}
