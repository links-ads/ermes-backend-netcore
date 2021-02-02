using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Organizations.Dto
{
    public class AssignOrganizationToCompetenceAreasInput
    {
        [Required]
        public int OrganizationId { get; set; }
        public List<int> CompetenceAreaIds { get; set; }
    }
}
