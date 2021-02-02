using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ermes.Dto
{
    public class IdInput<T>
    {
        [Required]
        public T Id { get; set; }
    }
}
