using Abp.Localization;
using Abp.UI;
using Ermes.Actions.Dto;
using Ermes.Alerts;
using Ermes.Attributes;
using Ermes.Authorization;
using Ermes.Communications;
using Ermes.Dashboard.Dto;
using Ermes.Dto.Datatable;
using Ermes.Enums;
using Ermes.GeoJson;
using Ermes.Helpers;
using Ermes.Linq.Extensions;
using Ermes.MapRequests;
using Ermes.Missions;
using Ermes.Organizations;
using Ermes.Persons;
using Ermes.Reports;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Ermes.Dashboard
{
    [ErmesAuthorize]
    public class DashboardAppService : ErmesAppServiceBase, IDashboardAppService
    {
        private readonly ReportManager _reportManager;
        private readonly OrganizationManager _organizationManager;
        private readonly MissionManager _missionManager;
        private readonly IGeoJsonBulkRepository _geoJsonBulkRepository;
        private readonly ErmesAppSession _session;
        private readonly ILanguageManager _languageManager;
        private readonly ErmesPermissionChecker _permissionChecker;
        private readonly CommunicationManager _communicationManager;
        private readonly PersonManager _personManager;
        private readonly MapRequestManager _mapRequestManager;
        private readonly AlertManager _alertManager;
        public DashboardAppService(
                ReportManager reportManager,
                MissionManager missionManager,
                IGeoJsonBulkRepository geoJsonBulkRepository,
                ErmesAppSession session,
                ILanguageManager languageManager,
                ErmesPermissionChecker permissionChecker,
                OrganizationManager organizationManager,
                CommunicationManager communicationManager,
                PersonManager personManager,
                MapRequestManager mapRequestManager,
                AlertManager alertManager
            )
        {
            _reportManager = reportManager;
            _missionManager = missionManager;
            _session = session;
            _languageManager = languageManager;
            _geoJsonBulkRepository = geoJsonBulkRepository;
            _permissionChecker = permissionChecker;
            _organizationManager = organizationManager;
            _communicationManager = communicationManager;
            _personManager = personManager;
            _mapRequestManager = mapRequestManager;
            _alertManager = alertManager;
        }
        public virtual async Task<GetStatisticsOutput> GetStatistics(GetStatisticsInput input)
        {
            DateTime start = DateTime.MinValue, end = DateTime.MaxValue;
            if (!input.StartDate.HasValue && !input.EndDate.HasValue)
            {
                start = DateTime.Today.AddDays(-29);
                end = DateTime.Today.AddDays(1);
            }
            else if (input.StartDate.HasValue && !input.EndDate.HasValue)
            {
                start = input.StartDate.Value;
                end = input.StartDate.Value.AddDays(30);
                if (end > DateTime.Today.AddDays(1))
                    end = DateTime.Today.AddDays(1);
            }
            else if (!input.StartDate.HasValue && input.EndDate.HasValue)
            {
                end = input.EndDate.Value;
                start = input.EndDate.Value.AddDays(-30);
            }
            else
            {
                start = input.StartDate.Value;
                end = input.EndDate.Value;
            }

            var person = _session.LoggedUserPerson;
            if (start > end)
                throw new UserFriendlyException(L("TimeWindowLimitError"));
            var timestap = end.Subtract(start);
            //TODO: could be restored, but check needs to be implemented client-side
            //if (timestap.TotalDays > 30)
            //    throw new UserFriendlyException(L("TimeWindowSizeError"));

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
            if (!hasPermission)
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
            if (hasPermission)
                orgIdList = await _organizationManager.GetOrganizationIdsAsync();
            else
                orgIdList = _session.LoggedUserPerson.OrganizationId.HasValue ? new int[] { _session.LoggedUserPerson.OrganizationId.Value } : null;

            string personName = _session.LoggedUserPerson.Username ?? _session.LoggedUserPerson.Email;
            var items = _geoJsonBulkRepository.GetPersonActions(start, end, orgIdList, null, null, null, null, personName, null, _languageManager.CurrentLanguage.Name);
            var deserialized = JsonConvert.DeserializeObject<GetActionsOutput>(items);
            var actions = deserialized != null && deserialized.PersonActions != null ? deserialized.PersonActions : new List<PersonActionDto>();
            ////////////////////

            //Map Requests///////////
            IQueryable<MapRequest> queryMapRequests;

            if (input.NorthEastBoundary != null && input.SouthWestBoundary != null)
            {
                Geometry boundingBox = GeometryHelper.GetPolygonFromBoundaries(input.SouthWestBoundary, input.NorthEastBoundary);
                queryMapRequests = _geoJsonBulkRepository.GetMapRequests(input.StartDate.Value, input.EndDate.Value, boundingBox);
            }
            else
                queryMapRequests = _mapRequestManager.GetMapRequests(start, end);

            hasPermission = _permissionChecker.IsGranted(_session.Roles, AppPermissions.MapRequests.MapRequest_CanSeeCrossOrganization);
            if (!hasPermission)
            {
                if (_session.LoggedUserPerson.OrganizationId.HasValue)
                    queryMapRequests = queryMapRequests.DataOwnership(new List<int>() { _session.LoggedUserPerson.OrganizationId.Value });
            }
            //////////////////////

            //Alerts///////////
            IQueryable<Alert> queryAlerts;

            if (input.NorthEastBoundary != null && input.SouthWestBoundary != null)
            {
                Geometry boundingBox = GeometryHelper.GetPolygonFromBoundaries(input.SouthWestBoundary, input.NorthEastBoundary);
                queryAlerts = _geoJsonBulkRepository.GetAlerts(input.StartDate.Value, input.EndDate.Value, boundingBox);
            }
            else
                queryAlerts = _alertManager.GetAlerts(input.StartDate.Value, input.EndDate.Value);

            hasPermission = _permissionChecker.IsGranted(_session.Roles, AppPermissions.MapRequests.MapRequest_CanSeeCrossOrganization);
            if (!hasPermission)
            {
                if (_session.LoggedUserPerson.OrganizationId.HasValue)
                    queryMapRequests = queryMapRequests.DataOwnership(new List<int>() { _session.LoggedUserPerson.OrganizationId.Value });
            }
            //////////////////////

            //Communications///////////
            IQueryable<Communication> queryCommunications;

            if (input.NorthEastBoundary != null && input.SouthWestBoundary != null)
            {
                Geometry boundingBox = GeometryHelper.GetPolygonFromBoundaries(input.SouthWestBoundary, input.NorthEastBoundary);
                queryCommunications = _geoJsonBulkRepository.GetCommunications(input.StartDate.Value, input.EndDate.Value, boundingBox);
            }
            else
                queryCommunications = _communicationManager.GetCommunications(start, end);
            //Admin can see everything
            hasPermission = _permissionChecker.IsGranted(_session.Roles, AppPermissions.Communications.Communication_CanSeeCrossOrganization);

            if (!hasPermission)
            {
                List<int> orgIdListComm = null;
                if (person.OrganizationId.HasValue)
                {
                    orgIdListComm = new List<int>() { person.OrganizationId.Value };
                    var p = await _personManager.GetPersonByIdAsync(person.Id);
                    Organization parent = await _organizationManager.GetParentOrganizationAsync(p.Organization.ParentId);
                    if (parent != null)
                    {
                        orgIdListComm ??= new List<int>();
                        orgIdListComm.Add(parent.Id);
                    }
                }
                queryCommunications = queryCommunications.DataOwnership(orgIdListComm, person);
            }
            //////////////////////

            var activations = _geoJsonBulkRepository.GetPersonActivations(start, end, ActionStatusType.Active);
            return new GetStatisticsOutput()
            {
                ReportsByHazard = queryReport
                                    .GroupBy(r => r.HazardString)
                                    .Select(g => new GroupDto()
                                    {
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
                Persons = actions,
                ActivationsByDay = new Dictionary<ActionStatusType, List<ActivationDto>>() {
                    {
                        ActionStatusType.Active, ObjectMapper.Map<List<ActivationDto>>(activations)
                    }
                },
                CommunicationsByRestriction = queryCommunications
                                    .GroupBy(c => c.RestrictionString)
                                    .Select(g => new GroupDto()
                                    {
                                        Id = g.Key,
                                        Label = g.Key,
                                        Value = g.Count()
                                    })
                                    .ToList(),
                MapRequestByType = queryMapRequests
                                    .GroupBy(mr => mr.TypeString)
                                    .Select(g => new GroupDto()
                                    {
                                        Id = g.Key,
                                        Label = g.Key,
                                        Value = g.Count()
                                    })
                                    .ToList(),
                AlertsByRestriction = queryAlerts
                                        .GroupBy(a => a.Restriction)
                                        .Select(g => new GroupDto()
                                        {
                                            Id = g.Key,
                                            Label = g.Key,
                                            Value = g.Count()
                                        })
                                        .ToList()
            };
        }
    }
}
