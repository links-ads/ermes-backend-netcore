using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Reports.Dto
{
    public class CategoryGroupDto
    {
        public string Group { get; set; }
        public List<CategoryDto> Categories { get; set; }
    }
}
