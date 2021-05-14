using Abp.Application.Services.Dto;
using Abp.UI;
using Ermes.Attributes;
using Ermes.Auth.Dto;
using Ermes.Authorization;
using Ermes.Dto.Datatable;
using Ermes.Helpers;
using Ermes.Missions;
using Ermes.Organizations;
using Ermes.Permissions;
using Ermes.Persons;
using Ermes.Profile.Dto;
using Ermes.Roles;
using Ermes.Teams;
using Ermes.Users.Dto;
using FusionAuthNetCore;
using io.fusionauth.domain;
using io.fusionauth.domain.api.user;
using io.fusionauth.domain.search;
using Microsoft.Extensions.Options;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ermes.Users
{
    [ErmesAuthorize(AppPermissions.Backoffice)]
    public class UsersAppService: ErmesAppServiceBase, IUsersAppService
    {
        private readonly ErmesAppSession _session;
        private readonly PersonManager _personManager;
        private readonly PermissionManager _permissionManager;
        private readonly MissionManager _missionManager;
        private readonly OrganizationManager _organizationManager;
        private readonly TeamManager _teamManager;
        private readonly IOptions<FusionAuthSettings> _fusionAuthSettings;

        public UsersAppService(
                    ErmesAppSession session,
                    PersonManager personManger,
                    PermissionManager permissionManager,
                    MissionManager missionManager,
                    IOptions<FusionAuthSettings> fusionAuthSettings,
                    OrganizationManager organizationManager,
                    TeamManager teamManager
            )
        {
            _session = session;
            _personManager = personManger;
            _missionManager = missionManager;
            _fusionAuthSettings = fusionAuthSettings;
            _teamManager = teamManager;
            _organizationManager = organizationManager;
            _permissionManager = permissionManager;
        }

        #region Private
        private async Task<PagedResultDto<ProfileDto>> InternalGetProfiles(GetUsersInput input)
        {
            PagedResultDto<ProfileDto> result = new PagedResultDto<ProfileDto>();
            var client = FusionAuth.GetFusionAuthClient(_fusionAuthSettings.Value);
            if (input.Search == null || input.Search.Value == null)
                input.Search = new DTSearch() { Regex = false, Value = "" };
            var response = await client.SearchUsersByQueryAsync(new SearchRequest()
            {
                search = new UserSearchCriteria()
                {
                    numberOfResults = input.MaxResultCount,
                    sortFields = input.Order.Select(a => new SortField() { name = a.Column, order = FusionAuth.GetFusionAuthSortParam(a.Dir.ToString()) }).ToList(),
                    startRow = input.SkipCount,
                    queryString = input.Search.Value
                }
            });
            if (response.WasSuccessful())
            {
                result.TotalCount = response.successResponse.total.HasValue ? (int)response.successResponse.total : 0;
                if (result.TotalCount > 0)
                {
                    var list = new List<ProfileDto>();
                    foreach (var item in response.successResponse.users)
                    {
                        var person = await _personManager.GetPersonByFusionAuthUserGuidAsync(item.id.Value, item.username);

                        ProfileDto profile = await GetProfileInternal(person, item, _personManager, _missionManager);

                        list.Add(profile);
                    }
                    result.Items = list;
                }
            }
            else
            {
                var fa_error = FusionAuth.ManageErrorResponse(response);
                throw new UserFriendlyException(fa_error.ErrorCode, fa_error.HasTranslation ? L(fa_error.Message) : fa_error.Message);

            }
            return result;
        }

        private async Task<long> CreateUserAsync(UpdateProfileInput input)
        {
            if (input.User.Timezone == null || input.User.Timezone.Length < 2)
                input.User.Timezone = AppConsts.DefaultTimezone;

            await CheckOrganizationAndTeam(_organizationManager, _teamManager, input.OrganizationId, input.TeamId);

            List<Role> rolesToAssign = await GetRolesAndCheckOrganization(input.User.Roles, input.OrganizationId, _personManager, _organizationManager, _session);

            var newUser = await CreateUserInternalAsync(input.User, rolesToAssign);

            //Create Person on Ermes DB
            var newPerson = new Person()
            {
                FusionAuthUserGuid = newUser.id.Value,
                Username = newUser.username,
                OrganizationId = input.OrganizationId,
                TeamId = input.TeamId
            };

            Logger.Info("Ermes: Create Person: " + newPerson.Username);
            long personId = await _personManager.InsertPerson(newPerson);

            // Assign roles
            foreach (Role rta in rolesToAssign)
            {
                PersonRole pr = new PersonRole()
                {
                    PersonId = personId,
                    RoleId = rta.Id
                };
                await _personManager.InsertPersonRoleAsync(pr);
            }

            return personId;
        }

        private async Task<User> CreateUserInternalAsync(UserDto userDto, List<Role> rolesToAssign)
        {
            var client = FusionAuth.GetFusionAuthClient(_fusionAuthSettings.Value);

            //Create user on FusionAuth
            var newUser = new RegistrationRequest()
            {
                user = ObjectMapper.Map<User>(userDto),
                registration = new UserRegistration()
                {
                    applicationId = new Guid(_fusionAuthSettings.Value.ApplicationId),
                    roles = rolesToAssign.Select(r => r.Name).ToList()
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
        #endregion

        public virtual async Task<DTResult<ProfileDto>> GetUsers(GetUsersInput input)
        {
            PagedResultDto<ProfileDto> result = await InternalGetProfiles(input);
            return new DTResult<ProfileDto>(input.Draw, result.TotalCount, result.Items.Count, result.Items.ToList());
        }


        [OpenApiOperation("Create User",
            @"
                BirthDate format: yyyy-mm-dd
            "
        )]
        public virtual async Task<long> CreateUser(UpdateProfileInput input)
        {
            return await CreateUserAsync(input);
        }
    }
}
