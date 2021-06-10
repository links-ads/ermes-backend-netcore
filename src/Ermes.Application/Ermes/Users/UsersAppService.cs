﻿using Abp.Application.Services.Dto;
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
using io.fusionauth.domain.api;
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
        private readonly MissionManager _missionManager;
        private readonly OrganizationManager _organizationManager;
        private readonly TeamManager _teamManager;
        private readonly ErmesPermissionChecker _permissionChecker;
        private readonly IOptions<FusionAuthSettings> _fusionAuthSettings;
        private readonly IOptions<ErmesSettings> _ermesSettings;

        public UsersAppService(
                    ErmesAppSession session,
                    PersonManager personManger,
                    MissionManager missionManager,
                    IOptions<FusionAuthSettings> fusionAuthSettings,
                    IOptions<ErmesSettings> ermesSettings,
                    OrganizationManager organizationManager,
                    TeamManager teamManager,
                    ErmesPermissionChecker permissionChecker
            )
        {
            _session = session;
            _personManager = personManger;
            _missionManager = missionManager;
            _fusionAuthSettings = fusionAuthSettings;
            _ermesSettings = ermesSettings;
            _teamManager = teamManager;
            _organizationManager = organizationManager;
            _permissionChecker = permissionChecker;
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
                currentUser = await UpdateUserInternalAsync(input.User, _fusionAuthSettings);
                person = _personManager.GetPersonByFusionAuthUserGuid(currentUser.id.Value);
            }

            person = await CreateOrUpdatePersonInternalAsync(person, currentUser, input.OrganizationId, input.TeamId, input.IsFirstLogin, rolesToAssign, _personManager);

            return new CreateOrUpdateUserOutput()
            {
                Profile = await GetProfileInternal(person, currentUser, _personManager, _missionManager)
            };
        }
    }
}
