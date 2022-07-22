using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Gamification.Dto
{
    [AutoMap(typeof(Achievement))]
    public class AchievementDto: RewardDto
    {
        public string GamificationActionCode { get; set; }
    }
}
