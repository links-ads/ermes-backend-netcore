using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.Importer.Dto
{
    public class GetLayersInput
    {
        public List<string> datatype_ids { get; set; }
        public string bbox { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
    }
}
