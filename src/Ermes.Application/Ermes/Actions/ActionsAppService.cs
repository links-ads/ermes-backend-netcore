using Abp.UI;
using Ermes.Actions.Dto;
using Ermes.Activities;
using Ermes.Attributes;
using Ermes.Authorization;
using Ermes.Enums;
using Ermes.Helpers;
using Ermes.Persons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSwag.Annotations;
using NetTopologySuite.Geometries;
using Ermes.GeoJson;
using Abp.Localization;
using Newtonsoft.Json;
using Ermes.Dto.Datatable;

namespace Ermes.Actions
{
    [ErmesAuthorize]
    public class ActionsAppService : ErmesAppServiceBase, IActionsAppService
    {
        private readonly ErmesAppSession _session;
        private readonly PersonManager _personManager;
        private readonly ActivityManager _activityManager;
        private readonly ErmesPermissionChecker _permissionChecker;
        private readonly IGeoJsonBulkRepository _geoJsonBulkRepository;
        private readonly ILanguageManager _languageManager;

        public ActionsAppService(
            PersonManager personManager, 
            ErmesAppSession session, 
            ActivityManager activityManager, 
            ErmesPermissionChecker permissionChecker,
            IGeoJsonBulkRepository geoJsonBulkRepository,
            ILanguageManager languageManager)
        {
            _personManager = personManager;
            _session = session;
            _activityManager = activityManager;
            _permissionChecker = permissionChecker;
            _geoJsonBulkRepository = geoJsonBulkRepository;
            _languageManager = languageManager;
        }
        [OpenApiOperation("Get Actions",
            @"
                This is a server-side paginated API
                Input: use the following properties to filter result list:
                    - Draw: Draw counter. This is used by DataTables to ensure that the Ajax returns from server-side processing requests are drawn in sequence by 
                        DataTables (Ajax requests are asynchronous and thus can return out of sequence)
                    - MaxResultCount: number of records that the table can display in the current draw (must be >= 0)
                    - SkipCount: paging first record indicator. This is the start point in the current data set (0 index based - i.e. 0 is the first record)
                    - Search: 
                               - value: global search value
                               - regex: true if the global filter should be treated as a regular expression for advanced searching, false otherwise
                    - Order (is a list, for multi-column sorting):
                                - column: name of the column to which sorting should be applied
                                - dir: sorting direction
                In addition to pagination parameters, there are additional properties for action filtering:
                    - StartDate and EndDate to define a time window of interest
                    - StatusTypes to select only person in a certain status (Off, Moving, Active)
                    - ActivityIds to select active people who are performing a specifi task (SV, CO, LG)
                    - SouthWestBoundary: bottom-left corner of the bounding box for a spatial query. (optional) (to be filled together with NorthEast property)
                    - NorthEastBoundary: top-right corner of the bounding box for a spatial query format. (optional) (to be filled together with SouthWest property)
                Output: list of PersonActionDto elements

                N.B.: A person has visibility only on actions belonging to his organization
            "
        )]
        public virtual async Task<DTResult<PersonActionDto>> GetActions(GetActionsInput input)
        {
            var start = input.StartDate ?? DateTime.MinValue;
            var end = input.EndDate ?? DateTime.MaxValue;
            var actIds = input.ActivityIds?.ToArray();
            var search = input.Search?.Value;
            Geometry boundingBox = null;
            if (input.NorthEastBoundary != null && input.SouthWestBoundary != null)
                boundingBox = GeometryHelper.GetPolygonFromBoundaries(input.SouthWestBoundary, input.NorthEastBoundary);

            var items = _geoJsonBulkRepository.GetPersonActions(start, end, _session.LoggedUserPerson.OrganizationId.HasValue ? new int[] { _session.LoggedUserPerson.OrganizationId.Value } : null, input.StatusTypes, actIds, boundingBox, search, _languageManager.CurrentLanguage.Name);

            var actions = JsonConvert.DeserializeObject<GetActionsOutput>(items);

            List<PersonActionDto> data = new List<PersonActionDto>();
            if (actions.PersonActions != null)
                data = actions.PersonActions.Skip(input.MaxResultCount * input.SkipCount).Take(input.MaxResultCount).ToList();
            else
                actions.PersonActions = new List<PersonActionDto>();

            return new DTResult<PersonActionDto>(input.Draw, actions.PersonActions.Count, data.Count, data);
        }

        [OpenApiOperation("Create Person Action",
            @"
                There are three types of actions a person can perform:
                    - PersonActionTracking: useful to track person position together with biometrical parameters (heart-rate, etc..).
                        Tracking is enabled only when the person is in Active/Movement status
                    - PersonActionStatus: allows a person to edit his current status. Possible values are: 
                            1) Off: the person is not at work
                            2) Moving: the person is moving toward a certain destination
                            3) Active: the person is working on a certain activity
                Input: the action that has to be created
                Output: the actions that has been created
            "
        )]
        public virtual async Task<CreatePersonActionOutput> CreatePersonAction(CreatePersonActionInput input)
        {
            var res = new CreatePersonActionOutput();
            long personId = _session.UserId.Value;
            
            switch (input.PersonAction.Type)
            {
                case PersonActionType.PersonActionTracking:
                    var item = ObjectMapper.Map<PersonActionTracking>(input.PersonAction);
                    item.PersonId = personId;
                    var lastAction = await _personManager.GetLastPersonActionAsync(personId);
                    item.CurrentStatus = lastAction != null ? lastAction.CurrentStatus : ActionStatusType.Off;
                    await _personManager.InsertPersonActionTrackingAsync(item);
                    res.PersonAction = ObjectMapper.Map<PersonActionDto>(item);
                    res.PersonAction.Type = PersonActionType.PersonActionTracking;
                    break;
                case PersonActionType.PersonActionStatus:
                    var item2 = ObjectMapper.Map<PersonActionStatus>(input.PersonAction);
                    item2.PersonId = personId;
                    item2.CurrentStatus = input.PersonAction.Status;
                    await _personManager.InsertPersonActionStatusAsync(ObjectMapper.Map<PersonActionStatus>(item2));
                    res.PersonAction = ObjectMapper.Map<PersonActionDto>(item2);
                    res.PersonAction.Type = PersonActionType.PersonActionStatus;
                    break;
                case PersonActionType.PersonActionActivity:
                    if (!(await _activityManager.CheckIfActivityExists(input.PersonAction.ActivityId)))
                        throw new UserFriendlyException(400, L("InvalidActivityId"));
                    var item3 = ObjectMapper.Map<PersonActionActivity>(input.PersonAction);
                    item3.PersonId = personId;
                    item3.CurrentStatus = ActionStatusType.Active;
                    await _personManager.InsertPersonActionActivityAsync(ObjectMapper.Map<PersonActionActivity>(item3));
                    res.PersonAction = ObjectMapper.Map<PersonActionDto>(item3);
                    res.PersonAction.Type = PersonActionType.PersonActionActivity;
                    break;
                //update the position of the last action of the current logged user
                //do not track if user is in Off status
                case PersonActionType.PersonActionSharingPosition:
                    var lastAction2 = await _personManager.GetLastPersonActionAsync(personId);
                    if (lastAction2 != null &&
                         input.PersonAction.Latitude.HasValue &&
                         input.PersonAction.Longitude.HasValue &&
                        input.PersonAction.Timestamp > lastAction2.Timestamp &&
                        lastAction2.CurrentStatus != ActionStatusType.Off
                    )
                    {
                        lastAction2.Location = new Point(input.PersonAction.Longitude.Value, input.PersonAction.Latitude.Value);
                        res.PersonAction = new PersonActionDto()
                        {
                            Type = PersonActionType.PersonActionSharingPosition,
                            Latitude = input.PersonAction.Latitude,
                            Longitude = input.PersonAction.Longitude
                        };
                    }
                    else
                        res.PersonAction = new PersonActionDto();

                    break;
                default:
                    throw new UserFriendlyException(L("InvalidPersonActionType"));
            }
            Logger.Info("Ermes: InsertPersonAction executed by person: " + personId);
            return res;
        }
    }
}
