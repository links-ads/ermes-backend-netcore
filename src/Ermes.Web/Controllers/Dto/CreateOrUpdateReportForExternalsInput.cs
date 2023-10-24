using Abp.Runtime.Validation;
using Ermes.Reports.Dto;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Ermes.Web.Controllers.Dto
{
    public class CreateOrUpdateReportForExternalsInput : ICustomValidate
    {
        [Required]
        public ReportDto Report { get; set; }
        public IFormFileCollection Files { get; set; }
        public int VolterId { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if(VolterId == 0) {
                context.Results.Add(new ValidationResult("Invalid VolterId"));
            }
        }
    }
}
