using Abp.Application.Services.Dto;
using Ermes.Persons;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Profile.Dto
{
    public class PersonDto
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public int? TeamId { get; set; }
        public string TeamName { get; set; }
        public int? OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string RegistrationToken { get; set; }
    }
}
