using Ermes.Auth.Dto;
using io.fusionauth.domain;
using System.ComponentModel.DataAnnotations;

namespace Ermes.Profile.Dto
{
    public class UpdateProfileInput
    {
        //public bool SkipVerification { get; set; } = true;
        //public bool SkipRegistrationVerification { get; set; } = true;
        //public bool SendSetPasswordEmail { get; set; } = false;
        [Required]
        public UserDto User { get; set; }
        public int? OrganizationId { get; set; }
        public int? TeamId { get; set; }
        public long? PersonId { get; set; }
        public bool IsFirstLogin { get; set; }
        public string TaxCode { get; set; }
    }
}
