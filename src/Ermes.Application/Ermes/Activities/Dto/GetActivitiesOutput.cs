using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Activities.Dto
{
    public class GetActivitiesOutput
    {
        public IList<ActivityDto> Activities { get; set; }
        public GetActivitiesOutput()
        {
            Activities = new List<ActivityDto>();
        }
    }
}
