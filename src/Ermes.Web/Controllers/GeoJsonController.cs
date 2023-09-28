using Abp.Localization;
using Ermes;
using Ermes.Attributes;
using Ermes.GeoJson;
using Ermes.GeoJson.Dto;
using Ermes.Helpers;
using Ermes.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using NSwag.Annotations;
using System.Threading.Tasks;
using Ermes.Web.Controllers;
using System;
using Ermes.Authorization;
using Ermes.Organizations;
using System.Collections.Generic;
using Ermes.Enums;
using Ermes.Persons;
using Ermes.Web.Controllers.Dto;
using Ermes.Net.MimeTypes;
using System.IO;
using Ermes.Dto;

namespace Ermes.Web.Controllers
{
    [ErmesAuthorize]
    public class GeoJsonController : ErmesControllerBase, IBackofficeApi
    {
        IGeoJsonBulkRepository _geoJsonBulkRepository;
        ILanguageManager _languageManager;
        private readonly ErmesAppSession _session;
        private readonly OrganizationManager _organizationManager;
        private readonly PersonManager _personManager;
        private readonly ErmesPermissionChecker _permissionChecker;
        private readonly IAppFolders _appFolders;
        private const string FILE_NAME = "collection_{0}.json";

        public GeoJsonController(
            IGeoJsonBulkRepository geoJsonBulkRepository, 
            ErmesAppSession session, 
            ILanguageManager languageManager, 
            ErmesPermissionChecker permissionChecker, 
            OrganizationManager organizationManager,
            PersonManager personManager,
            IAppFolders appFolders)
        {
            _geoJsonBulkRepository = geoJsonBulkRepository;
            _languageManager = languageManager;
            _session = session;
            _permissionChecker = permissionChecker;
            _organizationManager = organizationManager;
            _personManager = personManager;
            _appFolders = appFolders;
        }

        private class PreserializedJsonResult : JsonResult
        {
            ContentResult result;
            public PreserializedJsonResult(string alreadySerializedJson) : base("")
            {
                result = new ContentResult
                {
                    Content = alreadySerializedJson,
                    ContentType = "application/json"
                };
            }
            public override void ExecuteResult(ActionContext context)
            {
                result.ExecuteResult(context);
            }
            public override async Task ExecuteResultAsync(ActionContext context)
            {
                await result.ExecuteResultAsync(context);
            }
        }

        [Route("api/services/app/GeoJson/GetFeatureCollection")]
        [HttpGet]
        [SwaggerResponse(typeof(GetGeoJsonCollectionOutput))]
        public virtual JsonResult GetFeatureCollection(GetGeoJsonCollectionInput input)
        {
            return new PreserializedJsonResult(GetFeatureCollectionInternal(input));
        }

        [Route("api/services/app/GeoJson/DownloadFeatureCollection")]
        [HttpGet]
        [SwaggerResponse(typeof(FileDto))]
        [OpenApiOperation("Download geojson collection",
            @"
                The API accepts the same input object as the GetFeatureCollection API, use it to filter the result.
                The response contains a FileDto object with the following properties:
                    - fileName
                    - fileType
                    - fileToken
                To download the file, it's necessary to do, from the client:
                    location.href = {{base_url}}/File/DownloadTempFile?fileType={fileType}&fileToken={fileToken}&fileName={fileName}'
                N.B.: the token can only be used once, after which the file is deleted from the server
            "
        )]
        public virtual async Task<FileDto> DownloadFeatureCollection(GetGeoJsonCollectionInput input)
        {
            var result = GetFeatureCollectionInternal(input);
            return FileHelper.CreateFile(string.Format(FILE_NAME, DateTime.UtcNow.ToShortDateString()), MimeTypeNames.ApplicationGeoJson, _appFolders.TempFileDownloadFolder, result);
        }

        private string GetFeatureCollectionInternal(GetGeoJsonCollectionInput input)
        {
            Geometry boundingBox = null;
            if (input.SouthWestBoundary != null && input.NorthEastBoundary != null)
                boundingBox = GeometryHelper.GetPolygonFromBoundaries(input.SouthWestBoundary, input.NorthEastBoundary);
            input.EndDate = input.EndDate == DateTime.MinValue ? DateTime.MaxValue : input.EndDate;
            var actIds = input.ActivityIds?.ToArray();
            var teamIds = input.TeamIds?.ToArray();
            int[] orgIdList;
            var hasPermission = _permissionChecker.IsGranted(_session.Roles, AppPermissions.Actions.Action_CanSeeCrossOrganization);
            if (hasPermission)
                orgIdList = _organizationManager.GetOrganizationIds();
            else
                orgIdList = _session.LoggedUserPerson.OrganizationId.HasValue ? new int[] { _session.LoggedUserPerson.OrganizationId.Value } : null;

            //Admin can see everything
            hasPermission = _permissionChecker.IsGranted(_session.Roles, AppPermissions.Communications.Communication_CanSeeCrossOrganization);
            List<CommunicationRestrictionType> communicationRestrictionTypes = input.CommunicationRestrictionTypes == null ? new List<CommunicationRestrictionType>() { CommunicationRestrictionType.None } : input.CommunicationRestrictionTypes;
            if (!hasPermission)
            {
                foreach (var item in _session.Roles)
                {
                    if (item == AppRoles.CITIZEN)
                    {
                        //communicationRestrictionTypes.Add(CommunicationRestrictionType.Citizen);
                        communicationRestrictionTypes = new List<CommunicationRestrictionType> { CommunicationRestrictionType.Citizen };
                        input.ReportVisibilityType = VisibilityType.Public;
                    }
                }
            }

            Person person = _personManager.GetPersonById(_session.LoggedUserPerson.Id);
            string personName = person.Username ?? person.Email;
            return _geoJsonBulkRepository.GetGeoJsonCollection(
                    input.StartDate,
                    input.EndDate,
                    boundingBox,
                    input.EntityTypes,
                    orgIdList,
                    input.StatusTypes,
                    actIds,
                    teamIds,
                    input.HazardTypes,
                    input.ReportStatusTypes,
                    input.MissionStatusTypes,
                    input.MapRequestStatusTypes,
                    input.MapRequestTypes,
                    input.ReportVisibilityType,
                    input.ReportContentTypes,
                    communicationRestrictionTypes,
                    input.CommunicationScopeTypes,
                    AppConsts.Srid,
                    personName,
                    person.OrganizationId.HasValue ? person.Organization.ParentId : null,
                    _languageManager.CurrentLanguage.Name
            );

            // I need to return a JsonResult or in case of exception, Abp produces html instead of json. However, the real
            // JsonResult serializes the object I give him while I have an already serialized one.
            //PreserializedJsonResult result = new PreserializedJsonResult(responseContent);

            //return result;
        }

    }
}

/*
 * VISIBILITY RULES:
 * 
 * 1) Missions and MapRequest: follow hierarchy rule, from top to bottom
 * 2) Reports: public reports can be seen by everyone, private reports follow hierarchy rule, from top to bottom
 * 3) Communications: visibility is defined by Scope + Restriction properties:
 *      - Scope == Public --> can be seen by everyone (Restriction is None by default)
 *      - Scope == Restricted  --> need to check Restriction:
 *              * Restriction == Citizen --> can be seen only by citizens
 *              * Restriction == Professional --> can be seen by professionals from any organization
 *              * Restriction == Organization --> follow hierarchy rule, in BOTH directions
 * 4) Persons: a citizen can only see his own position, a professional cannot see citizens' position, but he can see the position of persons 
 *             inside his organization or child organizations
 * 
 * 
 * NOTIFICATION RECEIVERS:
 * 
 * 1) Missions: the notification will be sent to:
 *      - CoordinatorPersonId --> only to that person
 *      - CoordinatorTeamId --> to all members of the team
 *      - OrganizationId --> to all members of the same organization of the creator (NOT sent to children organization)
 * 2) Communications: notification si sent only to active persons (Status != Off) inside the AOI of the communication
 *      Additional criteria is defined by Scope + Restriction
 *      - Scope == Public --> sent to everyone
 *      - Scope == Restricted  --> need to check Restriction:
 *              * Restriction == Citizen --> all persons without organizationId
 *              * Restriction == Professional --> all persons with organizationId
 *              * Restriction == Organization --> follow hierarchy rule, from top to bottom
 *3) Reports: only bus notification
 *4) Gamification: notification sent to the current logged person
 */
