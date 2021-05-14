using Ermes.Attributes;
using Ermes.Persons;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq;
using System.Threading.Tasks;
using Ermes.Teams.Dto;
using Abp.UI;
using Abp.Application.Services.Dto;
using Ermes.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Abp.Linq.Extensions;
using Ermes.Dto.Datatable;
using Ermes.Users.Dto;
using Ermes.Dto;
using Abp.Json;
using Ermes.Permissions;
using Ermes.Authorization;
using Ermes.Organizations;

namespace Ermes.Teams
{
    [ErmesAuthorize]
    public class TeamsAppService: ErmesAppServiceBase, ITeamsAppService
    {
        private readonly ErmesAppSession _session;
        private readonly TeamManager _teamManager;
        private readonly PersonManager _personManager;
        private readonly OrganizationManager _organizationManager;
        private readonly ErmesPermissionChecker _permissionChecker;
        public TeamsAppService(
                    TeamManager teamManager, 
                    PersonManager personManager,
                    OrganizationManager organizationManager,
                    ErmesPermissionChecker permissionChecker,
                    ErmesAppSession session)
        {
            _session = session;
            _teamManager = teamManager;
            _personManager = personManager;
            _organizationManager = organizationManager;
            _permissionChecker = permissionChecker;
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
            else {
                if (_session.LoggedUserPerson.OrganizationId != team.OrganizationId)
                    throw new UserFriendlyException(L("InvalidOrganizationId"));
                input.OrganizationId = _session.LoggedUserPerson.OrganizationId;
            }

            ObjectMapper.Map<TeamDto, Team>(input, team);
            return input.Id;
        }

        private async Task<int> CreateTeam(TeamDto input)
        {
            Team team = ObjectMapper.Map<Team>(input);
            var person = _session.LoggedUserPerson;
            if (person.OrganizationId.HasValue)
                team.OrganizationId = person.OrganizationId.Value;
            else
            {
                if (_permissionChecker.IsGranted(_session.Roles, AppPermissions.Teams.Team_CanCreateTeamCrossOrganization))
                {
                    if (input.OrganizationId.HasValue && input.OrganizationId.Value > 0)
                    {
                        var org = await _organizationManager.GetOrganizationByIdAsync(input.OrganizationId.Value);
                        if(org == null)
                          throw new UserFriendlyException(L("InvalidOrganizationId", input.OrganizationId.Value));
                        else
                            team.OrganizationId = input.OrganizationId.Value;
                    }
                    else
                        throw new UserFriendlyException(L("OrganizationRequiredForOperation", input.Id, "Team"));
                }
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
            _personManager.Persons.Where(p => p.TeamId == input.TeamId).Where(p=> !input.MembersIds.Contains(p.Id)).ToList().ForEach(p => p.Team = null);
            // Get persons that will be members of team
            List<Person> affectedPersons = _personManager.Persons.Where(p => input.MembersIds.Contains(p.Id)).ToList();

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
                if(p.TeamId != input.TeamId) //otherwise skip as already associated
                    p.Team = team;
            });
            // Check if it was passed some unexistent user
            input.MembersIds.RemoveAll(pi => affectedPersons.Select(p => p.Id).Contains(pi));
            if(input.MembersIds.Count > 0)
            {
                throw new UserFriendlyException(L("UnexistentEntities", "Person", input.MembersIds.ToJsonString()));
            }
            return true;
        }

        private async Task<Team> CheckTeamInputValidity(int teamId)
        {
            Team team = _teamManager.GetTeamById(teamId);
            if (team == null)
                throw new UserFriendlyException(L("InvalidEntityId", teamId, "Team"));
            if (_session.LoggedUserPerson.OrganizationId.HasValue && _session.LoggedUserPerson.OrganizationId.Value != team.OrganizationId)
                throw new UserFriendlyException(L("EntityOutsideOrganization", team.Name));

            return team;
        }

    }
}
