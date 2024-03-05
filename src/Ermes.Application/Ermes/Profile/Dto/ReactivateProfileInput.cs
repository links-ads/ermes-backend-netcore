using Abp.Runtime.Validation;
using System.ComponentModel.DataAnnotations;

namespace Ermes.Profile.Dto
{
    public class ReactivateProfileInput : ICustomValidate
    {
        [Required]
        public string Email { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if(Email == null || Email == string.Empty)
                context.Results.Add(new ValidationResult("Email address required"));
        }
    }
}
