using Abp.Extensions;
using Abp.ObjectMapping;
using Abp.UI;
using Ermes.Attributes;
using Ermes.Auth.Dto;
using Ermes.Enums;
using Ermes.Helpers;
using Ermes.Linq.Extensions;
using Ermes.Organizations;
using Ermes.Persons.Dto;
using FusionAuthNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ermes.Persons
{
    [ErmesAuthorize]
    public class PersonsAppService : ErmesAppServiceBase, IPersonsAppService
    {
        private readonly ErmesAppSession _session;
        private readonly IObjectMapper _objectMapper;
        private readonly PersonManager _personManager;
        private readonly IOptions<FusionAuthSettings> _fusionAuthSettings;
        public PersonsAppService(
                    ErmesAppSession session, 
                    IObjectMapper objectMapper,
                    PersonManager personManger,
                    IOptions<FusionAuthSettings> fusionAuthSettings
            )
        {
            _session = session;
            _objectMapper = objectMapper;
            _personManager = personManger;
            _fusionAuthSettings = fusionAuthSettings;
        }

        #region Person

        //public virtual async  Task<GetMyProfileOutput> GetMyProfile()
        //{
        //    var client = FusionAuth.GetFusionAuthClient(_fusionAuthSettings.Value);
        //    var res = new GetMyProfileOutput();
        //    if (_session.FusionAuthUserId.HasValue)
        //    {
        //        var response = await client.RetrieveUserAsync(_session.FusionAuthUserId);
        //        if (response.WasSuccessful())
        //        {
        //            var person = await _personManager.GetPersonByUserIdAsync(_session.FusionAuthUserId.Value);
        //            res.User = _objectMapper.Map<UserDto>(response.successResponse.user);
        //            res.User.OrganizationName = person.Organization?.Name;
        //            var lastAction = await _personManager.GetLastPersonActionAsync(person.Id);
        //            if (lastAction != null)
        //            {
        //                res.User.CurrentStatus = lastAction.CurrentStatus;
        //                res.User.CurrentActivityId = lastAction.As<PersonActionActivity>().ActivityId;
        //            }
        //            else
        //                res.User.CurrentStatus = ActionStatusType.Off;
                    
        //            res.User.Roles = _session.Roles;
        //        }
        //        else
        //        {
        //            (var errorCode, var message) = FusionAuth.ManageErrorResponse(response);
        //            throw new UserFriendlyException(errorCode, message);
        //        }
        //    }

            

        //    return res;
        //}
        #endregion

        #region PersonActivities
        //public virtual async Task<CreatePersonActivityOutput> CreatePersonActivity(CreatePersonActivityInput input)
        //{
        //    var person = await ErmesCommon.GetPersonOfCurrentLoggedUser(_session.FusionAuthUserId, _personManager);

        //    var currentPerAct = _personManager.GetCurrentPersonActivity(person.Id);
        //    var newPersonActivity = _objectMapper.Map<PersonActivity>(input.PersonActivity);

        //    if (currentPerAct == null)// new group of activity
        //        newPersonActivity.WorkSessionId = _personManager.ComputeNewWorkingSessionId();
        //    else //add this new activity to the session work
        //    {
        //        newPersonActivity.WorkSessionId = currentPerAct.WorkSessionId;
        //        currentPerAct.EndDate = newPersonActivity.StartDate.AddSeconds(-1);
        //    }

        //    newPersonActivity.OrganizationId = person.OrganizationId;
        //    newPersonActivity.PersonId = person.Id;
        //    if (newPersonActivity.ActivityId == 18)
        //        newPersonActivity.EndDate = newPersonActivity.StartDate;

        //    newPersonActivity.Id = await _personManager.InsertPersonActivityAsync(newPersonActivity);
        //    var res = _objectMapper.Map<PersonActivityDto>(newPersonActivity);

        //    return new CreatePersonActivityOutput() { PersonActivity = res };
        //}

        //public virtual async Task<GetMyActivitiesOutput> GetMyActivities(GetMyActivitiesInput input)
        //public virtual async Task<GetMyActivitiesOutput> GetMyActivities(GetMyActivitiesInput input)
        //{
        //    var person = await ErmesCommon.GetPersonOfCurrentLoggedUser(_session.FusionAuthUserId, _personManager);
        //    var list = await _personManager.GetPersonActivitiesByPersonId(person.Id, input?.StartDate, input?.EndDate);

        //    return new GetMyActivitiesOutput()
        //    {
        //        PersonActivities = _objectMapper.Map<List<PersonActivityDto>>(list)
        //    };
        //}
    #endregion

    //public async Task<List<int>> GetMyColleagues()
    //    {
    //        var person = await ErmesCommon.GetPersonOfCurrentLoggedUser(_session.FusionAuthUserId, _personManager);
            
    //        return await _personManager
    //                .Persons
    //                .DataOwnership(person.OrganizationId.HasValue ? new List<int>() { person.OrganizationId.Value } : null)
    //                .Where(a => a.Id != person.Id)
    //                .Select(a => a.Id)
    //                .ToListAsync();
    //    }
    }
}
