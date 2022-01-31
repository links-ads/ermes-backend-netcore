using Abp.Domain.Entities.Auditing;
using Ermes.Tips;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Persons
{
    [Table("person_tips")]
    public class PersonTip : CreationAuditedEntity
    {
        public PersonTip(long personId, string tipCode)
        {
            PersonId = personId;
            TipCode = tipCode;
        }

        public virtual Person Person { get; set; }
        public long PersonId { get; set; }
        public virtual Tip Tip { get; set; }
        public string TipCode { get; set; }
    }
}
