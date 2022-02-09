using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ermes.Layers.Dto
{
    public class LayerDetailsDto
    {
        public string Name { get; set; }
        public List<DateTime> Timestamps { get; set; }
        public DateTime Created_At { get; set; }
        public string Request_Code { get; set; }
        public string MapRequestCode { get
            {
                return Request_Code?.Split('.').LastOrDefault();
            }
        }
        public string Creator
        {
            get
            {
                return Request_Code?.Split('.').FirstOrDefault();
            }
        }
    }
}
