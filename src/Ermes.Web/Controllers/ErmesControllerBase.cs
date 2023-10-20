using Abp.AspNetCore.Mvc.Controllers;
using Abp.Azure;
using Abp.AzureCognitiveServices.CognitiveServices;
using Abp.BackgroundJobs;
using Abp.Extensions;
using Abp.IO.Extensions;
using Abp.UI;
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
using Ermes.Roles;
using FusionAuthNetCore;
using io.fusionauth.domain;
using io.fusionauth.domain.api.user;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using User = io.fusionauth.domain.User;

namespace Ermes.Web.Controllers
{
    public abstract class ErmesControllerBase : AbpController
    {
        protected ErmesControllerBase()
        {
            LocalizationSourceName = ErmesConsts.LocalizationSourceName;
        }

        protected async Task<Person> CreateOrUpdatePersonInternalAsync(Person person, User user, int? organizationId, int? teamId, bool isFirstLogin, bool isNewUser, int volterId, List<Role> rolesToAssign, PersonManager _personManager)
        {
            //Manage Person on Ermes DB
            if (person == null)
            {
                person = new Person()
                {
                    FusionAuthUserGuid = user.id.Value,
                    Email = user.email,
                    LegacyId = volterId
                };
            }

            person.OrganizationId = organizationId;
            if (teamId.HasValue)
                person.TeamId = teamId;
            person.IsFirstLogin = isFirstLogin;
            person.IsNewUser = isNewUser;
            person.Username = user.username;

            Logger.Info("Ermes: Create or update Person: " + person.Username);
            long personId = await _personManager.InsertOrUpdatePersonAsync(person);

            //Delete old associations
            await _personManager.DeletePersonRolesAsync(personId);
            await CurrentUnitOfWork.SaveChangesAsync();

            // Assign roles
            foreach (Role rta in rolesToAssign)
            {
                PersonRole pr = new PersonRole()
                {
                    PersonId = personId,
                    RoleId = rta.Id
                };
                await _personManager.InsertPersonRoleAsync(pr);
            }

            await CurrentUnitOfWork.SaveChangesAsync();
            return person;
        }
        protected async Task<User> CreateUserInternalAsync(int volterId, IOptions<FusionAuthSettings> _fusionAuthSettings, IOptions<ErmesSettings> _ermesSettings)
        {
            var client = FusionAuth.GetFusionAuthClient(_fusionAuthSettings.Value);
            //Set the password based on current project
            string project = _ermesSettings != null ? _ermesSettings.Value.ErmesProject : "";
            if (string.IsNullOrWhiteSpace(project))
                project = ErmesConsts.DefaultProjectName;

            var user = new User()
            {
                active = true,
                email = string.Format("{0}@{1}.eu", volterId.ToString(), project.ToLower()),
                username = volterId.ToString(),
                password = string.Concat(project.ToLower(), ErmesConsts.DefaultYear),
                preferredLanguages = new List<string> { "it" }
            };

            //Create user on FusionAuth
            var newUser = new RegistrationRequest()
            {
                user = user,
                registration = new UserRegistration()
                {
                    applicationId = new Guid(_fusionAuthSettings.Value.ApplicationId),
                    roles = new List<string>() { AppRoles.FIRST_RESPONDER },
                },
                sendSetPasswordEmail = false,
                skipVerification = true,
                skipRegistrationVerification = true
            };

            var response = await client.RegisterAsync(null, newUser);

            if (response.WasSuccessful())
            {
                if (response.successResponse.user.id.HasValue)
                {
                    return response.successResponse.user;
                }
                else
                    throw new UserFriendlyException(L("FusionAuthUnknonwError"));
            }
            else
            {
                var fa_error = FusionAuth.ManageErrorResponse(response);
                throw new UserFriendlyException(fa_error.ErrorCode, fa_error.HasTranslation ? L(fa_error.Message) : fa_error.Message);
            }
        }
        protected async Task<ReportDto> CreateReportAsync(
            ReportDto reportDto,
            int? refOrgId,
            string[] userRoles,
            long refUserId,
            ReportManager _reportManager,
            MissionManager _missionManager,
            CategoryManager _categoryManager,
            OrganizationManager _organizationManager,
            PersonManager _personManager,
            GamificationManager _gamificationManager,
            IAzureManager _azureManager,
            ICognitiveServicesManager _cognitiveServicesManager,
            IBackgroundJobManager _backgroundJobManager,
            IOptions<ErmesSettings> _ermesSettings,
            IFormFileCollection media = null)
        {
            var tuple = await CheckReportValidityAsync(reportDto, _missionManager, _categoryManager, refOrgId);
            if (tuple != null && !tuple.Item1.IsNullOrWhiteSpace())
                throw new UserFriendlyException(L(tuple.Item1, tuple.Item2));

            var report = ObjectMapper.Map<Report>(reportDto);

            //Not mapped automatically, in update phase we need to ignore this prop
            report.Timestamp = reportDto.Timestamp;

            //citizens' report are public by default
            report.IsPublic = userRoles != null && userRoles.Contains(AppRoles.CITIZEN);
            //Need Id here, so that I can create Azure Report container
            report.Id = await _reportManager.InsertReportAsync(report);

            //If created without an active session (but through api key), CreatorUserId is not automatically set by EF
            if (!report.CreatorUserId.HasValue)
                report.CreatorUserId = refUserId;

            // Media content management
            await ManageMedia(report, media, null, _azureManager, _cognitiveServicesManager);

            await CurrentUnitOfWork.SaveChangesAsync();

            NotificationEvent<ReportNotificationDto> notification = new NotificationEvent<ReportNotificationDto>(report.Id,
                refUserId,
            ObjectMapper.Map<ReportNotificationDto>(report),
                EntityWriteAction.Create);
            await _backgroundJobManager.EnqueueEventAsync(notification);

            /////FASTER CSI service integration
            bool mustSendReport = _ermesSettings.Value != null && _ermesSettings.Value.ErmesProject == AppConsts.Ermes_Faster && refOrgId.HasValue;
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
            if (userRoles.Contains(AppRoles.CITIZEN))
            {
                //Gamification section
                Person p = await _personManager.GetPersonByIdAsync(refUserId);
                action = await _gamificationManager.GetActionByNameAsync(ErmesConsts.GamificationActionConsts.DO_REPORT);
                async Task<List<(EntityWriteAction, string NewValue, int EarnedPoints)>> AssignRewards(long personId)
                {
                    List<(EntityWriteAction, string newValue, int earnedPoints)> result = new List<(EntityWriteAction, string newValue, int earnedPoints)>();
                    var person = await _personManager.GetPersonByIdAsync(personId);

                    var reports = await _reportManager.GetReportsByPersonAsync(personId);
                    if (reports.Count == 1) // it user's first report, assign points
                    {
                        var isFirstReportAction = await _gamificationManager.GetActionByNameAsync(ErmesConsts.GamificationActionConsts.FIRST_REPORT);
                        await _gamificationManager.InsertAudit(refUserId, isFirstReportAction.Id, null, null);
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
                                    await _gamificationManager.InsertAudit(refUserId, null, item.Id, null);
                                    person.Points += item.Detail.Points;
                                    result.Add((EntityWriteAction.MedalObtained, item.Name, item.Detail.Points));
                                }
                            }
                        }
                    }

                    return result;
                }

                //The list contains the information about the notification to be sent
                var list = await _gamificationManager.UpdatePersonGamificationProfileAsync(refUserId, ErmesConsts.GamificationActionConsts.DO_REPORT, AssignRewards);

                foreach (var item in list)
                {
                    NotificationEvent<GamificationNotificationDto> gamNotification = new NotificationEvent<GamificationNotificationDto>(0,
                    refUserId,
                    new GamificationNotificationDto()
                    {
                        PersonId = refUserId,
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
        protected async Task<Tuple<string, string>> CheckReportValidityAsync(ReportDto report, MissionManager _missionManager, CategoryManager _categoryManager, int? refOrgId)
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
                if (mission == null || mission.OrganizationId != refOrgId)
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
                                    if (!res)
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
        protected async Task ManageMedia(Report report, IFormFileCollection media, List<string> toBeDeleted, IAzureManager _azureManager, ICognitiveServicesManager _cognitiveServicesManager)
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
                    await _azureReportStorageManager.UploadFile(fileNameWithFolder, fileBytes, mimeType);
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
                            if (imageAnalysis != null && imageAnalysis.Adult != null)
                            {
                                if (report.AdultInfo == null)
                                    report.AdultInfo = new List<ReportAdultInfo>();

                                var info = new ReportAdultInfo()
                                {
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
                            await _azureThumbnailStorageManager.UploadFile(thumbnailPath, ErmesCommon.GetJpegThumbnail(fileBytes, ErmesConsts.Thumbnail.SIZE, ErmesConsts.Thumbnail.QUALITY, Logger), MimeTypeNames.ImageJpeg);
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e.Message);
                            //Use Cognitive services to create image thumbnail
                            if (imageStream != null)
                            {
                                var newFileBytes = await _cognitiveServicesManager.GetImageThumbnail(ErmesConsts.Thumbnail.SIZE, ErmesConsts.Thumbnail.SIZE, imageStream);
                                if (newFileBytes != null)
                                {
                                    await _azureThumbnailStorageManager.UploadFile(thumbnailPath, newFileBytes, MimeTypeNames.ImageJpeg);
                                    Logger.InfoFormat("Thumbnail {0} obtained using Azure Cognitive Services", thumbnailPath);
                                }
                                else
                                {
                                    //upload original image
                                    await _azureThumbnailStorageManager.UploadFile(thumbnailPath, fileBytes, MimeTypeNames.ImageJpeg);
                                    Logger.WarnFormat("Azure Cognitive Services failed in creating the thumbnail {0}", thumbnailPath);
                                }
                            }
                            else
                                await _azureThumbnailStorageManager.UploadFile(thumbnailPath, fileBytes, MimeTypeNames.ImageJpeg);
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
                        report.AdultInfo = report.AdultInfo.Where(t => t.MediaURI != item).ToList();
                    }
                }
            }
        }
    }
}