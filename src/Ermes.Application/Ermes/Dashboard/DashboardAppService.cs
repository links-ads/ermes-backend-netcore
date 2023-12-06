using Abp.Localization;
using Abp.SensorService;
using Abp.UI;
using Ermes.Actions.Dto;
using Ermes.Alerts;
using Ermes.Attributes;
using Ermes.Authorization;
using Ermes.Communications;
using Ermes.Dashboard.Dto;
using Ermes.Enums;
using Ermes.GeoJson;
using Ermes.Helpers;
using Ermes.Linq.Extensions;
using Ermes.MapRequests;
using Ermes.Missions;
using Ermes.Organizations;
using Ermes.Persons;
using Ermes.Reports;
using Ermes.Stations.Dto;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        private readonly SensorServiceManager _sensorServiceManager;
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
                AlertManager alertManager,
                SensorServiceManager sensorServiceManager
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
            _sensorServiceManager = sensorServiceManager;
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

            Geometry boundingBox = null;
            if (input.NorthEastBoundary != null && input.SouthWestBoundary != null)
                boundingBox = GeometryHelper.GetPolygonFromBoundaries(input.SouthWestBoundary, input.NorthEastBoundary);

            //TODO: could be restored, but check needs to be implemented client-side
            //if (timestap.TotalDays > 30)
            //    throw new UserFriendlyException(L("TimeWindowSizeError"));

            //Reports////////////
            IQueryable<Report> queryReport = boundingBox != null ? _geoJsonBulkRepository.GetReports(start, end, boundingBox) : _reportManager.GetReports(start, end);

            var hasPermission = _permissionChecker.IsGranted(_session.Roles, AppPermissions.Reports.Report_CanSeeCrossOrganization);
            if (!hasPermission)
                queryReport = queryReport.DataOwnership(person.OrganizationId.HasValue ? new List<int>() { person.OrganizationId.Value } : null);
            /////////////////////

            //Missions///////////
            IQueryable<Mission> queryMission = boundingBox != null ? _geoJsonBulkRepository.GetMissions(start, end, boundingBox) : _missionManager.GetMissions(start, end);

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
            var items = _geoJsonBulkRepository.GetPersonActions(start, end, orgIdList, null, null, null, boundingBox, personName, null, _languageManager.CurrentLanguage.Name);
            var deserialized = JsonConvert.DeserializeObject<GetActionsOutput>(items);
            var actions = deserialized != null && deserialized.PersonActions != null ? deserialized.PersonActions : new List<PersonActionDto>();
            ////////////////////

            //Map Requests///////////
            IQueryable<MapRequest> queryMapRequests = boundingBox != null ? _geoJsonBulkRepository.GetMapRequests(start, end, boundingBox) : _mapRequestManager.GetMapRequests(start, end);

            hasPermission = _permissionChecker.IsGranted(_session.Roles, AppPermissions.MapRequests.MapRequest_CanSeeCrossOrganization);
            if (!hasPermission)
            {
                if (_session.LoggedUserPerson.OrganizationId.HasValue)
                    queryMapRequests = queryMapRequests.DataOwnership(new List<int>() { _session.LoggedUserPerson.OrganizationId.Value });
            }
            //////////////////////

            //Alerts///////////
            IQueryable<Alert> queryAlerts = boundingBox != null ? _geoJsonBulkRepository.GetAlerts(start, end, boundingBox) : _alertManager.GetAlerts(start, end);
            //////////////////////

            //Communications///////////
            IQueryable<Communication> queryCommunications = boundingBox != null ? _geoJsonBulkRepository.GetCommunications(start, end, boundingBox) : _communicationManager.GetCommunications(start, end);

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

            //Stations///////////
            var stations = new List<StationDto>();

            try
            {
                var fullStationList = await _sensorServiceManager.GetStations();
                if (input.SouthWestBoundary != null && input.NorthEastBoundary != null)
                    fullStationList = fullStationList.Where(a => boundingBox.Contains(GeometryHelper.GetPointFromCoordinates(a.Location.Coordinates))).ToList();

                foreach (var station in fullStationList)
                {
                    var summary = await _sensorServiceManager.GetStationSummary(station.Id, start, end);
                    var dto = ObjectMapper.Map<StationDto>(summary);
                    dto.IsOnline = dto.Sensors != null && dto.Sensors.Count > 0 && dto.Sensors.Any(a =>
                        {
                            var measure = a.Measurements.OrderByDescending(b => b.Timestamp).FirstOrDefault();
                            return measure != null && measure.Timestamp > DateTime.UtcNow.AddMinutes(-10);
                        }
                    );
                    //information about sensors not needed here
                    dto.Sensors = null;
                    stations.Add(dto);
                }
            }
            catch (Exception e){
                Logger.Warn(e.Message);
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
                                        .ToList(),
                Stations = stations
            };
        }
    }
}
