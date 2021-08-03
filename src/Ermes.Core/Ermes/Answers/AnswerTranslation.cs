using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Answers
{
    [Table("answer_translations")]
    public class AnswerTranslation : Entity, IEntityTranslation<Answer>
    {
        public const int MaxTextLength = 1000;

        public Answer Core { get; set; }
        public int CoreId { get; set; }
        public string Language { get; set; }

        [Required]
        [StringLength(MaxTextLength)]
        public string Text { get; set; }
    }
}
