using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Organizations.Dto
{
    public class CreateOrUpdateOrganizationInput
    {
        [Required]
        public OrganizationDto Organization { get; set; }
    }
}
