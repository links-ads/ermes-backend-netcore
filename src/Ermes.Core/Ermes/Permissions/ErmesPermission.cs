using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Ermes.Roles;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ermes.Permissions
{
    [Table("permissions")]
    public class ErmesPermission : CreationAuditedEntity
    {
        public ErmesPermission(){}
        public ErmesPermission(string name, int roleId)
        {
            Name = name;
            RoleId = roleId;
        }
        public string Name { get; set; }
        /// <summary>
        /// Reference to role
        /// </summary>
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
        public virtual int RoleId { get; set; }
    }
}
