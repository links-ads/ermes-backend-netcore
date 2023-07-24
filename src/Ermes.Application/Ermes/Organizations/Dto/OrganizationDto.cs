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
        [StringLength(Organization.MAX_SHORTNAME_LENGTH)]
        public string ShortName { get; set; }
        [Required]
        [StringLength(Organization.MAX_NAME_LENGTH)]
        public string Name { get; set; }
        [StringLength(Organization.MAX_DESCRIPTION_LENGTH)]
        public string Description { get; set; }
        [StringLength(Organization.MAX_WEBSITE_LENGTH)]
        public string WebSite { get; set; }
        public string LogoUrl { get; set; }
        public int? ParentId { get; set; }
        public string ParentName { get; set; }
        public bool MembersHaveTaxCode { get; set; }
        public bool HasChildren { get; set; }
    }
}
