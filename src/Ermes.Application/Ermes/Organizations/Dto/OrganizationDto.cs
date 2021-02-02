using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Organizations.Dto
{
    public class OrganizationDto
    {
        public int Id { get; set; }
        [Required]
        public string ShortName { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string WebSite { get; set; }
        public string LogoUrl { get; set; }
    }
}
