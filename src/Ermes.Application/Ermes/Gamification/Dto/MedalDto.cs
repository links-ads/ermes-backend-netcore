using Abp.AutoMapper;
using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Gamification.Dto
{
    [AutoMap(typeof(Medal))]
    public class MedalDto: AchievementDto
    {
        public MedalType Type { get; set; }
    }
}
