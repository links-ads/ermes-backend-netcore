using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Dashboard.Dto
{
    public class ActivationDto
    {
        public DateTime Timestamp { get; set; }
        public string X { get
            {
                return Timestamp.ToShortDateString();
            } 
        }
        public int Y { get; set; }
        public long[] PersonIdList { get; set; }
    }
}
