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
using Ermes.Organizations;
using Microsoft.Extensions.Options;
using Ermes.Configuration;
using Ermes.ExternalServices.Csi;
using Abp.Domain.Uow;

namespace Ermes.Actions
{
    [ErmesAuthorize]
    public class ActionsAppService : ErmesAppServiceBase, IActionsAppService
    {
        private readonly ErmesAppSession _session;
        private readonly PersonManager _personManager;
        private readonly ActivityManager _activityManager;
        private readonly OrganizationManager _organizationManager;
        private readonly IGeoJsonBulkRepository _geoJsonBulkRepository;
        private readonly ILanguageManager _languageManager;
        private readonly ErmesPermissionChecker _permissionChecker;
        private readonly IOptions<ErmesSettings> _ermesSettings;
        private readonly CsiManager _csiManager;

        public ActionsAppService(
            PersonManager personManager,
            ErmesAppSession session,
            ActivityManager activityManager,
            IGeoJsonBulkRepository geoJsonBulkRepository,
            OrganizationManager organizationManager,
            ErmesPermissionChecker permissionChecker,
            IOptions<ErmesSettings> ermesSettings,
            CsiManager csiManager,
            ILanguageManager languageManager)
        {
            _personManager = personManager;
            _session = session;
            _activityManager = activityManager;
            _geoJsonBulkRepository = geoJsonBulkRepository;
            _languageManager = languageManager;
            _permissionChecker = permissionChecker;
            _organizationManager = organizationManager;
            _ermesSettings = ermesSettings;
            _csiManager = csiManager;
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

            int[] orgIdList;
            var hasPermission = _permissionChecker.IsGranted(_session.Roles, AppPermissions.Actions.Action_CanSeeCrossOrganization);
            if (hasPermission)
                orgIdList = await _organizationManager.GetOrganizationIdsAsync();
            else
                orgIdList = _session.LoggedUserPerson.OrganizationId.HasValue ? new int[] { _session.LoggedUserPerson.OrganizationId.Value } : null;


            var items = _geoJsonBulkRepository.GetPersonActions(start, end, orgIdList, input.StatusTypes, actIds, boundingBox, search, _languageManager.CurrentLanguage.Name, orgIdList == null ? CommunicationScopeType.Citizens : CommunicationScopeType.Wide);

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
                There are four types of actions a person can perform:
                    - PersonActionSharingPosition: allows a person to share his live position with the control room;
                    - PersonActionTracking: useful to track person position together with biometrical parameters (heart-rate, etc..).
                        Tracking is enabled only when the person is in Active/Movement status
                    - PersonActionStatus: allows a person to edit his current status. Possible values are: 
                            1) Off: the person is not at work
                            2) Moving: the person is moving toward a certain destination
                            3) Active: the person is working on a certain activity
                    - PersonActionActivity: allows a person to specify the activity he's performing
                Input: the action that has to be created
                Output: the actions that has been created
            "
        )]
        public virtual async Task<CreatePersonActionOutput> CreatePersonAction(CreatePersonActionInput input)
        {
            var res = new CreatePersonActionOutput();
            long personId = _session.UserId.Value;
            var lastAction = await _personManager.GetLastPersonActionAsync(personId);
            var old_status = lastAction?.CurrentStatus;
            bool mustCreateIntervention = _ermesSettings.Value != null && _ermesSettings.Value.ErmesProject == AppConsts.Ermes_Faster && _session.LoggedUserPerson.OrganizationId.HasValue;

            switch (input.PersonAction.Type)
                //Use last valid location if new input location is null or is equals to (0,0)
            {
                //do not track if user is in Off status or new position is not valid
                //lastAction == null means status == Off, so do not track
                case PersonActionType.PersonActionSharingPosition:
                    if (
                         input.PersonAction.Latitude.HasValue &&
                         input.PersonAction.Longitude.HasValue &&
                         lastAction != null &&
                         input.PersonAction.Timestamp > lastAction.Timestamp &&
                         lastAction.CurrentStatus != ActionStatusType.Off
                    )
                    {
                        var p_sharPosition = ObjectMapper.Map<PersonActionSharingPosition>(input.PersonAction);
                        p_sharPosition.PersonId = personId;
                        p_sharPosition.CurrentExtensionData = lastAction?.CurrentExtensionData;
                        p_sharPosition.CurrentStatus = lastAction != null ? lastAction.CurrentStatus : ActionStatusType.Off;
                        p_sharPosition.CurrentActivityId = lastAction?.CurrentActivityId;
                        if ((p_sharPosition.Location == null || (p_sharPosition.Location.X == 0 && p_sharPosition.Location.Y == 0)) && lastAction != null)
                            p_sharPosition.Location = lastAction.Location;
                        p_sharPosition.Id = await _personManager.InsertPersonActionSharingPositionAsync(p_sharPosition);
                        res.PersonAction = ObjectMapper.Map<PersonActionDto>(p_sharPosition);
                    }
                    else
                        res.PersonAction = new PersonActionDto();
                    break;
                case PersonActionType.PersonActionTracking:
                    var p_tracking = ObjectMapper.Map<PersonActionTracking>(input.PersonAction);
                    p_tracking.PersonId = personId;
                    p_tracking.CurrentStatus = lastAction != null ? lastAction.CurrentStatus : ActionStatusType.Off;
                    p_tracking.CurrentActivityId = lastAction?.CurrentActivityId;
                    if ((p_tracking.Location == null || (p_tracking.Location.X == 0 && p_tracking.Location.Y == 0)) && lastAction != null)
                        p_tracking.Location = lastAction.Location;
                    p_tracking.Id = await _personManager.InsertPersonActionTrackingAsync(p_tracking);
                    res.PersonAction = ObjectMapper.Map<PersonActionDto>(p_tracking);
                    break;
                case PersonActionType.PersonActionStatus:
                    var p_status = ObjectMapper.Map<PersonActionStatus>(input.PersonAction);
                    p_status.PersonId = personId;
                    //Status can Be Moving or Off, so I need to cancel ActivityId value
                    p_status.CurrentActivityId = null;
                    p_status.CurrentExtensionData = lastAction?.CurrentExtensionData;
                    if ((p_status.Location == null || (p_status.Location.X == 0 && p_status.Location.Y == 0)) && lastAction != null)
                        p_status.Location = lastAction.Location;
                    p_status.Id = await _personManager.InsertPersonActionStatusAsync(ObjectMapper.Map<PersonActionStatus>(p_status));
                   
                    ////////////////
                    ///Section dedicated to CSI service integration, see issue #65 for details.
                    ///It is not necessary to close an intervention for a first responder already in off Status
                    ///The activity name is a mandatory field for this service, even if it has no meaning; a default value of "Sorveglianza" is being sent
                    if (mustCreateIntervention && p_status.CurrentStatus == ActionStatusType.Off && old_status != null && old_status.Value != ActionStatusType.Off)
                        await CreateInterventionAsync(personId, p_status.Location?.Y, p_status.Location?.X, p_status.Timestamp, ActionStatusType.Off);
                    ///////////////
                    res.PersonAction = ObjectMapper.Map<PersonActionDto>(p_status);
                    break;
                case PersonActionType.PersonActionActivity:
                    var activity = await _activityManager.getActivityTranslationByCoreIdAndLangAsync(input.PersonAction.ActivityId, _languageManager.CurrentLanguage.Name);
                    if (activity == null)
                        throw new UserFriendlyException(400, L("InvalidActivityId"));
                    var p_activity = ObjectMapper.Map<PersonActionActivity>(input.PersonAction);
                    p_activity.PersonId = personId;
                    p_activity.CurrentStatus = ActionStatusType.Active;
                    p_activity.CurrentExtensionData = lastAction?.CurrentExtensionData;
                    if ((p_activity.Location == null || (p_activity.Location.X == 0 && p_activity.Location.Y == 0)) && lastAction != null)
                        p_activity.Location = lastAction.Location;
                    p_activity.Id = await _personManager.InsertPersonActionActivityAsync(ObjectMapper.Map<PersonActionActivity>(p_activity));

                    ////////////////
                    ///Section dedicated to CSI service integration, see issue #65 for details.
                    ///It is not necessary to create a new Intervention if the first responder already is in an active status
                    if (mustCreateIntervention && (old_status == null || old_status.Value != ActionStatusType.Active))
                    {
                        var itaActivity = await _activityManager.getActivityTranslationByCoreIdAndLangAsync(input.PersonAction.ActivityId, "it");
                        await CreateInterventionAsync(personId, p_activity.Location?.Y, p_activity.Location?.X, p_activity.Timestamp, ActionStatusType.Active, itaActivity.Name);
                    }
                    ////////////////
                    res.PersonAction = ObjectMapper.Map<PersonActionDto>(p_activity);
                    res.PersonAction.ActivityName = activity.Name;
                    break;
                default:
                    throw new UserFriendlyException(L("InvalidPersonActionType"));
            }

            res.PersonAction.Username = _session.LoggedUserPerson.Username;
            res.PersonAction.OrganizationId = _session.LoggedUserPerson.OrganizationId.HasValue ? _session.LoggedUserPerson.OrganizationId.Value : 0;
            Logger.Info("Ermes: InsertPersonAction executed by person: " + personId);
            return res;
        }

        protected async Task CreateInterventionAsync(long personId, double? latitude, double? longitude, DateTime timestamp, ActionStatusType status, string activityName = "Sorveglianza")
        {
            var person = await _personManager.GetPersonByIdAsync(personId);
            /*
             * Some persons inside Protezione Civile Piemonte do not have an associated legacyId
             * but they can operate on the field. In this case it is not necessary to open/close an intervention
             */ 
            if (!person.OrganizationId.HasValue || !person.LegacyId.HasValue)
                return;

            //It is not necessary to create an intervention when first responder goes back to Active status without passing through Off status
            //Example: Active -> Moving -> Active -> Off
            //This must create only one Intervention
            if (person.CurrentOperationLegacyId.HasValue && status == ActionStatusType.Active)
                return;

            var refOrg = await _organizationManager.GetOrganizationByIdAsync(person.OrganizationId.Value);
            var housePartner = await SettingManager.GetSettingValueAsync(AppSettings.General.HouseOrganization);
            if (refOrg.Name == housePartner || (refOrg.ParentId.HasValue && refOrg.Parent.Name == housePartner))
            {
                var operationLegacyId = await _csiManager.InsertInterventiVolontariAsync(personId, person.LegacyId.Value, latitude, longitude, activityName, timestamp, status == ActionStatusType.Off ? AppConsts.CSI_OFFLINE : AppConsts.CSI_ACTIVITY, person.CurrentOperationLegacyId);
                if (operationLegacyId > 0)
                {
                    if (status == ActionStatusType.Off)
                        person.CurrentOperationLegacyId = null;
                    else if (status == ActionStatusType.Active)
                        person.CurrentOperationLegacyId = operationLegacyId;
                }
                else
                {
                    Logger.ErrorFormat("####CreateInterventionAsync failed for personId:{0} at Timestamp {1}", personId, timestamp);
                }
            }
        }
    }
}
