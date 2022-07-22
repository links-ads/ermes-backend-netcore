using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Ermes.Persons;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Gamification
{
    /// <summary>
    /// This table stores all the gamification actions performed by citizens.
    /// </summary>
    [Table("gamification_audit")]
    public class GamificationAudit : Entity, IHasCreationTime
    {
        [ForeignKey("PersonId")]
        public virtual Person Person { get; set; }
        public long PersonId { get; set; }

        [ForeignKey("RewardId")]
        public virtual Reward Reward { get; set; }
        public int? RewardId { get; set; }

        [ForeignKey("GamificationActionId")]
        public virtual GamificationAction GamificationAction { get; set; }
        public int? GamificationActionId { get; set; }

        [ForeignKey("LevelId")]
        public virtual Level Level { get; set; }
        public int? LevelId { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
