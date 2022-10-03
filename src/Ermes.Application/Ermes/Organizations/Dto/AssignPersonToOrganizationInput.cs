using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Organizations.Dto
{
    public class AssignPersonToOrganizationInput
    {
        public int OrganizationId { get; set; }
        public long PersonId { get; set; }
        public Guid PersonGuid { get; set; }
    }
}
