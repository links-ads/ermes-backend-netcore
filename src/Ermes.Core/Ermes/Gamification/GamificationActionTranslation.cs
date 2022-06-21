using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Gamification
{
    [Table("gamification_action_translations")]
    public class GamificationActionTranslation: Entity, IEntityTranslation<GamificationAction>
    {
        public const int MaxDescriptionLength = 1000;
        public const int MaxLanguageLength = 2;

        public GamificationAction Core { get; set; }
        public int CoreId { get; set; }

        [Required]
        [StringLength(MaxLanguageLength)]
        public string Language { get; set; }

        [Required]
        [StringLength(MaxDescriptionLength)]
        public string Description { get; set; }
    }
}
