using System;
using System.Collections.Generic;
using System.Text;

namespace FusionAuthNetCore.Dto
{
    public class BaseFusionAuthErrorDto
    {
        public string Message { get; set; }
        public int ErrorCode { get; set; }
        public bool HasTranslation { get; set; } = false;
        
    }
}
