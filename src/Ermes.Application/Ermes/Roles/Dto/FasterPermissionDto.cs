using Abp.AutoMapper;
using Ermes.Permissions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Roles.Dto
{
    [AutoMap(typeof(ErmesPermission))]
    public class ErmesPermissionDto
    {
        [Required]
        public int RoleId { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
