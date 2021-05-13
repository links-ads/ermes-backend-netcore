using Abp.Application.Services;
using Abp.UI;
using Ermes.Auth.Dto;
using Ermes.Enums;
using Ermes.Missions;
using Ermes.Missions.Dto;
using Ermes.Organizations;
using Ermes.Organizations.Dto;
using Ermes.Persons;
using Ermes.Profile.Dto;
using Ermes.Roles;
using Ermes.Teams;
using Ermes.Teams.Dto;
using io.fusionauth.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Location = Ermes.Profile.Dto.Location;

namespace Ermes
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class ErmesAppServiceBase : ApplicationService
    {
        protected ErmesAppServiceBase(
            )
        {
            LocalizationSourceName = ErmesConsts.LocalizationSourceName;
        }

        protected CompetenceAreaType GetCompetenceAreaTypeFromFileName(string filename)
        {
            var splittedFilenameString = filename.Split('.')[0].Split('_');
            Enum.TryParse(splittedFilenameString[1], true, out CompetenceAreaType result);
            return result;
        }

        protected async Task<List<Role>> GetRolesAndCheckOrganization(string[] roles, int? organizationId, PersonManager _personManager, OrganizationManager _organizationManager, ErmesAppSession _session)
        {
            List<Role> roleList;

            if (roles == null || roles.Count() == 0)
                roleList = _personManager.Roles.Where(r => r.Default).ToList();
            else
            {
                roleList = _personManager.Roles.Where(r => roles.Contains(r.Name)).ToList();
                if (roleList.Count < roles.Length)
                    throw new UserFriendlyException(L("InvalidRole", roles.Except(roleList.Select(r => r.Name)).Aggregate((r1, r2) => r1 + ", " + r2)));
            }

            if (organizationId == null)
                throw new UserFriendlyException(L("InvalidOrganizationId", organizationId));
            var org = await _organizationManager.GetOrganizationByIdAsync(organizationId.Value);
            if (org == null)
                throw new UserFriendlyException(L("InvalidOrganizationId", organizationId));

            if (_session.LoggedUserPerson.OrganizationId.HasValue && _session.LoggedUserPerson.OrganizationId.Value != organizationId)
                throw new UserFriendlyException(L("Forbidden_DifferentOrganizations"));

            return roleList;
        }

        protected async Task<ProfileDto> GetProfileInternal(Person person, User user, PersonManager _personManager, MissionManager _missionManager)
        {
            ProfileDto profile = new ProfileDto()
            {
                PersonId = person.Id,
                IsFirstLogin = person.IsFirstLogin,
                User = ObjectMapper.Map<UserDto>(user)
            };

            if (person.Organization != null)
                profile.Organization = ObjectMapper.Map<OrganizationDto>(person.Organization);

            if (person.Team != null)
                profile.Team = ObjectMapper.Map<TeamDto>(person.Team);

            var lastAction = await _personManager.GetLastPersonActionAsync(person.Id);

            if (lastAction != null)
            {
                profile.CurrentStatus = lastAction.CurrentStatus;
                if (lastAction.CurrentStatus == ActionStatusType.Active)
                    profile.CurrentActivityId = await _personManager.GetLastPersonActivityAsync(person.Id);

                if (lastAction.Location != null)
                    profile.Location = new Location(lastAction.Location.Coordinate.X, lastAction.Location.Coordinate.Y, lastAction.Timestamp);
            }
            else
                profile.CurrentStatus = ActionStatusType.Off;

            profile.CurrentMissions = ObjectMapper.Map<List<MissionDto>>(_missionManager.GetCurrentMissions(person));

            if (user.registrations != null && user.registrations.Count > 0)
                profile.User.Roles = user.registrations?.FirstOrDefault().roles.ToArray();

            if (profile.User.Timezone == null)
                profile.User.Timezone = AppConsts.DefaultTimezone;

            return profile;
        }
    
        protected async Task<bool> CheckOrganizationAndTeam(OrganizationManager _organizationManager, TeamManager _teamManager, int? organizationId, int? teamId)
        {
            if (!organizationId.HasValue)
                return true;

            //Check if Organization exists
            var org = await _organizationManager.GetOrganizationByIdAsync(organizationId.Value);
            if (org == null)
                throw new UserFriendlyException(L("InvalidOrganizationId", organizationId.Value));

            //Check if Team exists
            if (teamId.HasValue)
            {
                var team = await _teamManager.GetTeamByIdAsync(teamId.Value);
                if (team == null)
                    throw new UserFriendlyException(L("InvalidTeamId", teamId.Value));
                if (team.OrganizationId != org.Id)
                    throw new UserFriendlyException(L("TeamNotInOrganization", teamId, organizationId.Value));
            }

            return true;
        }
    }
}