using Abp.Azure;
using Abp.AzureCognitiveServices.CognitiveServices;
using Abp.BackgroundJobs;
using Abp.Extensions;
using Abp.UI;
using Ermes.Attributes;
using Ermes.Authorization;
using Ermes.Categories;
using Ermes.Enums;
using Ermes.EventHandlers;
using Ermes.Gamification;
using Ermes.Missions;
using Ermes.Organizations;
using Ermes.Persons;
using Ermes.Reports;
using Ermes.Reports.Dto;
using Ermes.Resources;
using Ermes.Web.Controllers.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ermes.Web.Controllers
{
    [ErmesAuthorize]
    [ErmesIgnoreApi(true)]
    //removed from swagger, because at the moment, openapi generator fails while generating this API service
    //see this issue: https://github.com/OpenAPITools/openapi-generator/issues/2610
    //when marked to solved, probably we can remove the IgnoreApi tag
    public class ReportsController : ErmesControllerBase
    {
        private readonly CategoryManager _categoryManager;
        private readonly ReportManager _reportManager;
        private readonly ErmesAppSession _session;
        private readonly MissionManager _missionManager;
        private readonly PersonManager _personManager;
        private readonly OrganizationManager _organizationManager;
        private readonly GamificationManager _gamificationManager;
        private readonly IAzureManager _azureManager;
        private readonly ICognitiveServicesManager _cognitiveServicesManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IOptions<ErmesSettings> _ermesSettings;

        public ReportsController(
                        CategoryManager categoryManager,
                        ReportManager reportManager,
                        ErmesAppSession session,
                        MissionManager missionManager,
                        PersonManager personManager,
                        OrganizationManager organizationManager,
                        GamificationManager gamificationManager,
                        IHttpContextAccessor httpContextAccessor,
                        IAzureManager azureManager,
                        ICognitiveServicesManager cognitiveServicesManager,
                        IOptions<ErmesSettings> ermesSettings,
                        IBackgroundJobManager backgroundJobManager
                    )
        {
            _httpContextAccessor = httpContextAccessor;
            _categoryManager = categoryManager;
            _reportManager = reportManager;
            _session = session;
            _missionManager = missionManager;
            _personManager = personManager;
            _organizationManager = organizationManager;
            _gamificationManager = gamificationManager;
            _azureManager = azureManager;
            _backgroundJobManager = backgroundJobManager;
            _cognitiveServicesManager = cognitiveServicesManager;
            _ermesSettings = ermesSettings;
        }

        [Route("api/services/app/Reports/CreateOrUpdateReport")]
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
        public virtual async Task<CreateOrUpdateReportOutput> CreateOrUpdateReport([FromForm] CreateOrUpdateReportInput input)
        {
            var context = _httpContextAccessor.HttpContext;
            var request = context.Request;

            if (request.ContentLength == 0)
                throw new UserFriendlyException(L("InvalidFile"));

            var media = request.Form.Files;

            var res = new CreateOrUpdateReportOutput();

            if (input.Report.Id == 0)
                res.Report = await CreateReportAsync(
                    input.Report,
                    _session.LoggedUserPerson.OrganizationId,
                    _session.Roles,
                    _session.LoggedUserPerson.Id,
                    _reportManager,
                    _missionManager,
                    _categoryManager,
                    _organizationManager,
                    _personManager,
                    _gamificationManager,
                    _azureManager,
                    _cognitiveServicesManager,
                    _backgroundJobManager,
                    _ermesSettings,
                    media
            );
            else
                res.Report = await UpdateReportAsync(input.Report, media);

            return res;
        }

        private async Task<ReportDto> UpdateReportAsync(ReportDto reportDto, IFormFileCollection media = null)
        {

            var tuple = await CheckReportValidityAsync(reportDto, _missionManager, _categoryManager, _session.LoggedUserPerson.OrganizationId);
            if (tuple != null && !tuple.Item1.IsNullOrWhiteSpace())
                throw new UserFriendlyException(L(tuple.Item1, tuple.Item2));

            var report = await _reportManager.GetReportByIdAsync(reportDto.Id);
            if (report == null)
                throw new UserFriendlyException(L("InvalidReportId", reportDto.Id));

            var _azureStorageManager = _azureManager.GetStorageManager(ResourceManager.GetBasePath(ResourceManager.Reports.ContainerName));

            //In MediaURIs I have to check if something has been deleted
            List<string> toBeDeleted;
            if (reportDto.MediaURIs == null || reportDto.MediaURIs.Count == 0)
            {
                toBeDeleted = report.MediaURIs ?? new List<string>();
                report.MediaURIs = null;
            }
            else
            {
                List<string> reportMediaURIs;
                if (report.MediaURIs != null)
                    reportMediaURIs = report.MediaURIs;
                else
                    reportMediaURIs = new List<string>();

                var confirmedMediaURIs = reportDto.MediaURIs.Select(m => ResourceManager.GetFilename(m.MediaURI)).ToList();
                toBeDeleted = reportMediaURIs.Where(m => !confirmedMediaURIs.Contains(m)).ToList();
                report.MediaURIs = confirmedMediaURIs.ToList();
            }

            ObjectMapper.Map(reportDto, report);

            //citizens' report are public by default
            //TODO: allow user to change this prop from client
            report.IsPublic = _session.Roles != null && _session.Roles.Contains(AppRoles.CITIZEN);

            //Files to be added are stored in media
            await ManageMedia(report, media, toBeDeleted, _azureManager, _cognitiveServicesManager);

            await CurrentUnitOfWork.SaveChangesAsync();

            NotificationEvent<ReportNotificationDto> notification = new NotificationEvent<ReportNotificationDto>(report.Id,
                _session.UserId.Value,
                ObjectMapper.Map<ReportNotificationDto>(report),
                EntityWriteAction.Update);
            await _backgroundJobManager.EnqueueEventAsync(notification);

            //Needed for MediaURIs property mapping
            var res = ObjectMapper.Map<ReportDto>(report);
            res.IsEditable = true;
            return res;
        }
    }
}
