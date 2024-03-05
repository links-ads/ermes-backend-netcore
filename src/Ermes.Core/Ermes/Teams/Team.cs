using Abp.Domain.Entities.Auditing;
using Ermes.Organizations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ermes.Teams
{
    [Table("teams")]
    public class Team : AuditedEntity
    {
        public const int MAX_TEAMNAME_LENGTH = 255;
        public Team(int organizationId, string name)
        {
            OrganizationId = organizationId;
            Name = name;
        }

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }
        public virtual int OrganizationId { get; set; }

        [StringLength(MAX_TEAMNAME_LENGTH)]
        public string Name { get; set; }
    }
}
