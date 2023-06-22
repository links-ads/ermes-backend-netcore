using Abp.Azure;
using Abp.AzureCognitiveServices.CognitiveServices;
using Abp.AzureCognitiveServices.CognitiveServices.ComputerVision;
using Abp.BackgroundJobs;
using Abp.Extensions;
using Abp.IO.Extensions;
using Abp.UI;
using Ermes;
using Ermes.Attributes;
using Ermes.Authorization;
using Ermes.Categories;
using Ermes.Configuration;
using Ermes.Enums;
using Ermes.EventHandlers;
using Ermes.Gamification;
using Ermes.Gamification.Dto;
using Ermes.Helpers;
using Ermes.Jobs;
using Ermes.Missions;
using Ermes.Net.MimeTypes;
using Ermes.Organizations;
using Ermes.Persons;
using Ermes.Reports;
using Ermes.Reports.Dto;
using Ermes.Resources;
using Ermes.Web.Controllers;
using Ermes.Web.Controllers.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
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
                res.Report = await CreateReportAsync(input.Report, media);
            else
                res.Report = await UpdateReportAsync(input.Report, media);

            return res;
        }

        private async Task<ReportDto> UpdateReportAsync(ReportDto reportDto, IFormFileCollection media = null)
        {

            var tuple = await CheckReportValidityAsync(reportDto);
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
            await ManageMedia(report, media, toBeDeleted);

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

        private async Task<ReportDto> CreateReportAsync(ReportDto reportDto, IFormFileCollection media = null)
        {
            var tuple = await CheckReportValidityAsync(reportDto);
            if (tuple != null && !tuple.Item1.IsNullOrWhiteSpace())
                throw new UserFriendlyException(L(tuple.Item1, tuple.Item2));

            var report = ObjectMapper.Map<Report>(reportDto);

            //Not mapped automatically, in update phase we need to ignore this prop
            report.Timestamp = reportDto.Timestamp;

            //citizens' report are public by default
            report.IsPublic = _session.Roles != null && _session.Roles.Contains(AppRoles.CITIZEN);
            //Need Id here, so that I can create Azure Report container
            report.Id = await _reportManager.InsertReportAsync(report);

            // Media content management
            await ManageMedia(report, media, null);

            await CurrentUnitOfWork.SaveChangesAsync();

            NotificationEvent<ReportNotificationDto> notification = new NotificationEvent<ReportNotificationDto>(report.Id,
                _session.UserId.Value,
                ObjectMapper.Map<ReportNotificationDto>(report),
                EntityWriteAction.Create);
            await _backgroundJobManager.EnqueueEventAsync(notification);

            /////FASTER CSI service integration
            bool mustSendReport = _ermesSettings.Value != null && _ermesSettings.Value.ErmesProject == AppConsts.Ermes_Faster && _session.LoggedUserPerson.OrganizationId.HasValue;
            if (mustSendReport)
            {
                var refOrg = await _organizationManager.GetOrganizationByIdAsync(report.Creator.OrganizationId.Value);
                var housePartner = await SettingManager.GetSettingValueAsync(AppSettings.General.HouseOrganization);
                if (refOrg.Name == housePartner || (refOrg.ParentId.HasValue && refOrg.Parent.Name == housePartner))
                {
                    _backgroundJobManager.Enqueue<SendReportJob, SendReportJobArgs>(
                    new SendReportJobArgs
                    {
                        ReportId = report.Id
                    });
                }
            }
            ///////////////////

            var res = ObjectMapper.Map<ReportDto>(report);
            res.IsEditable = true;
            GamificationAction action = null;
            if (_session.Roles.Contains(AppRoles.CITIZEN))
            {
                //Gamification section
                Person p = await _personManager.GetPersonByIdAsync(_session.LoggedUserPerson.Id);
                action = await _gamificationManager.GetActionByNameAsync(ErmesConsts.GamificationActionConsts.DO_REPORT);
                async Task<List<(EntityWriteAction, string NewValue, int EarnedPoints)>> AssignRewards(long personId)
                {
                    List<(EntityWriteAction, string newValue, int earnedPoints)> result = new List<(EntityWriteAction, string newValue, int earnedPoints)>();
                    var person = await _personManager.GetPersonByIdAsync(personId);

                    var reports = await _reportManager.GetReportsByPersonAsync(personId);
                    if (reports.Count == 1) // it user's first report, assign points
                    {
                        var isFirstReportAction = await _gamificationManager.GetActionByNameAsync(ErmesConsts.GamificationActionConsts.FIRST_REPORT);
                        await _gamificationManager.InsertAudit(_session.LoggedUserPerson.Id, isFirstReportAction.Id, null, null);
                        person.Points += isFirstReportAction.Points;
                        result.Add((EntityWriteAction.FirstReport, isFirstReportAction.Name, isFirstReportAction.Points));
                    }


                    if (action != null && action.Achievements != null && action.Achievements.Count > 0)
                    {
                        var personReports = await _reportManager.GetReportsByPersonAsync(personId);
                        foreach (var item in action.Achievements)
                        {
                            if (item is Medal)
                            {
                                if (item.Detail.Threshold == personReports.Count)
                                {
                                    await _gamificationManager.InsertAudit(_session.LoggedUserPerson.Id, null, item.Id, null);
                                    person.Points += item.Detail.Points;
                                    result.Add((EntityWriteAction.MedalObtained, item.Name, item.Detail.Points));
                                }

                            }
                        }
                    }

                    return result;
                }

                //The list contains the information about the notification to be sent
                var list = await _gamificationManager.UpdatePersonGamificationProfileAsync(_session.LoggedUserPerson.Id, ErmesConsts.GamificationActionConsts.DO_REPORT, AssignRewards);

                foreach (var item in list)
                {
                    NotificationEvent<GamificationNotificationDto> gamNotification = new NotificationEvent<GamificationNotificationDto>(0,
                    _session.LoggedUserPerson.Id,
                    new GamificationNotificationDto()
                    {
                        PersonId = _session.LoggedUserPerson.Id,
                        ActionName = item.Action.ToString(),
                        NewValue = item.NewValue,
                        EarnedPoints = item.EarnedPoints
                    },
                    item.Action,
                    true);
                    await _backgroundJobManager.EnqueueEventAsync(gamNotification);
                }

                res.Points = p.Points;
                res.LevelId = p.LevelId;
                res.LevelName = p.Level?.Name;
                res.EarnedPoints = action != null ? action.Points : 0;
            }
            return res;
        }

        private async Task<Tuple<string, string>> CheckReportValidityAsync(ReportDto report)
        {
            if (report.Location == null)
                return new Tuple<string, string>("InvalidLocation", "null");

            if (report.Timestamp == DateTime.MinValue)
                return new Tuple<string, string>("InvalidTimestamp", report.Timestamp.ToString());

            if (report.RelativeMissionId.HasValue && report.RelativeMissionId.Value > 0)
            {
                var mission = await _missionManager.GetMissionByIdAsync(report.RelativeMissionId.Value);

                //Do not throw exception if the mission Id is not valid
                //just delete the mission-report link and logs
                if (mission == null || mission.OrganizationId != _session.LoggedUserPerson.OrganizationId)
                {
                    Logger.WarnFormat("Create Report warning: RelativeMissionId {0} is not valid for the current report ({1}). Mission - Report link is deleted", report.RelativeMissionId.Value, report.Description);
                    report.RelativeMissionId = null;
                }
            }

            if (report.ExtensionData != null)
            {
                foreach (var item in report.ExtensionData)
                {
                    var cat = await _categoryManager.GetCategoryByIdAsync(item.CategoryId);
                    if (cat == null)
                        return new Tuple<string, string>("InvalidCategory", "null");

                    bool res;
                    switch (cat.Type)
                    {
                        case CategoryType.Range:
                            if (!cat.Translations.SelectMany(c => c.Values).Contains(item.Value))
                                return new Tuple<string, string>("InvalidCategoryRangeValue", item.Value);
                            break;
                        case CategoryType.Numeric:
                            switch (cat.FieldType)
                            {
                                case FieldType.None:
                                    break;
                                case FieldType.Int:
                                    res = decimal.TryParse(item.Value, out decimal tmpValue);
                                    if(!res)
                                        new Tuple<string, string>("InvalidCategoryNumericValue", item.Value);
                                    int intValue = (int)Math.Floor(tmpValue);
                                    if (int.Parse(cat.MinValue) > intValue || int.Parse(cat.MaxValue) < intValue)
                                        return new Tuple<string, string>("InvalidCategoryNumericValue", item.Value);
                                    break;
                                case FieldType.Decimal:
                                    res = decimal.TryParse(item.Value, out decimal decimalValue);
                                    if (!res)
                                        return new Tuple<string, string>("InvalidCategoryNumericValue", item.Value);
                                    if (decimal.Parse(cat.MinValue) > decimalValue || decimal.Parse(cat.MaxValue) < decimalValue)
                                        return new Tuple<string, string>("InvalidCategoryNumericValue", item.Value);
                                    break;
                                case FieldType.String:
                                    break;
                                case FieldType.Datetime:
                                    res = DateTime.TryParse(item.Value, out DateTime dateValue);
                                    if (!res)
                                        return new Tuple<string, string>("InvalidCategoryNumericValue", item.Value);
                                    if (DateTime.Parse(cat.MinValue) > dateValue || DateTime.Parse(cat.MaxValue) < dateValue)
                                        return new Tuple<string, string>("InvalidCategoryNumericValue", item.Value);
                                    break;
                                case FieldType.Boolean:
                                    res = bool.TryParse(item.Value, out bool boolValue);
                                    if (!res)
                                        return new Tuple<string, string>("InvalidCategoryNumericValue", item.Value);
                                    if (bool.Parse(cat.MinValue) != boolValue && bool.Parse(cat.MaxValue) != boolValue)
                                        return new Tuple<string, string>("InvalidCategoryNumericValue", item.Value);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            return null;
        }

        private async Task ManageMedia(Report report, IFormFileCollection media, List<string> toBeDeleted)
        {
            var _azureReportStorageManager = _azureManager.GetStorageManager(ResourceManager.GetBasePath(ResourceManager.Reports.ContainerName));
            var _azureThumbnailStorageManager = _azureManager.GetStorageManager(ResourceManager.GetBasePath(ResourceManager.Thumbnails.ContainerName));

            // Media content management
            if (media != null && media.Count > 0)
            {
                if (report.MediaURIs == null)
                    report.MediaURIs = new List<string>();

                var atLeastOneInappropriateContent = false;
                foreach (var file in media)
                {
                    byte[] fileBytes;
                    using (var stream = file.OpenReadStream())
                    {
                        fileBytes = stream.GetAllBytes();
                    }

                    string fileExtension = file.FileName.Split(".").LastOrDefault();
                    string mimeType = ErmesCommon.GetMimeTypeFromFileExtension(fileExtension);
                    //TBD
                    //if (mimeType.IsNullOrWhiteSpace())
                    //    throw new UserFriendlyException(L("UnsopportedMediaType"));
                    string uploadedFileName = string.Concat(Guid.NewGuid().ToString(), ".", fileExtension);

                    var fileNameWithFolder = ResourceManager.Reports.GetRelativeMediaPath(report.Id, uploadedFileName);
                    await _azureReportStorageManager.UploadFileAsync(fileNameWithFolder, fileBytes, mimeType);                    
                    MediaType mediaType = ErmesCommon.GetMediaTypeFromMimeType(mimeType);

                    if (mediaType == MediaType.Image)
                    {
                        Stream imageStream = null;
                        try
                        {
                            imageStream = new MemoryStream(fileBytes);
                            var imageAnalysis = await _cognitiveServicesManager.AnalyzeImage(imageStream);
                            if (imageAnalysis != null && imageAnalysis.Tags != null && imageAnalysis.Tags.Count > 0)
                            {
                                if (report.Tags == null)
                                    report.Tags = new List<ReportTag>();
                                report.Tags.AddRange(imageAnalysis.Tags.Select(t => new ReportTag()
                                {
                                    MediaURI = uploadedFileName,
                                    Name = t.Name,
                                    Confidence = t.Confidence
                                }).ToList());
                            }
                            if(imageAnalysis != null && imageAnalysis.Adult != null)
                            {
                                if (report.AdultInfo == null)
                                    report.AdultInfo = new List<ReportAdultInfo>();

                                var info = new ReportAdultInfo() { 
                                    IsAdultContent = imageAnalysis.Adult.IsAdultContent,
                                    IsGoryContent = imageAnalysis.Adult.IsGoryContent,
                                    IsRacyContent = imageAnalysis.Adult.IsRacyContent,
                                    AdultScore = imageAnalysis.Adult.AdultScore,
                                    GoreScore = imageAnalysis.Adult.GoreScore,
                                    RacyScore = imageAnalysis.Adult.RacyScore,
                                    MediaURI = uploadedFileName
                                };
                                report.AdultInfo.Add(info);

                                if (!atLeastOneInappropriateContent)
                                    atLeastOneInappropriateContent = imageAnalysis.Adult.IsAdultContent || imageAnalysis.Adult.IsGoryContent || imageAnalysis.Adult.IsRacyContent;
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Error("Error during image analisys phase: " + e.Message);
                        }
                        if (atLeastOneInappropriateContent)
                            report.Content = ReportContentType.Inappropriate;

                        string thumbnailName = ResourceManager.Thumbnails.GetJpegThumbnailFilename(uploadedFileName);
                        string thumbnailPath = ResourceManager.Thumbnails.GetRelativeMediaPath(report.Id, thumbnailName);
                        try
                        {
                            await _azureThumbnailStorageManager.UploadFileAsync(thumbnailPath, ErmesCommon.GetJpegThumbnail(fileBytes, AppConsts.ThumbnailSize, AppConsts.ThumbnailQuality), MimeTypeNames.ImageJpeg);
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e.Message);
                            //Use Cognitive services to create image thumbnail
                            if (imageStream != null) {
                                var newFileBytes = await _cognitiveServicesManager.GetImageThumbnail(AppConsts.ThumbnailSize, AppConsts.ThumbnailSize, imageStream);
                                if (newFileBytes != null)
                                {
                                    await _azureThumbnailStorageManager.UploadFileAsync(thumbnailPath, newFileBytes, MimeTypeNames.ImageJpeg);
                                    Logger.InfoFormat("Thumbnail {0} obtained using Azure Cognitive Services", thumbnailPath);
                                }
                                else
                                {
                                    //upload original image
                                    await _azureThumbnailStorageManager.UploadFileAsync(thumbnailPath, fileBytes, MimeTypeNames.ImageJpeg);
                                    Logger.WarnFormat("Azure Cognitive Services failed in creating the thumbnail {0}", thumbnailPath);
                                }
                            }
                            else
                                await _azureThumbnailStorageManager.UploadFileAsync(thumbnailPath, fileBytes, MimeTypeNames.ImageJpeg);
                        }
                    }

                    report.MediaURIs.Add(uploadedFileName);
                }
            }
            if (toBeDeleted != null && toBeDeleted.Count > 0)
            {
                foreach (var item in toBeDeleted)
                {
                    string filename = ResourceManager.Reports.GetRelativeMediaPath(report.Id, item);
                    await _azureReportStorageManager.DeleteBlobAsync(filename);

                    MediaType mediaType = ErmesCommon.GetMediaTypeFromFilename(item);
                    if (mediaType == MediaType.Image)
                    {
                        string thumbnailName = ResourceManager.Thumbnails.GetJpegThumbnailFilename(item);
                        try
                        {
                            await _azureThumbnailStorageManager.DeleteBlobAsync(ResourceManager.Thumbnails.GetRelativeMediaPath(report.Id, thumbnailName));
                        }
                        catch (Exception e) { Logger.Error(e.Message); }

                        //Delete Tags
                        report.Tags = report.Tags.Where(t => t.MediaURI != item).ToList();

                        //Delete Adult Info
                        report.AdultInfo= report.AdultInfo.Where(t => t.MediaURI != item).ToList();
                    }
                }
            }
        }
    }
}
