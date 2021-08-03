using Ermes.Dto.Datatable;
using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Gamification.Dto
{
    public class GetQuizzesInput: DTPagedSortedAndFilteredInputDto
    {
        public List<HazardType> Hazards { get; set; }
    }
}
