using Abp.Domain.Entities.Auditing;
using Ermes.Organizations;
using Ermes.Persons;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Teams
{
    [Table("teams")]
    public class Team:AuditedEntity
    {
        private const int MaxTeamNameLength = 255;

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }
        public virtual int OrganizationId { get; set; }

        [StringLength(MaxTeamNameLength)]
        public string Name { get; set; }
    }
}
