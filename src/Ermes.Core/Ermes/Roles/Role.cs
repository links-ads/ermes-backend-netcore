using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Roles
{
    [Table("roles")]
    public class Role : AuditedEntity
    {
        public const int MaxDescriptionLength = 255;
        public const int MaxNameLength = 50;
        public Role(){}

        public Role(string name, string description, bool isDefault = false, bool superRole = false)
        {
            Name = name;
            Description = description;
            Default = isDefault;
            SuperRole = superRole;
        }

        /// <summary>
        /// Role name
        /// </summary>
        [StringLength(MaxNameLength)]
        public string Name { get; set; }
        /// <summary>
        /// Role description
        /// </summary>
        [StringLength(MaxDescriptionLength)]
        public string Description { get; set; }
        /// <summary>
        /// A role is marked as default it will be assigned during registration when no roles 
        /// are explicitly provided; more than one role may be marked as default
        /// </summary>
        public bool Default { get; set; }
        /// <summary>
        /// An optional marker indicating this role supersedes all other roles, 
        /// this designation is only enforced in the UI to indicate no other roles need to be selected
        /// </summary>
        public bool SuperRole { get; set; }
    }
}
