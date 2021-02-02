using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Roles.Dto
{
    public class CreateOrUpdateRoleInput
    {
        [Required]
        public RoleDto Role { get; set; }
    }
}
