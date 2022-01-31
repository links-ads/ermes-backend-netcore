using Abp.Domain.Entities;
using Ermes.Enums;
using Ermes.Helpers;
using Ermes.Persons;
using Ermes.Quizzes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Tips
{
    [Table("tips")]
    public class Tip : Entity, IMultiLingualEntity<TipTranslation>
    {
        public const int MaxCodeLength = 10;

        [Required]
        [StringLength(MaxCodeLength)]
        public string Code { get; set; }

        [Required]
        [Column("Hazard")]
        public string HazardString
        {
            get { return Hazard.ToString(); }
            private set { Hazard = value.ParseEnum<HazardType>(); }
        }

        [NotMapped]
        public HazardType Hazard { get; set; }

        [Required]
        [Column("CrisisPhase")]
        public string CrisisPhaseKeyString
        {
            get { return CrisisPhaseKey.ToString(); }
            private set { CrisisPhaseKey = value.ParseEnum<CrisisPhaseType>(); }
        }

        [NotMapped]
        public CrisisPhaseType CrisisPhaseKey { get; set; }

        [Required]
        [Column("EventContext")]
        public string EventContextKeyString
        {
            get { return EventContextKey.ToString(); }
            private set { EventContextKey = value.ParseEnum<EventContextType>(); }
        }

        [NotMapped]
        public EventContextType EventContextKey { get; set; }

        public string Url { get; set; }

        public virtual ICollection<TipTranslation> Translations { get; set; }
        public virtual ICollection<Quiz> Quizzes { get; set; }
        public virtual ICollection<PersonTip> Readers { get; set; }
    }
}
