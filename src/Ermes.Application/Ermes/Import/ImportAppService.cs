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
using Ermes.Organizations;
using Ermes.Persons;
using Ermes.Teams;
using Microsoft.Extensions.Options;
using FusionAuthNetCore;
using io.fusionauth.domain;
using Ermes.Tips;
using Ermes.Quizzes;
using Ermes.Answers;
using Ermes.Layers;
using Ermes.Gamification;

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
        private readonly PersonManager _personManager;
        private readonly OrganizationManager _organizationManager;
        private readonly TeamManager _teamManager;
        private readonly TipManager _tipManager;
        private readonly QuizManager _quizManager;
        private readonly GamificationManager _gamificationManager;
        private readonly AnswerManager _answerManager;
        private readonly LayerManager _layerManager;
        private readonly ErmesPermissionChecker _permissionChecker;
        private readonly IOptions<FusionAuthSettings> _fusionAuthSettings;

        public ImportAppService(
            IHttpContextAccessor httpContextAccessor,
            ActivityManager activityManager,
            CategoryManager categoryManager,
            PersonManager personManager,
            OrganizationManager organizationManager,
            TeamManager teamManager,
            IAppFolders appFolders,
            ErmesLocalizationHelper localizer,
            IAzureManager azureManager,
            IBackgroundJobManager backgroundJobManager,
            IOptions<FusionAuthSettings> fusionAuthSettings,
            ErmesPermissionChecker permissionChecker,
            ErmesAppSession session,
            TipManager tipManager,
            QuizManager quizManager,
            LayerManager layerManager,
            AnswerManager answerManager,
            GamificationManager gamificationManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _appFolders = appFolders;
            _activityManager = activityManager;
            _personManager = personManager;
            _organizationManager = organizationManager;
            _localizer = localizer;
            _azureManager = azureManager;
            _session = session;
            _backgroundJobManager = backgroundJobManager;
            _categoryManager = categoryManager;
            _teamManager = teamManager;
            _fusionAuthSettings = fusionAuthSettings;
            _permissionChecker = permissionChecker;
            _tipManager = tipManager;
            _quizManager = quizManager;
            _answerManager = answerManager;
            _layerManager = layerManager;
            _gamificationManager = gamificationManager;
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
        public virtual async Task<ImportResultDto> ImportActivities(IFormFile file)
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
                Import (creating or updating) a list of categories and relative translations.
                Input: attach as form-data an excel (.xls or .xlsx) file with the correct format
                Output: An import result dto, containing a summary of insertions and updates
s            "
       )]
        [ErmesAuthorize(AppPermissions.Imports.Import_Categories)]
        public virtual async Task<ImportResultDto> ImportCategories(IFormFile file)
        {
            return await LoadFileAndImport((filename, contentType) =>
            {
                return CategoriesImporter.ImportCategoriesAsync(filename, contentType, _categoryManager, _localizer, CurrentUnitOfWork);
            }, new ImportCategoriesResourceContainer(), AcceptedCategorySourceMimeTypes);
        }

        #endregion

        #region Users
        private static readonly string[] AcceptedUsersSourceMimeTypes = { MimeTypeNames.ApplicationVndMsExcel, MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet };
        [OpenApiOperation("Import Users",
            @"
                Import a list of users.
                Input: attach as form-data an excel (.xls or .xlsx) file with the correct format
                Output: An import result dto, containing a summary of insertions and updates
                N.B. the upload of existing users for an update has not been implemented yet.
            "
        )]
        [ErmesAuthorize(AppPermissions.Imports.Import_Users)]
        public virtual async Task<ImportResultDto> ImportUsers(IFormFile file)
        {
            //read file
            return await LoadFileAndImport(async (filename, contentType) =>
            {
                //create data structure
                var result = await UsersImporter.ImportUsersAsync(filename, contentType, _personManager, _organizationManager, _teamManager, _localizer);
                
                //add user to FusionAuth
                await ImportUsersInternalAsync(result.Accounts.Select(a => a.Item1).ToList(), _fusionAuthSettings);

                //add person on project DB
                foreach (var tuple in result.Accounts)
                {
                    var rolesToAssgin = await _personManager.GetRolesByName(tuple.Item1.Roles);
                    //check roles, organizations, teams and permission association
                    List<Role> rolesToAssign = await GetRolesAndCheckOrganizationAndTeam(tuple.Item1.Roles, tuple.Item2.OrganizationId, tuple.Item2.TeamId, tuple.Item2.Id, _personManager, _organizationManager, _teamManager, _session, _permissionChecker);
                    await CreateOrUpdatePersonInternalAsync(null, ObjectMapper.Map<User>(tuple.Item1), tuple.Item2.OrganizationId, tuple.Item2.TeamId, tuple.Item2.IsFirstLogin, tuple.Item2.IsNewUser, rolesToAssign, _personManager);
                }

                return ObjectMapper.Map<ImportResultDto>(result);
            }, new ImportUsersResourceContainer(), AcceptedUsersSourceMimeTypes);
        }

        #endregion

        #region Layers

        [OpenApiOperation("Import Layers",
           @"
                Import (creating or updating) a list of layer and relative translations.
                Input: attach as form-data an excel (.xls or .xlsx) file with the correct format
                Output: An import result dto, containing a summary of insertions and updates
s            "
       )]
        [ErmesAuthorize(AppPermissions.Imports.Import_Layers)]
        public virtual async Task<ImportResultDto> ImportLayers(IFormFile file)
        {
            return await LoadFileAndImport((filename, contentType) =>
            {
                return LayersImporter.ImportLayersAsync(filename, contentType, _layerManager, _localizer, CurrentUnitOfWork);
            }, new ImportLayersResourceContainer(), AcceptedCategorySourceMimeTypes);
        }

        #endregion

        private static readonly string[] AcceptedGamificationSourceMimeTypes = { MimeTypeNames.ApplicationVndMsExcel, MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet };

        #region Tips

        [OpenApiOperation("Import Tips",
           @"
                Import (creating or updating) a list of tips and relative translations.
                Input: attach as form-data an excel (.xls or .xlsx) file with the correct format
                Output: An import result dto, containing a summary of insertions and updates
            "
       )]
        [ErmesAuthorize(AppPermissions.Imports.Import_Gamification)]
        public virtual async Task<ImportResultDto> ImportTips(IFormFile file)
        {
            return await LoadFileAndImport((filename, contentType) =>
            {
                return TipsImporter.ImportTipsAsync(filename, contentType, _tipManager, _localizer, CurrentUnitOfWork);
            }, new ImportTipsResourceContainer(), AcceptedGamificationSourceMimeTypes);
        }

        #endregion

        #region Quizzes


        [OpenApiOperation("Import Quizzes",
           @"
                Import (creating or updating) a list of quizzes and relative translations.
                Input: attach as form-data an excel (.xls or .xlsx) file with the correct format
                Output: An import result dto, containing a summary of insertions and updates
s            "
       )]
        [ErmesAuthorize(AppPermissions.Imports.Import_Gamification)]
        public virtual async Task<ImportResultDto> ImportQuizzes(IFormFile file)
        {
            return await LoadFileAndImport((filename, contentType) =>
            {
                return QuizzesImporter.ImportQuizzesAsync(filename, contentType, _quizManager, _localizer, CurrentUnitOfWork);
            }, new ImportQuizzesResourceContainer(), AcceptedGamificationSourceMimeTypes);
        }

        #endregion

        #region Answers

        [OpenApiOperation("Import Answers",
           @"
                Import (creating or updating) a list of answers and relative translations.
                Input: attach as form-data an excel (.xls or .xlsx) file with the correct format
                Output: An import result dto, containing a summary of insertions and updates
s            "
       )]
        [ErmesAuthorize(AppPermissions.Imports.Import_Gamification)]
        public virtual async Task<ImportResultDto> ImportAnswers(IFormFile file)
        {
            return await LoadFileAndImport((filename, contentType) =>
            {
                return AnswersImporter.ImportAnswersAsync(filename, contentType, _answerManager, _localizer, CurrentUnitOfWork);
            }, new ImportAnswersResourceContainer(), AcceptedGamificationSourceMimeTypes);
        }

        #endregion

        #region Gamification Actions
        [OpenApiOperation("Import Gamification action",
            @"
                Import (creating or updating) a list of actions and relative translations.
                Input: attach as form-data an excel (.xls or .xlsx) file with the correct format
                Output: An import result dto, containing a summary of insertions and updates
            "
        )]
        [ErmesAuthorize(AppPermissions.Imports.Import_Gamification)]
        public virtual async Task<ImportResultDto> ImportGamificationActions(IFormFile file)
        {
            return await LoadFileAndImport((filename, contentType) =>
            {
                return GamificationActionsImporter.ImportGamificationActionsAsync(filename, contentType, _gamificationManager, _localizer, CurrentUnitOfWork);
            }, new ImportGamificationActionsResourceContainer(), AcceptedGamificationSourceMimeTypes);
        }
        #endregion
    }
}