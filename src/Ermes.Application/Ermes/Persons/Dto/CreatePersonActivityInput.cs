using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Persons.Dto
{
    public class CreatePersonActivityInput
    {
        [Required]
        public PersonActivityDto PersonActivity { get; set; }
    }
}
