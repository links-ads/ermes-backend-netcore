using Abp.Runtime.Validation;
using Ermes.Actions.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ermes.Web.Controllers.Dto
{
    public class CreatePersonActionForExternalsInput : ExternalBase
    {
        [Required]
        public PersonActionDto PersonAction { get; set; }
    }
}
