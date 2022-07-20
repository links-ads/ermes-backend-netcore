using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Gamification
{
    [Table("gamification_levels")]
    public class Level: Entity
    {
        [Required]
        public string Name { get; set; }
        public int LowerBound { get; set; }
        public int UpperBound { get; set; }

        [ForeignKey("PreviousLevelId")]
        public virtual Level PreviousLevel { get; set; }
        public int? PreviousLevelId { get; set; }


        [ForeignKey("FollowingLevelId")]
        public virtual Level FollowingLevel { get; set; }
        public int? FollowingLevelId { get; set; }

        public virtual ICollection<Barrier> Barriers { get; set; }
    }
}
