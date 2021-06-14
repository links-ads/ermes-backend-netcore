using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Reports.Dto
{
    public class CategoryGroupDto
    {
        public string Group { get; set; }
        public string GroupIcon { get; set; }
        //Key value for Group, not influenced by localization
        public string GroupKey { get; set; }
        public List<CategoryDto> Categories { get; set; }
    }
}
