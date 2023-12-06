using System.ComponentModel.DataAnnotations;

namespace Ermes.Actions.Dto
{
    public class CreatePersonActionInput
    {
        [Required]
        public PersonActionDto PersonAction { get; set; }
    }
}
