using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Ermes.Gamification.Dto
{
    public class CheckPersonAnswerInput
    {
        [Required]
        public string AnswerCode { get; set; }
    }
}
