﻿using Abp.Localization;
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

namespace Ermes.Web.Controllers
{
    [ErmesAuthorize]
    public class GeoJsonController : ErmesControllerBase, IBackofficeApi
    {
        IGeoJsonBulkRepository _geoJsonBulkRepository;
        ILanguageManager _languageManager;
        private readonly ErmesAppSession _session;
        private readonly ErmesPermissionChecker _permissionChecker;
        public GeoJsonController(IGeoJsonBulkRepository geoJsonBulkRepository, ErmesAppSession session, ILanguageManager languageManager, ErmesPermissionChecker permissionChecker)
        {
            _geoJsonBulkRepository = geoJsonBulkRepository;
            _languageManager = languageManager;
            _session = session;
            _permissionChecker = permissionChecker;
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
            Geometry boundingBox = null;
            if(input.SouthWestBoundary != null && input.NorthEastBoundary != null)
                boundingBox = GeometryHelper.GetPolygonFromBoundaries(input.SouthWestBoundary, input.NorthEastBoundary);
            input.EndDate = input.EndDate == DateTime.MinValue ? DateTime.MaxValue : input.EndDate;
            var actIds = input.ActivityIds?.ToArray();
            int[] orgIdList;
            var hasPermission = _permissionChecker.IsGranted(_session.Roles, AppPermissions.Actions.Action_CanSeeCrossOrganization);
            if (hasPermission)
                orgIdList = null;
            else
                orgIdList = _session.LoggedUserPerson.OrganizationId.HasValue ? new int[] { _session.LoggedUserPerson.OrganizationId.Value } : null;
            string responseContent = _geoJsonBulkRepository.GetGeoJsonCollection(input.StartDate, input.EndDate, boundingBox, input.EntityTypes, orgIdList, input.StatusTypes, actIds, AppConsts.Srid, _languageManager.CurrentLanguage.Name);

            // I need to return a JsonResult or in case of exception, Abp produces html instead of json. However, the real
            // JsonResult serializes the object I give him while I have an already serialized one.
            PreserializedJsonResult result = new PreserializedJsonResult(responseContent);

            return result;
        }

    }
}
