using System.Collections.Generic;

namespace Ermes.Persons.Dto
{
    public class GetMyActivitiesOutput
    {
        public IList<PersonActivityDto> PersonActivities { get; set; }

        public GetMyActivitiesOutput()
        {
            PersonActivities = new List<PersonActivityDto>();
        }
    }
}
