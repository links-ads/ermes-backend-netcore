using Abp.Domain.Entities;
using Ermes.Answers;
using Ermes.Enums;
using Ermes.Helpers;
using Ermes.Tips;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Quizzes
{
    [Table("quizzes")]
    public class Quiz : Entity, IMultiLingualEntity<QuizTranslation>
    {
        public const int MaxCodeLength = 10;

        [Required]
        [StringLength(MaxCodeLength)]
        public string Code { get; set; }


        public virtual Tip Tip { get; set; }
        public string TipCode { get; set; }

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

        [Required]
        [Column("Difficulty")]
        public string DifficultyKeyString
        {
            get { return DifficultyKey.ToString(); }
            private set { DifficultyKey = value.ParseEnum<DifficultyType>(); }
        }

        [NotMapped]
        public DifficultyType DifficultyKey { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }
        public virtual ICollection<QuizTranslation> Translations { get; set; }
    }
}
