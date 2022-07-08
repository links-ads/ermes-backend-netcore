using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Gamification.Dto
{
    public class BadgeDto: AchievementDto
    {
        public HazardType Hazard { get; set; }
    }
}
