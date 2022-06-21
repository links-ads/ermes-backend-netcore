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
    public class GamificationAction: Entity, IMultiLingualEntity<GamificationActionTranslation>
    {
        public const int MaxCodeLength = 100;

        [Required]
        [StringLength(MaxCodeLength)]
        public string Code { get; set; }
        public int Points { get; set; }
        [Column("Competence")]
        public string CompetenceString
        {
            get { return Competence.ToString(); }
            private set { Competence = value.ParseEnum<CompetenceType>(); }
        }
        [NotMapped]
        public CompetenceType Competence { get; set; }

        public ICollection<GamificationActionTranslation> Translations { get; set; }
    }
}
