using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Ermes.Roles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Persons
{
    [Table("person_roles")]
    public class PersonRole : CreationAuditedEntity
    {
        [ForeignKey("PersonId")]
        public virtual Person Person { get; set; }
        public virtual long PersonId { get; set; }

        /// <summary>
        /// Reference to role
        /// </summary>
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
        public virtual int RoleId { get; set; }
    }
}
