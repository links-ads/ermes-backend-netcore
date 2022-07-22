using Abp.AutoMapper;
using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Gamification.Dto
{
    [AutoMap(typeof(Reward))]
    public class RewardDto
    {
        public CompetenceType Competence { get; set; }
        public string Name { get; set; }
    }
}
