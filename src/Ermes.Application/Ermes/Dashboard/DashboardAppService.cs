using Abp.Localization;
using Ermes.Actions.Dto;
using Ermes.Attributes;
using Ermes.Authorization;
using Ermes.Dashboard.Dto;
using Ermes.GeoJson;
using Ermes.Helpers;
using Ermes.Linq.Extensions;
using Ermes.Missions;
using Ermes.Reports;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Dashboard
{
    [ErmesAuthorize]
    public class DashboardAppService : ErmesAppServiceBase, IDashboardAppService
    {
        private readonly ReportManager _reportManager;
        private readonly MissionManager _missionManager;
        private readonly IGeoJsonBulkRepository _geoJsonBulkRepository;
        private readonly ErmesAppSession _session;
        private readonly ILanguageManager _languageManager;
        private readonly ErmesPermissionChecker _permissionChecker;
        public DashboardAppService(
                ReportManager reportManager,
                MissionManager missionManager,
                IGeoJsonBulkRepository geoJsonBulkRepository,
                ErmesAppSession session,
                ILanguageManager languageManager,
                ErmesPermissionChecker permissionChecker
            )
        {
            _reportManager = reportManager;
            _missionManager = missionManager;
            _session = session;
            _languageManager = languageManager;
            _geoJsonBulkRepository = geoJsonBulkRepository;
            _permissionChecker = permissionChecker;
        }
        public virtual async Task<GetStatisticsOutput> GetStatistics(GetStatisticsInput input)
        {
            var start = input.StartDate ?? DateTime.MinValue;
            var end = input.EndDate ?? DateTime.MaxValue;
            var person = _session.LoggedUserPerson;

            //Reports////////////
            IQueryable<Report> queryReport;

            if (input.NorthEastBoundary != null && input.SouthWestBoundary != null)
            {
                Geometry boundingBox = GeometryHelper.GetPolygonFromBoundaries(input.SouthWestBoundary, input.NorthEastBoundary);
                queryReport = _geoJsonBulkRepository.GetReports(start, end, boundingBox);
            }
            else
                queryReport = _reportManager.GetReports(start, end);

            var hasPermission = _permissionChecker.IsGranted(_session.Roles, AppPermissions.Reports.Report_CanSeeCrossOrganization);
            if(!hasPermission)
                queryReport = queryReport.DataOwnership(person.OrganizationId.HasValue ? new List<int>() { person.OrganizationId.Value } : null);
            /////////////////////
            
            //Missions///////////
            IQueryable<Mission> queryMission;

            if (input.NorthEastBoundary != null && input.SouthWestBoundary != null)
            {
                Geometry boundingBox = GeometryHelper.GetPolygonFromBoundaries(input.SouthWestBoundary, input.NorthEastBoundary);
                queryMission = _geoJsonBulkRepository.GetMissions(input.StartDate.Value, input.EndDate.Value, boundingBox);
            }
            else
                queryMission = _missionManager.GetMissions(start, end);

            hasPermission = _permissionChecker.IsGranted(_session.Roles, AppPermissions.Missions.Mission_CanSeeCrossOrganization);
            if (!hasPermission)
                queryMission = queryMission.DataOwnership(person.OrganizationId.HasValue ? new List<int>() { person.OrganizationId.Value } : null);
            //////////////////////

            //Persons////////////
            int[] orgIdList;
            hasPermission = _permissionChecker.IsGranted(_session.Roles, AppPermissions.Actions.Action_CanSeeCrossOrganization);
            if (!hasPermission)
                orgIdList = null;
            else
                orgIdList = _session.LoggedUserPerson.OrganizationId.HasValue ? new int[] { _session.LoggedUserPerson.OrganizationId.Value } : null;

            var items = _geoJsonBulkRepository.GetPersonActions(start, end, orgIdList, null, null, null, null, _languageManager.CurrentLanguage.Name);
            var deserialized = JsonConvert.DeserializeObject<GetActionsOutput>(items);
            var actions = deserialized != null && deserialized.PersonActions != null ? deserialized.PersonActions : new List<PersonActionDto>();
            ////////////////////
            
            return new GetStatisticsOutput()
            {
                ReportsByHazard = queryReport
                                    .GroupBy(r => r.HazardString)
                                    .Select(g => new GroupDto() { 
                                        Id = g.Key,
                                        Label = g.Key,
                                        Value = g.Count()
                                    })
                                    .ToList(),
                MissionsByStatus = queryMission
                                    .GroupBy(r => r.CurrentStatusString)
                                    .Select(g => new GroupDto()
                                    {
                                        Id = g.Key,
                                        Label = g.Key,
                                        Value = g.Count()
                                    })
                                    .ToList(),
                PersonsByStatus = actions
                                    .GroupBy(a => a.Status.ToString())
                                    .Select(g => new GroupDto()
                                    {
                                        Id = g.Key,
                                        Label = g.Key,
                                        Value = g.Count()
                                    })
                                    .ToList(),
                Persons = actions
            };
        }
    }
}
