using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Gamification
{
    [Table("gamification_barriers")]
    public class Barrier: Entity
    {
        [ForeignKey("LevelName")]
        public virtual Level Level { get; set; }
        public string LevelName { get; set; }

        [ForeignKey("RewardName")]
        public virtual Reward Reward { get; set; }
        public string RewardName { get; set; }
    }
}
