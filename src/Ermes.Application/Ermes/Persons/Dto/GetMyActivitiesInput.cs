using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Persons.Dto
{
    public class GetMyActivitiesInput
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
