﻿using Abp.Azure;
using Abp.BackgroundJobs;
using Abp.Extensions;
using Abp.IO.Extensions;
using Abp.UI;
using Ermes;
using Ermes.Attributes;
using Ermes.Categories;
using Ermes.Enums;
using Ermes.EventHandlers;
using Ermes.Helpers;
using Ermes.Missions;
using Ermes.Net.MimeTypes;
using Ermes.Reports;
using Ermes.Reports.Dto;
using Ermes.Resources;
using Ermes.Web.Controllers;
using Ermes.Web.Controllers.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IAzureManager _azureManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBackgroundJobManager _backgroundJobManager;

        public ReportsController(
                        CategoryManager categoryManager,
                        ReportManager reportManager,
                        ErmesAppSession session,
                        MissionManager missionManager,
                        IHttpContextAccessor httpContextAccessor,
                        IAzureManager azureManager,
                        IBackgroundJobManager backgroundJobManager
                    )
        {
            _httpContextAccessor = httpContextAccessor;
            _categoryManager = categoryManager;
            _reportManager = reportManager;
            _session = session;
            _missionManager = missionManager;
            _azureManager = azureManager;
            _backgroundJobManager = backgroundJobManager;
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

            string message = await CheckReportValidityAsync(reportDto);
            if (!message.IsNullOrWhiteSpace())
                throw new UserFriendlyException(L(message));

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
            string message = await CheckReportValidityAsync(reportDto);
            if (!message.IsNullOrWhiteSpace())
                throw new UserFriendlyException(L(message));

            var report = ObjectMapper.Map<Report>(reportDto);

            //Not mapped automatically, in update phase we need to ignore this prop
            report.Timestamp = reportDto.Timestamp;
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

            var res = ObjectMapper.Map<ReportDto>(report);
            res.IsEditable = true;
            return res;
        }

        private async Task<string> CheckReportValidityAsync(ReportDto report)
        {
            if (report.Location == null)
                return "InvalidLocation";

            if (report.Timestamp == DateTime.MinValue)
                return "InvalidTimestamp";

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
                        return "InvalidCategory";

                    switch (cat.Type)
                    {
                        case CategoryType.Range:
                            var localizedValues = ObjectMapper.Map<LocalizedCategoryValuesDto>(cat);
                            if (!localizedValues.Values.Contains(item.Value))
                                return "InvalidCategoryRangeValue";
                            break;
                        case CategoryType.Numeric:
                            var res = float.TryParse(item.Value, out float numericValue);
                            if (!res)
                                return "InvalidCategoryNumericValue";
                            if (cat.MinValue > numericValue || cat.MaxValue < numericValue)
                                return "InvalidCategoryNumericValue";
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
                        string thumbnailName = ResourceManager.Thumbnails.GetJpegThumbnailFilename(uploadedFileName);
                        string thumbnailPath = ResourceManager.Thumbnails.GetRelativeMediaPath(report.Id, thumbnailName);
                        try
                        {
                            await _azureThumbnailStorageManager.UploadFile(thumbnailPath, ErmesCommon.GetJpegThumbnail(fileBytes, AppConsts.ThumbnailSize, AppConsts.ThumbnailQuality), MimeTypeNames.ImageJpeg);
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e.Message);
                            //TODO: improve management in case of error
                            //upload original image
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
                    }
                }
            }
        }
    }
}
