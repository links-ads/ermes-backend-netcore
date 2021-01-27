using Abp.Domain.Entities.Auditing;
using Ermes.CompetenceAreas;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Organizations
{
    [Table("organizations")]
    public class Organization : AuditedEntity
    {
        public const int MaxShortNameLength = 30;
        public const int MaxNameLength = 100;
        public const int MaxDescriptionLength = 1024;
        public const int MaxWebSiteLength = 255;

        public Organization(){}
        public Organization(string name, string shortname)
        {
            Name = name;
            ShortName = shortname;
        }

        /// <summary>
        /// Organization short name
        /// </summary>
        [Required]
        [StringLength(MaxShortNameLength)]
        public virtual string ShortName { get; set; }
        
        /// <summary>
        /// Organization Name
        /// </summary>
        [Required]
        [StringLength(MaxNameLength)]
        public virtual string Name { get; set; }
        
        /// <summary>
        /// Organization Description
        /// </summary>
        [StringLength(MaxDescriptionLength)]
        public virtual string Description { get; set; }
        
        /// <summary>
        /// Organization Web site
        /// </summary>
        [StringLength(MaxWebSiteLength)]
        public virtual string WebSite { get; set; }
        
        /// <summary>
        /// Organization Logo url
        /// </summary>
        public virtual string LogoUrl { get; set; }

        /// <summary>
        /// List of competence areas associated to the this organization
        /// </summary>
        [ForeignKey("OrganizationId")]
        public virtual ICollection<OrganizationCompetenceArea> CompetenceAreas { get; set; }
    }
}
