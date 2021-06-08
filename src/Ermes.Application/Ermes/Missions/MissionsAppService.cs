using Abp.Application.Services.Dto;
using Abp.Linq.Extensions;
using Abp.UI;
using Ermes.Attributes;
using Ermes.Authorization;
using Ermes.Dto;
using Ermes.Dto.Datatable;
using Ermes.Dto.Spatial;
using Ermes.Enums;
using Ermes.Helpers;
using Ermes.Linq.Extensions;
using Ermes.Missions.Dto;
using Ermes.Persons;
using Ermes.Reports;
using Ermes.Reports.Dto;
using Ermes.Teams;
using Ermes.Teams.Dto;
using Ermes.Notifiers;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.IO;
using NpgsqlTypes;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Uow;
using Abp.BackgroundJobs;
using Ermes.EventHandlers;
using NetTopologySuite.Geometries;
using Ermes.GeoJson;
using System.Net;

namespace Ermes.Missions
{
    [ErmesAuthorize]
    public class MissionsAppService : ErmesAppServiceBase, IMissionsAppService
    {
        private readonly ErmesAppSession _session;
        private readonly MissionManager _missionManager;
        private readonly PersonManager _personManager;
        private readonly ReportManager _reportManager;
        private readonly TeamManager _teamManager;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IGeoJsonBulkRepository _geoJsonBulkRepository;
        private readonly ErmesPermissionChecker _permissionChecker;


        public MissionsAppService(
                ErmesAppSession session,
                MissionManager missionManager,
                PersonManager personManager,
                ReportManager reportManager,
                TeamManager teamManager,
                IBackgroundJobManager backgroundJobManager,
                ErmesPermissionChecker permissionChecker,
                IGeoJsonBulkRepository geoJsonBulkRepository
        )
        {
            _session = session;
            _missionManager = missionManager;
            _personManager = personManager;
            _reportManager = reportManager;
            _teamManager = teamManager;
            _backgroundJobManager = backgroundJobManager;
            _geoJsonBulkRepository = geoJsonBulkRepository;
            _permissionChecker = permissionChecker;
        }

        #region Private
        private async Task<PagedResultDto<MissionDto>> InternalGetMissions(GetMissionsInput input, bool filterByOrganization = true)
        {
            PagedResultDto<MissionDto> result = new PagedResultDto<MissionDto>();

            IQueryable<Mission> query;
            input.StartDate = input.StartDate.HasValue ? input.StartDate : DateTime.MinValue;
            input.EndDate = input.EndDate.HasValue ? input.EndDate : DateTime.MaxValue;

            if (input.NorthEastBoundary != null && input.SouthWestBoundary != null)
            {
                Geometry boundingBox = GeometryHelper.GetPolygonFromBoundaries(input.SouthWestBoundary, input.NorthEastBoundary);
                query = _geoJsonBulkRepository.GetMissions(input.StartDate.Value, input.EndDate.Value, boundingBox);
            }
            else
                query = _missionManager.Missions.Where(a => new NpgsqlRange<DateTime>(input.StartDate.Value, input.EndDate.Value).Contains(a.Duration));

            if (input.Status != null && input.Status.Count > 0)
            {
                //input.Status.Contains throw an exception
                //I need to go through the strings rather then the enum
                var list = input.Status.Select(a => a.ToString()).ToList();
                query = query.Where(a =>list.Contains(a.CurrentStatusString));
            }

            query = query.DTFilterBy(input);

            var currentUserPerson = _session.LoggedUserPerson;

            //List of Missions available only for pro users
            var hasPermission = _permissionChecker.IsGranted(_session.Roles, AppPermissions.Missions.Mission_CanSeeCrossOrganization);
            if (!hasPermission)
            {
                if (filterByOrganization && currentUserPerson.OrganizationId.HasValue)
                    query = query.DataOwnership(new List<int>() { currentUserPerson.OrganizationId.Value });
                else
                    return result;
            }

            result.TotalCount = await query.CountAsync();

            if (input?.Order != null && input.Order.Count == 0)
            {
                query = query.OrderByDescending(a => a.Duration.LowerBound);
                query = query.PageBy(input);
            }
            else
            {
                query = query.DTOrderedBy(input)
                    .PageBy(input);
            }

            List<Mission> items = await query.ToListAsync();

            result.Items = items.GroupJoin(_reportManager.Reports, m => m.Id, r => r.RelativeMissionId, (m, r) => {
                MissionDto md = ObjectMapper.Map<MissionDto>(m);
                md.Reports = r.Select(rx => {
                    ReportDto report = ObjectMapper.Map<ReportDto>(rx);
                    report.IsEditable = (currentUserPerson.Id == rx.CreatorUserId);
                    return report;
                }).ToList();
                return md; 
            }).ToList();

            return result;
        }

        private async Task VerifyOrganizationAndCoordinators(MissionDto newMissionData, Mission oldMission)
        {
            int? currentUserOrganizationId = _session.LoggedUserPerson.OrganizationId;
            if (!currentUserOrganizationId.HasValue)
                throw new UserFriendlyException(L("OrganizationRequiredForOperation"));

            if(oldMission != null && oldMission.OrganizationId != currentUserOrganizationId)
                throw new UserFriendlyException(L("OrganizationMismatch"));

            if (newMissionData.CoordinatorPersonId.HasValue && newMissionData.CoordinatorTeamId.HasValue)
                throw new UserFriendlyException(L("OnlyOneMissionCoordinator"));

            if (newMissionData.CoordinatorPersonId.HasValue && (oldMission == null || newMissionData.CoordinatorPersonId != oldMission.CoordinatorPersonId))
            {
                Person coord = await _personManager.GetPersonByIdAsync(newMissionData.CoordinatorPersonId.Value);
                if (coord == null || coord.OrganizationId != currentUserOrganizationId)
                    throw new UserFriendlyException(L("InvalidEntityId", newMissionData.CoordinatorPersonId.Value, "Coordinator-Person"));
            }
            else if (newMissionData.CoordinatorTeamId.HasValue && (oldMission == null || newMissionData.CoordinatorTeamId != oldMission.CoordinatorTeamId))
            {
                Team coord = await _teamManager.GetTeamByIdAsync(newMissionData.CoordinatorTeamId.Value);
                if (coord == null || coord.OrganizationId != currentUserOrganizationId)
                    throw new UserFriendlyException(L("InvalidEntityId", newMissionData.CoordinatorTeamId.Value, "Coordinator-Team"));
            }
        }

        private async Task<int> UpdateMissionAsync(FeatureDto<MissionDto> featureDto)
        {
            var mission = await CheckInputValidity(featureDto.Properties);

            await VerifyOrganizationAndCoordinators(featureDto.Properties, mission);

            ObjectMapper.Map(featureDto.Properties, mission);

            await CurrentUnitOfWork.SaveChangesAsync();

            NotificationEvent<MissionNotificationDto> notification = new NotificationEvent<MissionNotificationDto>(mission.Id,
                _session.UserId.Value,
                ObjectMapper.Map<MissionNotificationDto>(mission),
                EntityWriteAction.Update);
            await _backgroundJobManager.EnqueueEventAsync(notification);

            return mission.Id;
        }

        private async Task<Mission> CheckInputValidity(MissionDto input)
        {
            Mission mission;
            if (input.Id > 0)
            {
                mission = await _missionManager.GetMissionByIdAsync(input.Id);
                if (mission == null)
                    throw new UserFriendlyException(L("InvalidMissionId", input.Id));

                if(mission.OrganizationId != _session.LoggedUserPerson.OrganizationId)
                    throw new UserFriendlyException(L("EntityOutsideOrganization"));
            }
            else
                mission = ObjectMapper.Map<Mission>(input);

            if (input.CoordinatorPersonId.HasValue && input.CoordinatorTeamId.HasValue)
                throw new UserFriendlyException(L("OnlyOneMissionCoordinator"));

            if (input.CoordinatorPersonId.HasValue)
            {
                var coordinator = await _personManager.GetPersonByIdAsync(input.CoordinatorPersonId.Value);
                if (coordinator == null || coordinator.OrganizationId != _session.LoggedUserPerson.OrganizationId)
                    throw new UserFriendlyException(L("InvalidCoordinatorId", input.CoordinatorPersonId.Value));
            }

            if (input.CoordinatorTeamId.HasValue)
            {
                var team = await _teamManager.GetTeamByIdAsync(input.CoordinatorTeamId.Value);
                if (team == null || team.OrganizationId != _session.LoggedUserPerson.OrganizationId)
                    throw new UserFriendlyException(L("InvalidTeamId", input.CoordinatorTeamId.Value));
            }

            return mission;

        }

        private async Task<int> CreateMissionAsync(FeatureDto<MissionDto> featureDto)
        {
            var newMission = await CheckInputValidity(featureDto.Properties);
            // Associate the mission to the organization of the current user
            if (!_session.LoggedUserPerson.OrganizationId.HasValue)
                throw new UserFriendlyException(L("OrganizationRequiredForOperation"));
            newMission.OrganizationId = _session.LoggedUserPerson.OrganizationId.Value;

            if (featureDto.FullGeometry.IsValid)
                newMission.AreaOfInterest = featureDto.FullGeometry;
            else
                throw new UserFriendlyException(L("InvalidAOI"));

            newMission.Id = await _missionManager.InsertMissionAsync(newMission);
            await CurrentUnitOfWork.SaveChangesAsync();

            NotificationEvent<MissionNotificationDto> notification = new NotificationEvent<MissionNotificationDto>(newMission.Id,
                _session.UserId.Value,
                ObjectMapper.Map<MissionNotificationDto>(newMission),
                EntityWriteAction.Create);
            await _backgroundJobManager.EnqueueEventAsync(notification);

            return newMission.Id;
        }

        private async Task<Mission> GetMissionAsync(int missionId)
        {
            var mission = await _missionManager.GetMissionByIdAsync(missionId);
            if (mission == null)
                throw new UserFriendlyException(L("InvalidMissionId", missionId));

            return mission;
        }
        #endregion

        [OpenApiOperation("Create or Update a Mission", 
            @"
                Input: GeoJson feature, with MissionDto element in Properties field
                If the input contains an Id > 0, an update is performed, otherwise a new mission is created
                Output: the Id the mission that has been created/updated
                This operation will trigger notifications
            "
        )]
        [UnitOfWork(false)]
        public virtual async Task<int> CreateOrUpdateMission(CreateOrUpdateMissionInput input)
        {
            if (input.Feature.Properties.Id == 0)
                return await CreateMissionAsync(input.Feature);
            else
                return await UpdateMissionAsync(input.Feature);
        }

        [OpenApiOperation("Get Mission by Id",
             @"
                Input: 
                        - Id: the id of the mission to be retrived
                        - IncludeArea: if true, the response will contain the geometry of the mission
                Output: GeoJson feature, with MissionDto element in Properties field
                Exception: invalid id of the mission
             "
        )]
        public virtual async Task<GetEntityByIdOutput<MissionDto>> GetMissionById(GetEntityByIdInput<int> input)
        {
            var mission = await GetMissionAsync(input.Id);
            if (mission.OrganizationId != _session.LoggedUserPerson.OrganizationId)
                throw new UserFriendlyException(L("EntityOutsideOrganization"));

            var writer = new GeoJsonWriter();
            var res = new GetEntityByIdOutput<MissionDto>()
            {
                Feature = new FeatureDto<MissionDto>()
                {
                    Geometry = input.IncludeArea ? writer.Write(mission.AreaOfInterest) : null,
                    Properties = ObjectMapper.Map<MissionDto>(mission)
                }
            };

            res.Feature.Properties.Reports = _reportManager.Reports.Where(r => r.RelativeMissionId == input.Id).Select(r => ObjectMapper.Map<ReportDto>(r)).ToList();
            res.Feature.Properties.Reports.ForEach(r => r.IsEditable = (_session.UserId == r.CreatorId));

            return res;
        }


        [OpenApiOperation("Get Missions",
            @"
                This is a server-side paginated API
                Input: use the following properties to filter result list:
                    - Draw: Draw counter. This is used by DataTables to ensure that the Ajax returns from server-side processing requests are drawn in sequence by 
                        DataTables (Ajax requests are asynchronous and thus can return out of sequence)
                    - MaxResultCount: number of records that the table can display in the current draw (must be >= 0)
                    - SkipCount: paging first record indicator. This is the start point in the current data set (0 index based - i.e. 0 is the first record)
                    - Search: 
                               - value: global search value
                               - regex: true if the global filter should be treated as a regular expression for advanced searching, false otherwise
                    - Order (is a list, for multi-column sorting):
                                - column: name of the column to which sorting should be applied
                                - dir: sorting direction
                In addition to pagination parameters, there are additional properties for mission filtering:
                    - Status: list of MissionStatus of interest
                    - StartDate and EndDate to define a time window of interest
                    - SouthWestBoundary: bottom-left corner of the bounding box for a spatial query. (optional) (to be filled together with NorthEast property)
                    - NorthEastBoundary: top-right corner of the bounding box for a spatial query format. (optional) (to be filled together with SouthWest property)
                Output: list of MissionDto elements

                N.B.: A person has visibility only on missions belonging to his organization
            "
        )]
        public virtual async Task<DTResult<MissionDto>> GetMissions(GetMissionsInput input)
        {
            PagedResultDto<MissionDto> result = await InternalGetMissions(input);
            return new DTResult<MissionDto>(input.Draw, result.TotalCount, result.Items.Count, result.Items.ToList());
        }

        [OpenApiOperation("Update the Status of a Mission",
            @"
                Input: the id of the mission to be updated, and the new status
                Output: true if the operation has been excuted successfully
                Exception: invalid new status. 
                This operation will trigger notifications
            "
        )]
        [UnitOfWork(false)]
        public virtual async Task<bool> UpdateMissionStatus(UpdateMissionStatusInput input)
        {
            Mission mission = await GetMissionAsync(input.MissionId);
            if (mission.OrganizationId != _session.LoggedUserPerson.OrganizationId)
                throw new UserFriendlyException(L("EntityOutsideOrganization"));
            if (mission.CoordinatorPersonId.HasValue && mission.CoordinatorPersonId.Value != _session.LoggedUserPerson.Id)
                throw new UserFriendlyException(L("Forbidden_InsufficientRoleOrUnassociatedEntity"));
            if(mission.CoordinatorTeamId.HasValue && mission.CoordinatorTeamId.Value != _session.LoggedUserPerson.TeamId)
                throw new UserFriendlyException(L("Forbidden_InsufficientRoleOrUnassociatedEntity"));

            if (_missionManager.CheckNewStatus(mission.CurrentStatus, input.Status))
            {
                mission.CurrentStatus = input.Status;

                //Need to update status before sending the notification
                await CurrentUnitOfWork.SaveChangesAsync();

                NotificationEvent<MissionNotificationDto> notification = new NotificationEvent<MissionNotificationDto>(mission.Id,
                    _session.UserId.Value,
                    ObjectMapper.Map<MissionNotificationDto>(mission),
                    EntityWriteAction.StatusChange);
                await _backgroundJobManager.EnqueueEventAsync(notification);

                return true;
            }
            else
                throw new UserFriendlyException((int)HttpStatusCode.BadRequest, L("InvalidMissionUpdateStatus", input.Status));
        }

        [OpenApiOperation("Delete a Mission",
            @"
                Input: the id of the mission to be deleted
                Output: true if the operation has been excuted successfully, false otherwise
                Exception: invalid mission Id 
            "
        )]
        public virtual async Task<bool> DeleteMission(IdInput<int> input)
        {
            var mission = await GetMissionAsync(input.Id);
            if(mission.CurrentStatus != MissionStatusType.Deleted)
            {
                mission.CurrentStatus = MissionStatusType.Deleted;
                return true;
            }

            return false;
        }
    }
}
