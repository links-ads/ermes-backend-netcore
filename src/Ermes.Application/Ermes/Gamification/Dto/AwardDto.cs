using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Gamification.Dto
{
    [AutoMap(typeof(Award))]
    public class AwardDto: RewardDto
    {
        public int Points { get; set; }
        public string Description { get; set; }
    }
}
