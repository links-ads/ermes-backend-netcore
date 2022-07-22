using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Dto
{
    public class ResponseBaseDto
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
