using Abp.Runtime.Validation;
using System.ComponentModel.DataAnnotations;

namespace Ermes.Web.Controllers.Dto
{
    public class ExternalBase: ICustomValidate
    {
        public int VolterId { get; set; }
        public string CreatorFullName { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (VolterId == 0)
            {
                context.Results.Add(new ValidationResult("Invalid VolterId"));
            }
        }
    }
}
