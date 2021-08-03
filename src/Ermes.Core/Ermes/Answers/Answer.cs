using Abp.Domain.Entities;
using Ermes.Quizzes;
using Ermes.Tips;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Answers
{
    [Table("answers")]
    public class Answer : Entity, IMultiLingualEntity<AnswerTranslation>
    {
        public const int MaxCodeLength = 10;

        [Required]
        [StringLength(MaxCodeLength)]
        public string Code { get; set; }

        public virtual Quiz Quiz { get; set; }
        public string QuizCode { get; set; }

        public bool IsTheRightAnswer { get; set; }

        public ICollection<AnswerTranslation> Translations { get; set; }
    }
}
