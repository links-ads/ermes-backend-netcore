using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Quizzes
{
    [Table("quiz_translations")]
    public class QuizTranslation : Entity, IEntityTranslation<Quiz>
    {
        public const int MaxTextLength = 1000;
        public const int MaxDetailLength = 50;
        public Quiz Core { get; set; }
        public int CoreId { get; set; }
        public string Language { get; set; }

        [Required]
        [StringLength(MaxTextLength)]
        public string Text { get; set; }
        [Required]
        [StringLength(MaxDetailLength)]
        public string CrisisPhase { get; set; }
        [Required]
        [StringLength(MaxDetailLength)]
        public string EventContext { get; set; }
        [Required]
        [StringLength(MaxDetailLength)]
        public string Difficulty { get; set; }
    }
}
