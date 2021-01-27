using Abp.Domain.Entities;
using Ermes.CompetenceAreas;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Organizations
{
    [Table("organization_competence_areas")]
    public class OrganizationCompetenceArea : Entity
    {
        public virtual Organization Organization { get; set; }
        [Required]
        public virtual int OrganizationId { get; set; }

        [ForeignKey("CompetenceAreaId")]
        public virtual CompetenceArea CompetenceArea { get; set; }
        [Required]
        public virtual int CompetenceAreaId { get; set; }
    }
}
