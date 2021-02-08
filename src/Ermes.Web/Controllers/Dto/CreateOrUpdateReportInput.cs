using Ermes.Reports.Dto;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Ermes.Web.Controllers.Dto
{
    public class CreateOrUpdateReportInput
    {
        [Required]
        public ReportDto Report { get; set; }
        public IFormFileCollection Files { get; set; }
    }
}
