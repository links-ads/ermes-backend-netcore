using Abp.Domain.Entities.Auditing;
using Ermes.Quizzes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Persons
{
    [Table("person_quizzes")]
    public class PersonQuiz: CreationAuditedEntity
    {
        public PersonQuiz(long personId, string quizCode)
        {
            PersonId = personId;
            QuizCode = quizCode;
        }

        public virtual Person Person { get; set; }
        public long PersonId { get; set; }
        public virtual Quiz Quiz { get; set; }
        public string QuizCode { get; set; }
    }
}
