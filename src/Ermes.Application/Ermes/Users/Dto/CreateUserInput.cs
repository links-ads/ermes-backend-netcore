using Ermes.Auth.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Users.Dto
{
    public class CreateUserInput
    {
        public bool SkipVerification { get; set; }
        public bool SkipRegistrationVerification { get; set; }
        public bool SendSetPasswordEmail { get; set; }
        [Required]
        public UserDto User { get; set; }
        public int OrganizationId { get; set; }
        public long PersonId { get; set; }
    }
}
