using Abp.Application.Services.Dto;
using Abp.BackgroundJobs;
using Abp.Json;
using Abp.Linq.Extensions;
using Abp.UI;
using Ermes.Attributes;
using Ermes.Authorization;
using Ermes.Dto;
using Ermes.Dto.Datatable;
using Ermes.Enums;
using Ermes.EventHandlers;
using Ermes.Linq.Extensions;
using Ermes.Organizations;
using Ermes.Persons;
using Ermes.Teams.Dto;
using Ermes.Users.Dto;
using Microsoft.EntityFrameworkCore;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Ermes.Teams
{
    [ErmesAuthorize]
    public class TeamsAppService : ErmesAppServiceBase, ITeamsAppService
    {
        private readonly ErmesAppSession _session;
        private readonly TeamManager _teamManager;
        private readonly PersonManager _personManager;
        private readonly OrganizationManager _organizationManager;
        private readonly ErmesPermissionChecker _permissionChecker;
        private readonly IBackgroundJobManager _backgroundJobManager;
        public TeamsAppService(
                    TeamManager teamManager,
                    PersonManager personManager,
                    OrganizationManager organizationManager,
                    ErmesPermissionChecker permissionChecker,
                    IBackgroundJobManager backgroundJobManager,
                    ErmesAppSession session)
        {
            _session = session;
            _teamManager = teamManager;
            _personManager = personManager;
            _organizationManager = organizationManager;
            _permissionChecker = permissionChecker;
            _backgroundJobManager = backgroundJobManager;
        }
        [OpenApiOperation("Get Team",
            @"
                Get the Team data given a team id.
                Input: TeamId
                Output: TeamOutputDto object
            "
        )]
        public virtual async Task<TeamOutputDto> GetTeamById(IdInput<int> input)
        {
            Team team = await CheckTeamInputValidity(input.Id);
            TeamOutputDto teamOutput = ObjectMapper.Map<TeamOutputDto>(team);
            teamOutput.Members = _personManager.Persons.Where(p => p.TeamId == input.Id).Select(p => ObjectMapper.Map<ListUsernamesDto>(p)).ToList();
            return teamOutput;
        }
        [OpenApiOperation("Get Teams",
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
                Output: list of TeamOutputDto elements

                N.B.: A person has visibility only on team belonging to his organization
            "
        )]
        public virtual async Task<DTResult<TeamOutputDto>> GetTeams(GetTeamsInput input)
        {
            PagedResultDto<TeamOutputDto> result = await InternalGetTeams(input);
            return new DTResult<TeamOutputDto>(input.Draw, result.TotalCount, result.Items.Count, result.Items.ToList());
        }

        private async Task<PagedResultDto<TeamOutputDto>> InternalGetTeams(GetTeamsInput input, bool filterByOrganization = true)
        {
            PagedResultDto<TeamOutputDto> result = new PagedResultDto<TeamOutputDto>();
            var person = _session.LoggedUserPerson;

            IQueryable<Team> query = _teamManager.Teams.Include(t => t.Organization);

            filterByOrganization = !(_permissionChecker.IsGranted(_session.Roles, AppPermissions.Teams.Team_CanViewAll));

            //citizen not allowed to see teams
            if (filterByOrganization && person != null && !person.OrganizationId.HasValue)
                throw new UserFriendlyException(L("MissingPermission", AppPermissions.Teams.Team_CanViewAll));

            if (filterByOrganization && person != null && person.OrganizationId.HasValue)
                query = query.DataOwnership(new List<int>() { person.OrganizationId.Value });

            query = query.DTFilterBy(input);

            result.TotalCount = await query.CountAsync();

            if (input?.Order != null && input.Order.Count == 0)
            {
                query = query.OrderByDescending(a => a.Name);
                query = query.PageBy(input);
            }
            else
            {
                query = query.DTOrderedBy(input).PageBy(input);
            }

            List<Team> items = await query.ToListAsync();
            List<TeamOutputDto> resultItems = ObjectMapper.Map<List<TeamOutputDto>>(items);
            result.Items = resultItems.GroupJoin(_personManager.Persons, t => t.Id, p => p.TeamId, (t, p) => { t.Members = p.Select(p => ObjectMapper.Map<ListUsernamesDto>(p)).ToList(); return t; }).ToList();
            return result;
        }


        [OpenApiOperation("Create or Update Team",
            @"
                Create or update the data for a team.
                Input: CreateUpdateTeamInput object
                Output: Id of the affected team
                
                N.B. OrganizationId must not be set. It will be automatically set equal to the organization id
                     of the current logged user. Only users with special permissions (Admin) can set this field
            "
        )]
        public virtual async Task<int> CreateOrUpdateTeam(CreateUpdateTeamInput input)
        {
            //Update
            if (input.Team.Id != 0)
                return await UpdateTeam(input.Team);
            else
                return await CreateTeam(input.Team);
        }

        private async Task<int> UpdateTeam(TeamDto input)
        {
            Team team = _teamManager.GetTeamById(input.Id);
            if (team == null)
                throw new UserFriendlyException(L("InvalidEntityId", input.Id, "Team"));


            if (input.OrganizationId.HasValue)
            {
                if (!_permissionChecker.IsGranted(_session.Roles, AppPermissions.Teams.Team_CanCreateTeamCrossOrganization))
                {
                    //citizen
                    if (!_session.LoggedUserPerson.OrganizationId.HasValue)
                        throw new UserFriendlyException(L("MissingPermission"));

                    //cannot update team cross-org
                    if (input.OrganizationId.Value != _session.LoggedUserPerson.OrganizationId.Value)
                        throw new UserFriendlyException(L("MissingPermission"));

                    //invalid input
                    if (input.OrganizationId.Value != team.OrganizationId)
                        throw new UserFriendlyException(L("InvalidOrganizationId", input.OrganizationId.Value));
                }
            }
            else
            {
                if (team.Organization != null)
                {
                    var organization = await _organizationManager.GetOrganizationByIdAsync(team.Organization.Id);
                    if (organization == null)
                        throw new UserFriendlyException(L("InvalidOrganizationId", team.Organization.Id));
                    if (_session.LoggedUserPerson.OrganizationId != team.Organization.Id && _session.LoggedUserPerson.OrganizationId != team.Organization.ParentId)
                        throw new UserFriendlyException(L("InvalidOrganizationId"));
                    input.OrganizationId = organization.Id;
                }
            }

            ObjectMapper.Map<TeamDto, Team>(input, team);
            return input.Id;
        }

        private async Task<int> CreateTeam(TeamDto input)
        {
            Team team = ObjectMapper.Map<Team>(input);
            var person = _session.LoggedUserPerson;

            if (!input.OrganizationId.HasValue || input.OrganizationId.Value == 0)
                throw new UserFriendlyException(L("OrganizationRequiredForOperation", input.Id, "Team"));

            var org = await _organizationManager.GetOrganizationByIdAsync(input.OrganizationId.Value);
            if (org == null)
                throw new UserFriendlyException(L("InvalidOrganizationId", input.OrganizationId.Value));


            //By default, a user can create Team for his own org or for one of the child organizations
            if (!person.OrganizationId.HasValue) //Admin user
            {
                if (_permissionChecker.IsGranted(_session.Roles, AppPermissions.Teams.Team_CanCreateTeamCrossOrganization))
                    team.OrganizationId = input.OrganizationId.Value;
                else
                    throw new UserFriendlyException(L("MissingPermission"));
            }
            else
            {
                if (person.OrganizationId.Value == org.Id || person.OrganizationId.Value == org.ParentId)
                    team.OrganizationId = input.OrganizationId.Value;
                else
                    throw new UserFriendlyException(L("MissingPermission"));
            }

            return await _teamManager.InsertTeamAsync(team);
        }

        [OpenApiOperation("Delete Teams",
            @"
                De-associate all members for a team and delete it.
                Input: SelectTeamInput object, with the id of the affected team
                Output: True if success, or exception
            "
        )]
        public virtual async Task<bool> DeleteTeam(IdInput<int> input)
        {
            // TODO-Security: Add Same Organization Access Control
            // De-associate persons from team
            var team = await CheckTeamInputValidity(input.Id);
            _personManager.Persons.Where(p => p.TeamId == input.Id).ToList().ForEach(p => p.Team = null);
            await _teamManager.DeleteTeamAsync(input.Id);
            return true;
        }
        [OpenApiOperation("Set Team Members",
            @"
                Replace the current members for a team with those specified in input.
                Input: SetTeamMembersInput object, with the id of the affected team and the Person/User Ids of the new members.
                Output: True if success, or exception
            "
        )]
        public virtual async Task<bool> SetTeamMembers(SetTeamMembersInput input)
        {
            Team team = await CheckTeamInputValidity(input.TeamId);
            // De-associate persons no longer in team (EF should implicitly convert the contains in a SQL IN clause)
            var deAssociated = _personManager.Persons.Where(p => p.TeamId == input.TeamId).Where(p => !input.MembersGuids.Contains(p.FusionAuthUserGuid)).ToList();
            deAssociated.ForEach(p => p.Team = null);
            // Get persons that will be members of team
            List<Person> affectedPersons = _personManager.Persons.Where(p => input.MembersGuids.Contains(p.FusionAuthUserGuid)).ToList();
            List<Guid> associated = new List<Guid>();
            // Verify that at least one has mission management permissions
            /*List<int> memberRoles = _permissionManager.PersonRoles.Where(pr => input.MembersIds.Contains(pr.PersonId))
                        .Select(pr => pr.RoleId)
                        .Distinct()
                        .ToList();
            bool missionManagementPermissionPresent = _permissionManager.Permissions
                .Where(pe => memberRoles.Contains(pe.RoleId))
                .Any(pe => pe.Name == AppPermissions.Missions_Management);
            
             if(!missionManagementPermissionPresent)
                throw new UserFriendlyException();*/

            // Associate persons to team
            affectedPersons.ForEach(p =>
            {
                if (p.OrganizationId != team.OrganizationId)
                    throw new UserFriendlyException(L("OrganizationMismatch", "Team", team.Id, "Person", p.Id));
                if (p.TeamId != input.TeamId)//otherwise skip as already associated (do not send notification to persons that were already associated to this team)
                {
                    p.Team = team;
                    associated.Add(p.FusionAuthUserGuid);
                }
            });
            // Check if it was passed some unexistent user
            input.MembersGuids.RemoveAll(pi => affectedPersons.Select(p => p.FusionAuthUserGuid).Contains(pi));
            if (input.MembersGuids.Count > 0)
            {
                throw new UserFriendlyException(L("UnexistentEntities", "Person", input.MembersGuids.ToJsonString()));
            }

            TeamNotificationDto tn = new TeamNotificationDto()
            {
                Name = team.Name,
                Guids = associated
            };
            NotificationEvent<TeamNotificationDto> notification = new NotificationEvent<TeamNotificationDto>(team.Id,
                _session.UserId.Value,
                tn,
                EntityWriteAction.TeamAssociation);
            await _backgroundJobManager.EnqueueEventAsync(notification);

            //Send notification to all persons that are no longer part of this team
            tn.Guids = deAssociated.Select(a => a.FusionAuthUserGuid).ToList();

            notification = new NotificationEvent<TeamNotificationDto>(team.Id,
                _session.UserId.Value,
                tn,
                EntityWriteAction.TeamDissociation);

            await _backgroundJobManager.EnqueueEventAsync(notification);

            return true;
        }

        private async Task<Team> CheckTeamInputValidity(int teamId)
        {
            Team team = _teamManager.GetTeamById(teamId);
            if (team == null)
                throw new UserFriendlyException(L("InvalidEntityId", teamId, "Team"));
            var hasPermission = _permissionChecker.IsGranted(_session.Roles, AppPermissions.Teams.Team_CanViewAll);
            if (!hasPermission)
            {
                if (_session.LoggedUserPerson.OrganizationId.HasValue && _session.LoggedUserPerson.OrganizationId.Value != team.OrganizationId && _session.LoggedUserPerson.OrganizationId != team.Organization.ParentId)
                    throw new UserFriendlyException(L("EntityOutsideOrganization", team.Name));
            }

            return team;
        }

    }
}
