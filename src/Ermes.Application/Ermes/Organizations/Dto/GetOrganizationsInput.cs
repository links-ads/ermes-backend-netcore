using Ermes.Dto.Datatable;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Organizations.Dto
{
    public class GetOrganizationsInput : DTPagedSortedAndFilteredInputDto
    {
        public int? ParentId { get; set; }
    }
}
