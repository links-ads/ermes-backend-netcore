using Abp.Runtime.Validation;
using Ermes.Actions.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ermes.Web.Controllers.Dto
{
    public class CreatePersonActionForExternalsInput : ICustomValidate
    {
        [Required]
        public PersonActionDto PersonAction { get; set; }
        public int VolterId { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (VolterId == 0)
            {
                context.Results.Add(new ValidationResult("Invalid VolterId"));
            }
        }
    }
}
