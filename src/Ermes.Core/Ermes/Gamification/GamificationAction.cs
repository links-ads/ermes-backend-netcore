using Abp.Domain.Entities;
using Ermes.Enums;
using Ermes.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Gamification
{
    [Table("gamification_actions")]
    public class GamificationAction: Entity
    {
        public const int MaxCodeLength = 100;
        public const int MaxNameLength = 100;
        public const int MaxDescriptionLength = 1000;

        [Required]
        [StringLength(MaxCodeLength)]
        public string Code { get; set; }
        [Required]
        [StringLength(MaxNameLength)]
        public string Name { get; set; }
        [StringLength(MaxDescriptionLength)]
        public string Description { get; set; }

        public int Points { get; set; }
        [Column("Competence")]
        public string CompetenceString
        {
            get { return Competence.ToString(); }
            private set { Competence = value.ParseEnum<CompetenceType>(); }
        }
        [NotMapped]
        public CompetenceType Competence { get; set; }

        public ICollection<Achievement> Achievements { get; set; }
    }
}
