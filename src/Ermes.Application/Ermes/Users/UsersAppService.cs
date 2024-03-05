using Abp.Application.Services.Dto;
using Abp.BackgroundJobs;
using Abp.UI;
using Ermes.Attributes;
using Ermes.Authorization;
using Ermes.Dto.Datatable;
using Ermes.Gamification;
using Ermes.Missions;
using Ermes.Organizations;
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
    public class UsersAppService : ErmesAppServiceBase, IUsersAppService
    {
        private readonly ErmesAppSession _session;
        private readonly PersonManager _personManager;
        private readonly MissionManager _missionManager;
        private readonly GamificationManager _gamificationManager;
        private readonly OrganizationManager _organizationManager;
        private readonly TeamManager _teamManager;
        private readonly ErmesPermissionChecker _permissionChecker;
        private readonly IOptions<FusionAuthSettings> _fusionAuthSettings;
        private readonly IOptions<ErmesSettings> _ermesSettings;
        private readonly IBackgroundJobManager _jobManager;

        public UsersAppService(
                    ErmesAppSession session,
                    PersonManager personManger,
                    MissionManager missionManager,
                    GamificationManager gamificationManager,
                    IOptions<FusionAuthSettings> fusionAuthSettings,
                    IOptions<ErmesSettings> ermesSettings,
                    OrganizationManager organizationManager,
                    TeamManager teamManager,
                    ErmesPermissionChecker permissionChecker,
                    IBackgroundJobManager jobManager
            )
        {
            _session = session;
            _personManager = personManger;
            _missionManager = missionManager;
            _gamificationManager = gamificationManager;
            _fusionAuthSettings = fusionAuthSettings;
            _ermesSettings = ermesSettings;
            _teamManager = teamManager;
            _organizationManager = organizationManager;
            _permissionChecker = permissionChecker;
            _jobManager = jobManager;
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
                var currentOrgId = _session.LoggedUserPerson.OrganizationId;
                result.TotalCount = response.successResponse.total.HasValue ? (int)response.successResponse.total : 0;
                if (result.TotalCount > 0)
                {
                    var list = new List<ProfileDto>();
                    var permission = _permissionChecker.IsGranted(_session.Roles, AppPermissions.Users.Users_CanCreateCitizenOrPersonCrossOrganization);
                    foreach (var item in response.successResponse.users)
                    {
                        var person = await _personManager.GetPersonByFusionAuthUserGuidAsync(item.id.Value, item.email, item.username);
                        if (!permission) //admin can see everyone
                        {
                            //do not return citizens
                            if (!currentOrgId.HasValue || !person.OrganizationId.HasValue || (currentOrgId.Value != person.OrganizationId && person.Organization.ParentId != currentOrgId))
                                continue;
                        }

                        ProfileDto profile = await GetProfileInternal(person, item, _personManager, _missionManager, _gamificationManager, _session, _jobManager);
                        list.Add(profile);
                    }
                    result.Items = list.OrderBy(a => a.User?.DisplayName).ToList();
                }
            }
            else
            {
                var fa_error = FusionAuth.ManageErrorResponse(response);
                throw new UserFriendlyException(fa_error.ErrorCode, fa_error.HasTranslation ? L(fa_error.Message) : fa_error.Message);

            }
            return result;
        }

        private async Task<PagedResultDto<ProfileDto>> InternalGetUncompletedProfiles(GetUncompletedUsersInput input)
        {
            PagedResultDto<ProfileDto> result = new PagedResultDto<ProfileDto>();
            var client = FusionAuth.GetFusionAuthClient(_fusionAuthSettings.Value);
            if (input.Search == null || input.Search.Value == null)
                input.Search = new DTSearch() { Regex = false, Value = "" };
            var response = await client.SearchUsersByQueryAsync(new SearchRequest()
            {
                search = new UserSearchCriteria()
                {
                    numberOfResults = 10000,
                    sortFields = input.Order.Select(a => new SortField() { name = a.Column, order = FusionAuth.GetFusionAuthSortParam(a.Dir.ToString()) }).ToList(),
                    startRow = 0,
                    queryString = input.Search.Value
                }
            });
            if (response.WasSuccessful())
            {
                if (response.successResponse.total > 0)
                {
                    var list = new List<ProfileDto>();
                    foreach (var item in response.successResponse.users)
                    {
                        var person = await _personManager.GetPersonByFusionAuthUserGuidAsync(item.id.Value, item.email, item.username);
                        ProfileDto profile = await GetProfileInternal(person, item, _personManager, _missionManager, _gamificationManager, _session, _jobManager);
                        if (!item.verified.HasValue || !item.verified.Value)
                            list.Add(profile);
                        else if (!profile.User.Roles.Any(a => a == AppRoles.CITIZEN || a == AppRoles.ADMINISTRATOR) && !person.OrganizationId.HasValue)
                            list.Add(profile);
                    }
                    result.TotalCount = list.Count;
                    result.Items = list
                            .Skip(input.SkipCount)
                            .Take(input.MaxResultCount)
                            .OrderBy(a => a.User?.DisplayName)
                            .ToList();
                }
            }
            else
            {
                var fa_error = FusionAuth.ManageErrorResponse(response);
                throw new UserFriendlyException(fa_error.ErrorCode, fa_error.HasTranslation ? L(fa_error.Message) : fa_error.Message);

            }
            return result;
        }
        #endregion

        public virtual async Task<DTResult<ProfileDto>> GetUsers(GetUsersInput input)
        {
            PagedResultDto<ProfileDto> result = await InternalGetProfiles(input);
            return new DTResult<ProfileDto>(input.Draw, result.TotalCount, result.Items.Count, result.Items.ToList());
        }

        [ErmesAuthorize(AppPermissions.Users.Users_CanSeeUncompletedUsers)]
        public virtual async Task<DTResult<ProfileDto>> GetUncompletedUsers(GetUncompletedUsersInput input)
        {
            PagedResultDto<ProfileDto> result = await InternalGetUncompletedProfiles(input);
            return new DTResult<ProfileDto>(input.Draw, result.TotalCount, result.Items.Count, result.Items.ToList());
        }


        [OpenApiOperation("Create User",
            @"
                BirthDate format: yyyy-mm-dd
            "
        )]
        public virtual async Task<CreateOrUpdateUserOutput> CreateOrUpdateUser(UpdateProfileInput input)
        {
            if (input.User.Timezone == null || input.User.Timezone.Length < 2)
                input.User.Timezone = AppConsts.DefaultTimezone;

            List<Role> rolesToAssign = await GetRolesAndCheckOrganizationAndTeam(input.User.Roles, input.OrganizationId, input.TeamId, input.PersonId, _personManager, _organizationManager, _teamManager, _session, _permissionChecker);

            User currentUser;
            Person person = null;
            if (input.User.Id == Guid.Empty)
                currentUser = await CreateUserInternalAsync(input.User, input.User.Roles, _fusionAuthSettings, _ermesSettings);
            else
            {
                currentUser = await UpdateUserInternalAsync(input.User, _fusionAuthSettings, input.User.Roles);
                person = _personManager.GetPersonByFusionAuthUserGuid(currentUser.id.Value);
            }

            person = await CreateOrUpdatePersonInternalAsync(person, currentUser, input.OrganizationId, input.TeamId, input.IsFirstLogin, input.IsNewUser, rolesToAssign, _personManager);

            return new CreateOrUpdateUserOutput()
            {
                Profile = await GetProfileInternal(person, currentUser, _personManager, _missionManager, _gamificationManager, _session, _jobManager)
            };
        }



        //[OpenApiOperation("Create User",
        //    @"
        //        BirthDate format: yyyy-mm-dd
        //    "
        //)]
        //public async Task RegisterUser(RegisterUserEventInput input)
        //{
        //    Logger.Info("-------------- WebHook works fine! ---------------------");
        //    return;
        //}
    }
}
