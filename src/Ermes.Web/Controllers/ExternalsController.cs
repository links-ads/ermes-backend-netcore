using Abp.Azure;
using Abp.AzureCognitiveServices.CognitiveServices;
using Abp.BackgroundJobs;
using Abp.UI;
using Ermes.Actions.Dto;
using Ermes.Activities;
using Ermes.Activities.Dto;
using Ermes.Attributes;
using Ermes.Authorization;
using Ermes.Categories;
using Ermes.Configuration;
using Ermes.Enums;
using Ermes.ExternalServices.Csi;
using Ermes.Interfaces;
using Ermes.Missions;
using Ermes.Organizations;
using Ermes.Persons;
using Ermes.Reports;
using Ermes.Web.Controllers.Dto;
using FusionAuthNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ermes.Web.Controllers
{
    [ErmesApiKey]
    [ErmesIgnoreApi(true)]
    public class ExternalsController : ErmesControllerBase, IBackofficeApi
    {
        private readonly ReportManager _reportManager;
        private readonly MissionManager _missionManager;
        private readonly CategoryManager _categoryManager;
        private readonly PersonManager _personManager;
        private readonly OrganizationManager _organizationManager;
        private readonly IAzureManager _azureManager;
        private readonly ICognitiveServicesManager _cognitiveServicesManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IOptions<ErmesSettings> _ermesSettings;
        private readonly IOptions<FusionAuthSettings> _fusionAuthSettings;
        private readonly ActivityManager _activityManager;
        private readonly CsiManager _csiManager;

        public ExternalsController(
                        ReportManager reportManager,
                        MissionManager missionManager,
                        CategoryManager categoryManager,
                        PersonManager personManager,
                        OrganizationManager organizationManager,
                        IHttpContextAccessor httpContextAccessor,
                        IAzureManager azureManager,
                        ICognitiveServicesManager cognitiveServicesManager,
                        IOptions<ErmesSettings> ermesSettings,
                        IOptions<FusionAuthSettings> fusionAuthSettings,
                        IBackgroundJobManager backgroundJobManager,
                        ActivityManager activityManager,
                        CsiManager csiManager
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _reportManager = reportManager;
            _missionManager = missionManager;
            _categoryManager = categoryManager;
            _personManager = personManager;
            _organizationManager = organizationManager;
            _azureManager = azureManager;
            _backgroundJobManager = backgroundJobManager;
            _cognitiveServicesManager = cognitiveServicesManager;
            _ermesSettings = ermesSettings;
            _fusionAuthSettings = fusionAuthSettings;
            _activityManager = activityManager;
            _csiManager = csiManager;
        }

        [Route("api/services/app/Externals/CreateOrUpdateReport")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        [OpenApiOperation("Create or Update a Report",
            @"
                The API accepts as input a multipart/form-data object
                Input: a ReportDto and the list of media associated to the report. Media types accepted are audio, image and video
                If the input contains an Id > 0, an update is performed, otherwise a new report is created
                Output: the report that has been created/updated
                This operation will trigger notifications
            "
        )]
        public virtual async Task<CreateOrUpdateReportForExternalsOutput> CreateOrUpdateReportForExternals([FromForm] CreateOrUpdateReportForExternalsInput input)
        {
            var context = _httpContextAccessor.HttpContext;
            var request = context.Request;

            if (request.ContentLength == 0)
                throw new UserFriendlyException(L("InvalidFile"));

            var media = request.Form.Files;

            var res = new CreateOrUpdateReportForExternalsOutput();
            var (creator, organizationId, roleList) = await GetExternalPersonAsync(input.VolterId, input.CreatorFullName);

            if (input.Report.Id == 0)
            {
                res.Report = await CreateReportAsync(
                    input.Report,
                    organizationId,
                    roleList.ToArray(),
                    creator.Id,
                    _reportManager,
                    _missionManager,
                    _categoryManager,
                    _organizationManager,
                    _personManager,
                    null,
                    _azureManager,
                    _cognitiveServicesManager,
                    _backgroundJobManager,
                    _ermesSettings,
                    media
                );
            }
            else
                throw new UserFriendlyException(L("InvalidEntityId", "Report", input.Report.Id));

            return res;
        }

        [Route("api/services/app/Externals/GetActivities")]
        [OpenApiOperation("Get Activities",
            @"
                This api provides the list of all possible activies that a person can perform during his working session. Only leaf activity of the hierarchy are returned.
                Input: FullList. If true, all activities are returned, otherwise, only leaf activities will be included in the result list
                Output: list of ActivityDto items
            "
        )]
        public virtual async Task<GetActivitiesOutput> GetActivities(GetActivitiesInput input)
        {
            List<Activity> actList = input.FullList ? await _activityManager.GetAllAsync() : await _activityManager.GetLeafActivities();

            return new GetActivitiesOutput()
            {
                Activities = ObjectMapper.Map<List<ActivityDto>>(actList)
            };
        }


        /// Duplication of code contained in ActionsAppService, optimization required
        [Route("api/services/app/Externals/CreatePersonAction")]
        [HttpPost]
        [SwaggerResponse(typeof(CreatePersonActionOutput))]
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
        public virtual async Task<CreatePersonActionOutput> CreatePersonAction([FromBody] CreatePersonActionForExternalsInput input)
        {
            var res = new CreatePersonActionOutput();

            var (creator, organizationId, roleList) = await GetExternalPersonAsync(input.VolterId, input.CreatorFullName);

            long personId = creator.Id;
            var lastAction = await _personManager.GetLastPersonActionAsync(personId);
            var old_status = lastAction?.CurrentStatus;
            bool mustCreateIntervention = _ermesSettings.Value != null && _ermesSettings.Value.ErmesProject == AppConsts.Ermes_Faster && creator.OrganizationId.HasValue;

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
                    var activity = await _activityManager.getActivityTranslationByCoreIdAndLangAsync(input.PersonAction.ActivityId, "it");
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

            res.PersonAction.Username = creator.Username;
            res.PersonAction.Email = creator.Email;
            res.PersonAction.OrganizationId = creator.OrganizationId.HasValue ? creator.OrganizationId.Value : 0;
            Logger.Info("Ermes: InsertPersonAction executed by person: " + personId);
            return res;
        }

        private async Task<Tuple<Person, int, List<string>>> GetExternalPersonAsync(int volterId, string creatorFullName)
        {
            var roleList = new List<string>() { AppRoles.FIRST_RESPONDER };

            var externalOrg = await _organizationManager.GetOrganizationByNameAsync(ErmesConsts.EXTERNAL_ORGANIZATION_NAME);
            if (externalOrg == null)
                throw new UserFriendlyException("MissiongExternalOrganization");
            //Before report creation, I need to check if the creator is present in DB
            var creator = _personManager.GetPersonByLegacyId(volterId);
            if (creator == null)
            {
                var currentUser = await CreateUserInternalAsync(volterId, creatorFullName, _fusionAuthSettings, _ermesSettings);
                var roles = await _personManager.GetRolesByName(roleList);
                creator = await CreateOrUpdatePersonInternalAsync(creator, currentUser, externalOrg.Id, null, true, true, volterId, roles, _personManager);
            }

            return new Tuple<Person, int, List<string>>(creator, externalOrg.Id, roleList);
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
