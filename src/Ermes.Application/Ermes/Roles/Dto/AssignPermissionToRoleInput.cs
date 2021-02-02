using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Roles.Dto
{
    public class AssignPermissionToRoleInput
    {
        [Required]
        public ErmesPermissionDto Permission { get; set; }
    }
}
