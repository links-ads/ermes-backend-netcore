using Abp.Domain.Entities.Auditing;
using Ermes.Organizations;
using Ermes.Persons.Cache;
using Ermes.Teams;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ermes.Persons
{
    [Table("persons")]
    public class Person : AuditedEntity<long>, IPersonBase
    {
        private const int MaxUsernameLength = 255;
        /// <summary>
        /// UserId assign to the user by Fusion Auth app
        /// </summary>
        [Required]
        public Guid FusionAuthUserGuid { get; set; }

        /// <summary>
        /// Reference to organization
        /// </summary>
        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }
        public virtual int? OrganizationId { get; set; }

        [ForeignKey("TeamId")]
        public virtual Team Team { get; set; }
        public virtual int? TeamId { get; set; }

        [StringLength(MaxUsernameLength)]
        public string Username { get; set; }

        /// <summary>
        /// Token for push notification (i.e. Firebase)
        /// </summary>
        public string RegistrationToken { get; set; }

        public bool IsFirstLogin { get; set; } = true;

        public int? LegacyId { get; set; }
    }
}
