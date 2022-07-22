using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Tips
{
    [Table("tip_translations")]
    public class TipTranslation : Entity, IEntityTranslation<Tip>
    {
        public const int MaxTitleLength = 255;
        public const int MaxDetailLength = 50;
        public const int MaxTextLength = 1000;
        public const int MaxLanguageLength = 2;

        public Tip Core { get; set; }
        public int CoreId { get; set; }
        [StringLength(MaxLanguageLength)]
        public string Language { get; set; }
        [Required]
        [StringLength(MaxTitleLength)]
        public string Title { get; set; }

        [Required]
        [StringLength(MaxTextLength)]
        public string Text { get; set; }

        [Required]
        [StringLength(MaxDetailLength)]
        public string CrisisPhase { get; set; }
        [Required]
        [StringLength(MaxDetailLength)]
        public string EventContext { get; set; }
    }
}
