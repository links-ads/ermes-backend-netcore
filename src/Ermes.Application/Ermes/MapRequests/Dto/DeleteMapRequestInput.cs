using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.MapRequests.Dto
{
    public class DeleteMapRequestInput
    {
        [Required]
        public List<string> MapRequestCodes { get; set; }

    }
}
