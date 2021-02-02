using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Actions.Dto
{
    public class CreatePersonActionInput
    {
        [Required]
        public PersonActionDto PersonAction { get; set; }
    }
}
