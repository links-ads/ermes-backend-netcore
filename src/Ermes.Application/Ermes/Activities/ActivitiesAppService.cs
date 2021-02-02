using Abp.ObjectMapping;
using Ermes;
using Ermes.Activities.Dto;
using Ermes.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NSwag.Annotations;

namespace Ermes.Activities
{
    [ErmesAuthorize]
    public class ActivitiesAppService : ErmesAppServiceBase, IActivitiesAppService
    {
        private readonly ActivityManager _activityManager;
        private readonly IObjectMapper _objectMapper;
        public ActivitiesAppService(ActivityManager activityManager, IObjectMapper objectMapper)
        {
            _activityManager = activityManager;
            _objectMapper = objectMapper;
        }

        [OpenApiOperation("Get Activities",
            @"
                This api provides the list of all possible activies that a person can perform during his working session. Only leaf activity of the hierarchy are returned.
                Input: FullList. If true, all activities are returned, otherwise, only leaf activities will be included in the result list
                Output: list of ActivityDto items
            "
        )]
        public virtual async Task<GetActivitiesOutput> GetActivities(GetActivitiesInput input)
        {
            List<Activity> actList;

            if(input.FullList)
                actList = await _activityManager.GetAllAsync();
            else
                actList = await _activityManager.GetLeafActivities();
                                    
            return new GetActivitiesOutput()
            {
                Activities = _objectMapper.Map<List<ActivityDto>>(actList)
            };
        }
    }
}
