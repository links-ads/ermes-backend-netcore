using Abp.UI;
using Ermes.Activities.Dto;
using Ermes.Attributes;
using Ermes.Authorization;
using Ermes.Communications;
using Ermes.Dto.Spatial;
using Ermes.Enums;
using Ermes.GeoJson.Dto;
using Ermes.Helpers;
using Ermes.Persons;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ermes.GeoJson
{
    [ErmesAuthorize(AppPermissions.Backoffice)]
    [ErmesIgnoreApi(true)]
    public class GeoJsonAppService: ErmesAppServiceBase, IGeoJsonAppService
    {
        private readonly IGeoJsonBulkRepository _geoJsonBulkRepository;
        public GeoJsonAppService(IGeoJsonBulkRepository geoJsonBulkRepository)
        {
            _geoJsonBulkRepository = geoJsonBulkRepository;
        }

        
        public virtual GetGeoJsonCollectionOutput GetFeatureCollection2(GetGeoJsonCollectionInput input)
        {
            Coordinate ne = new Coordinate(input.NorthEastBoundary.Longitude, input.NorthEastBoundary.Latitude);
            Coordinate se = new Coordinate(input.NorthEastBoundary.Longitude, input.SouthWestBoundary.Latitude);
            Coordinate sw = new Coordinate(input.SouthWestBoundary.Longitude, input.SouthWestBoundary.Latitude);
            Coordinate nw = new Coordinate(input.SouthWestBoundary.Longitude, input.NorthEastBoundary.Latitude);
            Geometry boundingBox = new Polygon(new LinearRing(new Coordinate[] { nw, ne, se, sw, nw}));

            GetGeoJsonCollectionOutput retval = new GetGeoJsonCollectionOutput();

            var communications = ObjectMapper.Map<List<FeatureDto<GeoJsonItem>>>(_geoJsonBulkRepository.GetCommunications(input.StartDate, input.EndDate, boundingBox).ToList());
            retval.Features.AddRange(communications);

            var missions = ObjectMapper.Map<List<FeatureDto<GeoJsonItem>>>(_geoJsonBulkRepository.GetMissions(input.StartDate, input.EndDate, boundingBox).ToList());
            retval.Features.AddRange(missions);

            var reports = ObjectMapper.Map<List<FeatureDto<GeoJsonItem>>>(_geoJsonBulkRepository.GetReports(input.StartDate, input.EndDate, boundingBox).ToList());
            retval.Features.AddRange(reports);

            var reportRequests = ObjectMapper.Map<List<FeatureDto<GeoJsonItem>>>(_geoJsonBulkRepository.GetReportRequests(input.StartDate, input.EndDate, boundingBox).ToList());
            retval.Features.AddRange(reportRequests);


            List<PersonActionActivity> personActionActivities = _geoJsonBulkRepository
                .GetPersonActionActivities(input.StartDate, input.EndDate, boundingBox)
                .Include(p => p.Person)
                .Include(p => p.Activity)
                .ThenInclude(a => a.Translations)
                .ToList();
            foreach (PersonActionActivity paa in personActionActivities)
            {
                FeatureDto<GeoJsonItem> pad = ObjectMapper.Map<FeatureDto<GeoJsonItem>>(paa);
                pad.Properties.Type = EntityType.PersonActionActivity.ToString();
                pad.Properties.Details = L("GeoJsonDetail.PersonActionActivity", paa.Person.Username, ObjectMapper.Map<LocalizedActivityNameDto>(paa.Activity).Name);
                retval.Features.Add(pad);
            }

            List<PersonActionStatus> personActionStatuses = _geoJsonBulkRepository
                .GetPersonActionStatuses(input.StartDate, input.EndDate, boundingBox)
                .Include(p => p.Person)
                .ToList();
            foreach (PersonActionStatus pas in personActionStatuses)
            {
                FeatureDto<GeoJsonItem> pad = ObjectMapper.Map<FeatureDto<GeoJsonItem>>(pas);
                pad.Properties.Type = EntityType.PersonActionStatus.ToString();
                pad.Properties.Details = L("GeoJsonDetail.PersonActionStatus", pas.Person.Username, pas.StatusString);
                retval.Features.Add(pad);
            }

            List<PersonActionTracking> personActionTrackings = _geoJsonBulkRepository
                .GetPersonActionTrackings(input.StartDate, input.EndDate, boundingBox)
                .Include(p => p.Person)
                .ToList();
            foreach (PersonActionTracking pat in personActionTrackings)
            {
                FeatureDto<GeoJsonItem> pad = ObjectMapper.Map<FeatureDto<GeoJsonItem>>(pat);
                pad.Properties.Type = EntityType.PersonActionTracking.ToString();
                pad.Properties.Details = L("GeoJsonDetail.PersonActionTracking", pat.Person.Username, pat.ExtensionData);
                retval.Features.Add(pad);
            }

            return retval;
        }
    }
}
