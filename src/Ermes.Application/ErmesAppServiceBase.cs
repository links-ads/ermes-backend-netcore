using Abp.Application.Services;
using Abp.BackgroundJobs;
using Abp.UI;
using Ermes.Auth.Dto;
using Ermes.Authorization;
using Ermes.Enums;
using Ermes.EventHandlers;
using Ermes.Gamification;
using Ermes.Gamification.Dto;
using Ermes.Missions;
using Ermes.Missions.Dto;
using Ermes.Organizations;
using Ermes.Organizations.Dto;
using Ermes.Persons;
using Ermes.Profile.Dto;
using Ermes.Roles;
using Ermes.Teams;
using Ermes.Teams.Dto;
using FusionAuthNetCore;
using io.fusionauth.domain;
using io.fusionauth.domain.api;
using io.fusionauth.domain.api.user;
using Microsoft.Extensions.Options;
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

        protected async Task<List<Role>> GetRolesAndCheckOrganizationAndTeam(List<string> roles, int? organizationId, int? teamId, long? personId, PersonManager _personManager, OrganizationManager _organizationManager, TeamManager _teamManager, ErmesAppSession _session, ErmesPermissionChecker _permissionChecker)
        {
            List<Role> roleList;

            if (roles == null || roles.Count() == 0)
                roleList = _personManager.Roles.Where(r => r.Default).ToList();
            else
            {
                roleList = _personManager.Roles.Where(r => roles.Contains(r.Name)).ToList();
                if (roleList.Count < roles.Count)
                    throw new UserFriendlyException(L("InvalidRole", roles.Except(roleList.Select(r => r.Name)).Aggregate((r1, r2) => r1 + ", " + r2)));
            }

            var users_CanCreateCitizenOrPersonCrossOrganization = _permissionChecker.IsGranted(_session.Roles, AppPermissions.Users.Users_CanCreateCitizenOrPersonCrossOrganization);
            var users_CanEditColleagues = _permissionChecker.IsGranted(_session.Roles, AppPermissions.Users.Users_CanEditColleagues);
            if (organizationId == null || organizationId == 0) //if null, must be a citizen or must have the right permission
            {
                if (!users_CanCreateCitizenOrPersonCrossOrganization && roleList.Count(a => a.Name == AppRoles.CITIZEN) == 0)
                    throw new UserFriendlyException(L("InvalidOrganizationId", organizationId));

                if (personId.HasValue) //cannot edit other profiles without permissions
                {
                    if (!users_CanEditColleagues && _session.LoggedUserPerson.Id != personId && !users_CanCreateCitizenOrPersonCrossOrganization)
                        throw new UserFriendlyException(L("Forbidden_DifferentOrganizations"));
                }
            }
            else
            {
                var org = await _organizationManager.GetOrganizationByIdAsync(organizationId.Value);
                if (org == null)
                    throw new UserFriendlyException(L("InvalidOrganizationId", organizationId));
                
                
                //TODO: add here check for a self registered user
                if (!users_CanCreateCitizenOrPersonCrossOrganization)
                {
                    //cannot edit people belonging to other organizations without the right permission
                    if (_session.LoggedUserPerson.OrganizationId.HasValue && _session.LoggedUserPerson.OrganizationId.Value != organizationId && _session.LoggedUserPerson.OrganizationId != org.ParentId)
                        throw new UserFriendlyException(L("Forbidden_DifferentOrganizations"));

                    if (personId.HasValue) //cannot edit other profiles without permissions
                    {
                        if (!users_CanEditColleagues && _session.LoggedUserPerson.Id != personId)
                            throw new UserFriendlyException(L("Forbidden_DifferentOrganizations"));
                    }
                }
                //Check if Team exists
                if (teamId.HasValue && teamId.Value > 0)
                {
                    var team = await _teamManager.GetTeamByIdAsync(teamId.Value);
                    if (team == null)
                        throw new UserFriendlyException(L("InvalidTeamId", teamId.Value));
                    if (team.OrganizationId != org.Id)
                        throw new UserFriendlyException(L("TeamNotInOrganization", teamId, organizationId.Value));
                }
            }

            return roleList;
        }

        protected async Task<ProfileDto> GetProfileInternal(Person person, User user, PersonManager _personManager, MissionManager _missionManager, GamificationManager _gamificationManager, ErmesAppSession _session, IBackgroundJobManager _backgroundJobManager)
        {
            ProfileDto profile = new ProfileDto()
            {
                PersonId = person.Id,
                IsFirstLogin = person.IsFirstLogin,
                IsNewUser = person.IsNewUser,
                LegacyId = person.LegacyId,
                Points = person.Points,
                Level = person.LevelId.HasValue ? person.Level.Name : null,
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

            profile.User.Roles = await _personManager.GetPersonRoleNamesAsync(profile.PersonId);
            if(profile.User.Roles.Count == 0)
            {
                var reg = user?.registrations?.ElementAt(0);
                if (reg != null) //look for roles from FusionAuth
                {
                    var roles = await _personManager.GetRolesByName(reg.roles);
                    foreach (var r in roles)
                    {
                        await _personManager.InsertPersonRoleAsync(new PersonRole()
                        {
                            PersonId = person.Id,
                            RoleId = r.Id
                        });
                        profile.User.Roles.Add(r.Name);
                        if (r.Name == AppRoles.CITIZEN)
                            person.LevelId = (await _gamificationManager.GetDefaultLevel()).Id;
                    }
                }
                else //assign default role
                {
                    var defaultRole = await _personManager.GetDefaultRole();
                    await _personManager.InsertPersonRoleAsync(new PersonRole()
                    {
                        PersonId = person.Id,
                        RoleId = defaultRole.Id
                    });
                    profile.User.Roles.Add(defaultRole.Name);
                    if (defaultRole.Name == AppRoles.CITIZEN)
                        person.LevelId = (await _gamificationManager.GetDefaultLevel()).Id;
                }
            }

            foreach (var role in profile.User.Roles)
            {
                switch (role)
                {
                    case AppRoles.ADMINISTRATOR:
                        profile.Permissions = AppRoles.ADMINISTRATOR_PERMISSION_LIST;
                        break;
                    case AppRoles.CITIZEN:
                        profile.Permissions = AppRoles.CITIZEN_PERMISSION_LIST;
                        break;
                    case AppRoles.FIRST_RESPONDER:
                    case AppRoles.TEAM_LEADER:
                        profile.Permissions = AppRoles.FIRST_RESPONDER_PERMISSION_LIST;
                        break;
                    case AppRoles.ORGANIZATION_MANAGER:
                    case AppRoles.DECISION_MAKER:
                        profile.Permissions = AppRoles.ORGANIZATION_MANAGER_PERMISSION_LIST;
                        break;
                    default:
                        break;
                }
            }

            if (profile.User.Timezone == null)
                profile.User.Timezone = AppConsts.DefaultTimezone;

            //set Default language, but does not update User profile on FusionAuth
            if (profile.User.PreferredLanguages == null || profile.User.PreferredLanguages.Count == 0)
                profile.User.PreferredLanguages = new List<string>() { AppConsts.DefaultLanguage };

            profile.Medals = ObjectMapper.Map<List<MedalDto>>(await _gamificationManager.GetPersonMedalsAsync(profile.PersonId));
            profile.Badges = ObjectMapper.Map<List<BadgeDto>>(await _gamificationManager.GetPersonBadgesAsync(profile.PersonId));

            return profile;
        }

        protected async Task<User> UpdateUserInternalAsync(UserDto userDto, IOptions<FusionAuthSettings> _fusionAuthSettings, List<string> rolesToAssign)
        {
            var client = FusionAuth.GetFusionAuthClient(_fusionAuthSettings.Value);

            //Update user-registration on FusionAuth
            //this function allows to update roles
            var regRequest = new RegistrationRequest()
            {
                user = ObjectMapper.Map<User>(userDto),
                registration = new UserRegistration()
                {
                    applicationId = new Guid(_fusionAuthSettings.Value.ApplicationId),
                    roles = rolesToAssign,
                },
                sendSetPasswordEmail = false,
                skipVerification = true,
                skipRegistrationVerification = true
            };
            var regResponse = await client.UpdateRegistrationAsync(regRequest.user.id, regRequest);
            if (!regResponse.WasSuccessful())
                throw new UserFriendlyException(regResponse.errorResponse.ToString());
            //Update user on FusionAuth
            //this function allows to update user profile
            var userToUpdate = new UserRequest()
            {
                user = ObjectMapper.Map<User>(userDto),
                sendSetPasswordEmail = false,
                skipVerification = true,
            };

            var response = await client.UpdateUserAsync(userToUpdate.user.id, userToUpdate);
            
            if(response.WasSuccessful())
                return response.successResponse.user;
            else
            {
                var fa_error = FusionAuth.ManageErrorResponse(response);
                throw new UserFriendlyException(fa_error.ErrorCode, fa_error.HasTranslation ? L(fa_error.Message) : fa_error.Message);
            }
        }

        protected async Task<Person> CreateOrUpdatePersonInternalAsync(Person person, User user, int? organizationId, int? teamId, bool isFirstLogin, bool isNewUser, List<Role> rolesToAssign, PersonManager _personManager)
        {
            //Manage Person on Ermes DB
            if (person == null)
            {
                person = new Person()
                {
                    FusionAuthUserGuid = user.id.Value
                };
            }

            person.OrganizationId = organizationId;
            if(teamId.HasValue)
                person.TeamId = teamId;
            person.IsFirstLogin = isFirstLogin;
            person.IsNewUser = isNewUser;
            person.Username = user.username;

            Logger.Info("Ermes: Create or update Person: " + person.Username);
            long personId = await _personManager.InsertOrUpdatePersonAsync(person);

            //Delete old associations
            await _personManager.DeletePersonRolesAsync(personId);
            await CurrentUnitOfWork.SaveChangesAsync();

            // Assign roles
            foreach (Role rta in rolesToAssign)
            {
                PersonRole pr = new PersonRole()
                {
                    PersonId = personId,
                    RoleId = rta.Id
                };
                await _personManager.InsertPersonRoleAsync(pr);

                //If citizen, set Ready as default state;
                //This allows to send communications to citizens, since they cannot change their status from the UI of the Chatbot;
                //Communications are not sent to persons in Off status
                if (rta.Name == AppRoles.CITIZEN)
                {
                    var lastAction = await _personManager.GetLastPersonActionAsync(personId);
                    if (lastAction == null)
                    {
                        await _personManager.InsertPersonActionStatusAsync(new PersonActionStatus()
                        {
                            CurrentStatus = ActionStatusType.Ready,
                            PersonId = personId,
                            Timestamp = DateTime.UtcNow,
                            Status = ActionStatusType.Ready,
                            CreatorUserId = personId
                        });
                    }
                }
            }

            await CurrentUnitOfWork.SaveChangesAsync();
            return person;
        }

        protected async Task<User> CreateUserInternalAsync(UserDto userDto, List<string> rolesToAssign, IOptions<FusionAuthSettings> _fusionAuthSettings, IOptions<ErmesSettings> _ermesSettings)
        {
            var client = FusionAuth.GetFusionAuthClient(_fusionAuthSettings.Value);
            var user = ObjectMapper.Map<User>(userDto);

            //Set the password based on current project
            string project = _ermesSettings != null ? _ermesSettings.Value.ErmesProject : "";
            if (string.IsNullOrWhiteSpace(project))
                project = ErmesConsts.DefaultProjectName;
            user.password = string.Concat(project.ToLower(), ErmesConsts.DefaultYear);
            //Create user on FusionAuth
            var newUser = new RegistrationRequest()
            {
                user = user,
                registration = new UserRegistration()
                {
                    applicationId = new Guid(_fusionAuthSettings.Value.ApplicationId),
                    roles = rolesToAssign,
                },
                sendSetPasswordEmail = false,
                skipVerification = true,
                skipRegistrationVerification = true
            };

            var response = await client.RegisterAsync(null, newUser);

            if (response.WasSuccessful())
            {
                if (response.successResponse.user.id.HasValue)
                {
                    return response.successResponse.user;
                }
                else
                    throw new UserFriendlyException(L("FusionAuthUnknonwError"));
            }
            else
            {
                var fa_error = FusionAuth.ManageErrorResponse(response);
                throw new UserFriendlyException(fa_error.ErrorCode, fa_error.HasTranslation ? L(fa_error.Message) : fa_error.Message);
            }
        }

        protected async Task<bool> ImportUsersInternalAsync(List<UserDto> users, IOptions<FusionAuthSettings> _fusionAuthSettings)
        {
            var client = FusionAuth.GetFusionAuthClient(_fusionAuthSettings.Value);

            var fa_users = ObjectMapper.Map<List<User>>(users);

            fa_users.Select(u => {
                    u.registrations = new List<UserRegistration>() {
                        new UserRegistration()
                        {
                            applicationId = new Guid(_fusionAuthSettings.Value.ApplicationId),
                            roles = users.Where(user => user.Id == u.id).Single().Roles,
                            verified = true
                        }
                    };
                    u.verified = true;
                    u.active = true;
                    return u;
                }).ToList();
            //Create user on FusionAuth
            var import = new ImportRequest()
            {
                users = fa_users,
                validateDbConstraints = false
            };

            var response = await client.ImportUsersAsync(import);

            if (response.WasSuccessful())
                return true;
            else
            {
                var fa_error = FusionAuth.ManageErrorResponse(response);
                throw new UserFriendlyException(fa_error.ErrorCode, fa_error.HasTranslation ? L(fa_error.Message) : fa_error.Message);
            }
        }
    }
}