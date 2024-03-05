using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Ermes.Organizations
{
    [Table("organizations")]
    public class Organization : AuditedEntity, IPassivable
    {
        public const int MAX_SHORTNAME_LENGTH = 30;
        public const int MAX_NAME_LENGTH = 100;
        public const int MAX_DESCRIPTION_LENGTH = 1024;
        public const int MAX_WEBSITE_LENGTH = 255;

        public Organization() { }
        public Organization(string name)
        {
            Name = name;
            ShortName = name.Split(' ').Select(a => a.First().ToString().ToUpper()).Aggregate((a, b) => a + b);
        }
        public Organization(string name, string shortname)
        {
            Name = name;
            ShortName = shortname;
        }

        /// <summary>
        /// Organization short name
        /// </summary>
        [Required]
        [StringLength(MAX_SHORTNAME_LENGTH)]
        public string ShortName { get; set; }

        /// <summary>
        /// Organization Name
        /// </summary>
        [Required]
        [StringLength(MAX_NAME_LENGTH)]
        public string Name { get; set; }

        /// <summary>
        /// Organization Description
        /// </summary>
        [StringLength(MAX_DESCRIPTION_LENGTH)]
        public string Description { get; set; }

        /// <summary>
        /// Organization Web site
        /// </summary>
        [StringLength(MAX_WEBSITE_LENGTH)]
        public string WebSite { get; set; }

        /// <summary>
        /// Organization Logo url
        /// </summary>
        public string LogoUrl { get; set; }

        /// <summary>
        /// List of competence areas associated to the this organization
        /// </summary>
        [ForeignKey("OrganizationId")]
        public virtual ICollection<OrganizationCompetenceArea> CompetenceAreas { get; set; }

        /// <summary>
        /// Reference to the parent organization in the hierarchy
        /// </summary>
        [ForeignKey("ParentId")]
        public virtual Organization Parent { get; set; }
        public int? ParentId { get; set; }

        public bool MembersHaveTaxCode { get; set; }

        /// <summary>
        /// List of children 
        /// </summary>
        public virtual ICollection<Organization> Children { get; set; }

        [NotMapped]
        public bool HasChildren { get { return Children != null && Children.Count > 0; } }

        public bool IsActive { get; set; } = true;
    }
}
