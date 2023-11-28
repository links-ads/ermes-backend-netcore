using Abp.Azure;
using Abp.AzureCognitiveServices.CognitiveServices;
using Abp.BackgroundJobs;
using Abp.UI;
using Ermes.Activities.Dto;
using Ermes.Activities;
using Ermes.Attributes;
using Ermes.Authorization;
using Ermes.Categories;
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
                        ActivityManager activityManager)
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
            var roleList = new List<string>() { AppRoles.FIRST_RESPONDER };
            
            var externalOrg = await _organizationManager.GetOrganizationByNameAsync(ErmesConsts.EXTERNAL_ORGANIZATION_NAME);
            if(externalOrg == null)
                throw new UserFriendlyException("MissiongExternalOrganization");
            
            //Before report creation, I need to check if the creator is present in DB
            var creator = _personManager.GetPersonByLegacyId(input.VolterId);
            if (creator == null)
            {
                var currentUser = await CreateUserInternalAsync(input.VolterId, _fusionAuthSettings, _ermesSettings);
                var roles = await _personManager.GetRolesByName(roleList);
                creator = await CreateOrUpdatePersonInternalAsync(creator, currentUser, externalOrg.Id, null, true, true, input.VolterId, roles, _personManager);
            }

            if (input.Report.Id == 0)
            {
                res.Report = await CreateReportAsync(
                    input.Report,
                    externalOrg.Id,
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

    }
}
