using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Roles.Dto
{
    public class AssignRolesToPersonInput
    {
        [Required]
        public List<string> Roles { get; set; }
        [Required]
        public long PersonId { get; set; }
        public Guid PersonGuid { get; set; }
    }
}
