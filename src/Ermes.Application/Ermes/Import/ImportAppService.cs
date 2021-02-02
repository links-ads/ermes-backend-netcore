using Abp.ObjectMapping;
using Abp.UI;
using Ermes.Attributes;
using Ermes.Authorization;
using Ermes.Roles.Dto;
using Ermes.Roles;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ermes.Permissions;
using Ermes.Import.Dto;
using Microsoft.AspNetCore.Http;
using Ermes.Interfaces;
using Ermes.Net.MimeTypes;
using System.Linq;
using Ermes.Helpers;
using System.IO;
using Abp.IO.Extensions;
using Ermes.Activities;
using OfficeOpenXml;
using Ermes.Localization;
using Abp.Azure;
using Ermes.Resources;
using NSwag.Annotations;
using Abp.BackgroundJobs;
using Ermes.Enums;
using Ermes.Jobs;
using Abp.Domain.Uow;
using Ermes.Categories;
using Ermes.Reports.Dto;
using static Ermes.Resources.ResourceManager;

namespace Ermes.Import
{
    [ErmesAuthorize(AppPermissions.Imports.Import)]
    public class ImportAppService : ErmesAppServiceBase, IImportAppService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAppFolders _appFolders;
        private readonly ActivityManager _activityManager;
        private readonly ErmesLocalizationHelper _localizer;
        private readonly IAzureManager _azureManager;
        private readonly ErmesAppSession _session;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly CategoryManager _categoryManager;

        public ImportAppService(
            IHttpContextAccessor httpContextAccessor,
            ActivityManager activityManager,
            CategoryManager categoryManager,
            IAppFolders appFolders,
            ErmesLocalizationHelper localizer,
            IAzureManager azureManager,
            IBackgroundJobManager backgroundJobManager,
            ErmesAppSession session)
        {
            _httpContextAccessor = httpContextAccessor;
            _appFolders = appFolders;
            _activityManager = activityManager;
            _localizer = localizer;
            _azureManager = azureManager;
            _session = session;
            _backgroundJobManager = backgroundJobManager;
            _categoryManager = categoryManager;
        }

        private async Task UploadImportSourceAndCleanup(IImportResourceContainer resourceContainer, bool success, IFormFile file, String tempFilePath)
        {
            var _azureStorageManager = _azureManager.GetStorageManager(ResourceManager.GetBasePath(resourceContainer.ContainerName));
            byte[] pictureBytes;
            using (var stream = file.OpenReadStream())
            {
                pictureBytes = stream.GetAllBytes();
            }

            string fileExtension = file.FileName.Split(".").LastOrDefault();
            string mimeType = ErmesCommon.GetMimeTypeFromFileExtension(fileExtension);
            string fileName = resourceContainer.FileNameBase + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + _session.UserId + "." + fileExtension;
            string fileNameWithFolder = resourceContainer.GetRelativeMediaPath(success, fileName);
            try
            {
                await _azureStorageManager.UploadFile(fileNameWithFolder, pictureBytes, mimeType);
            }
            catch (Exception)
            {
                if (success)
                    throw new UserFriendlyException(L("AzureBackupFailed"));
            }
            finally
            {
                File.Delete(tempFilePath);
            }
        }

        private async Task<ImportResultDto> LoadFileAndImport(Func<string, string, Task<ImportResultDto>> importerFunction, IImportResourceContainer resourceContainer, string[] acceptedSourceMimeTypes)
        {
            HttpRequest request = _httpContextAccessor.HttpContext.Request;

            if (request.ContentLength == 0 || !request.HasFormContentType || request.Form.Files.Count != 1)
                throw new UserFriendlyException(L("FormFileNotReceived"));

            IFormFile file = request.Form.Files.FirstOrDefault();

            if (file == null)
                throw new UserFriendlyException(L("FormFileNotReceived"));

            if (!acceptedSourceMimeTypes.Contains(file.ContentType))
                throw new UserFriendlyException(L("MimeTypeNotAccepted"));

            //Save file locally
            byte[] fileBytes;
            using (var stream = file.OpenReadStream())
            {
                fileBytes = stream.GetAllBytes();
            }

            string tempFilePath = Path.Combine(_appFolders.TempFileDownloadFolder, file.FileName);
            bool skipUpload = false;
            try
            {
                File.WriteAllBytes(tempFilePath, fileBytes);
                ImportResultDto toRet = await importerFunction(tempFilePath, file.ContentType);
                skipUpload = true;
                await UploadImportSourceAndCleanup(resourceContainer, true, file, tempFilePath);
                return toRet;
            }
            catch (Exception e)
            {
                if (!skipUpload)
                    await UploadImportSourceAndCleanup(resourceContainer, false, file, tempFilePath);
                throw new UserFriendlyException(e.Message);
            }

        }

        #region Activities

        private static readonly string[] AcceptedActivitySourceMimeTypes = { MimeTypeNames.ApplicationVndMsExcel, MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet };

        [OpenApiOperation("Import Activities",
           @"
                Import (creating or updating) a list of activities and relative translations.
                Input: attach as form-data an excel (.xls or .xlsx) file with the following format:
                    First row:        (ignored)           | (ignored)                  | 2-letter language1 code  | 2-letter language2 code  | ....
                    Following rows:   activity short code | parent activity short code | translated text in lang1 | translated text in lang2 | ....
                Output: An import result dto, containing a summary of insertions and updates
                N.B; Short Code max length = 8
            "
       )]
        [ErmesAuthorize(AppPermissions.Imports.Import_Activities)]
        public virtual async Task<ImportResultDto> ImportActivities()
        {
            return await LoadFileAndImport((filename, contentType) =>
            {
                return ActivitiesImporter.ImportActivitiesAsync(filename, contentType, _activityManager, _localizer);
            }, new ImportActivitiesResourceContainer(), AcceptedActivitySourceMimeTypes);            
        }
        #endregion

        #region CompetenceAreas
        private static readonly string[] AcceptedCompetenceAreaSourceMimeTypes = { MimeTypeNames.ApplicationGeoJson };

        [ErmesAuthorize(AppPermissions.Imports.Import_CompetenceArea)]
        public virtual async Task<bool> ImportCompetenceAreas()
        {
            var context = _httpContextAccessor.HttpContext;
            var request = context.Request;

            if (request.ContentLength == 0)
                throw new UserFriendlyException(L("InvalidFile"));

            var file = request.Form.Files.FirstOrDefault();
            var validation = ErmesCommon.ValidateFile(file, AcceptedCompetenceAreaSourceMimeTypes);
            if (validation != null)
                throw new UserFriendlyException(L(validation));

            //Save file locally
            byte[] fileBytes;
            using (var stream = file.OpenReadStream())
            {
                fileBytes = stream.GetAllBytes();
            }
            try
            {
                var tempFilePath = Path.Combine(_appFolders.TempFileDownloadFolder, file.FileName);
                File.WriteAllBytes(tempFilePath, fileBytes);
                EnqueueNewBackgroundJob(file.FileName, GetCompetenceAreaTypeFromFileName(file.FileName));
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }

            Logger.Info("Ermes: ImportCompetenceAreas from file: " + file.FileName);
            return true;
        }

        private void EnqueueNewBackgroundJob(string fileName, CompetenceAreaType type)
        {
            _backgroundJobManager.Enqueue<ImportCompetenceAreasJob, ImportCompetenceAreasJobArgs>(
                new ImportCompetenceAreasJobArgs
                {
                    Filename = fileName,
                    CompetenceAreaType = type
                });
        }
        #endregion

        #region Categories

        private static readonly string[] AcceptedCategorySourceMimeTypes = { MimeTypeNames.ApplicationVndMsExcel, MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet };

        [OpenApiOperation("Import Categories",
           @"
                Import (creating or updating) a list of activities and relative translations.
                Input: attach as form-data an excel (.xls or .xlsx) file with the correct format
                Output: An import result dto, containing a summary of insertions and updates
s            "
       )]
        [ErmesAuthorize(AppPermissions.Imports.Import_Categories)]
        public virtual async Task<ImportResultDto> ImportCategories()
        {
            return await LoadFileAndImport((filename, contentType) =>
            {
                return CategoriesImporter.ImportCategoriesAsync(filename, contentType, _categoryManager, _localizer, CurrentUnitOfWork);
            }, new ImportCategoriesResourceContainer(), AcceptedCategorySourceMimeTypes);
        }
        #endregion
    }
}