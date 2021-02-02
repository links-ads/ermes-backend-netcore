using Abp.Application.Services.Dto;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Persons.Dto
{
    public class PersonActivityDto
    {
        public long PersonId { get; set; }
        public int? OrganizationId { get; set; }
        public Point Location { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ActivityPlace { get; set; }
        [Required]
        public int ActivityId { get; set; }
        public int WorkSessionId { get; set; }
    }
}
